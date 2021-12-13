using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using SaveDataCurrentVersion = SaveData1;
using System.Text;
using System.Security.Cryptography;
/*
<List�� 3���� ���������� �ѹ��� Jsonȭ ���� ���� ������>

SaveDataList �ʿ�
1. BinaryData�� ����� List��ü�� �����ϰ���, Deserialize�δ� �ҷ��� �� ����.
2. TextData�� �ʿ�����. List ���׸����ڸ� SaveDataBase�� �ٲ���� Deserialize�� �Ұ��������� ����.
������ JObject�� ������ Ȯ�� �� �����ؾ��ϴµ�, List���� Serialize/Deserialize�ؼ��� JObject�� ������ �� ������.
���� Binary�� ���������� �ν��Ͻ�ȭ�ؼ� �����ϰ� �ε��ؾ���.

����Ʈ�� SerializeObject�� Jsonȭ �ϸ�, JObject�δ� ����� �� ����.
Ŭ������ SerializeObject�� Jsonȭ �ϸ�, JObject�� ����� �� �ִ�.

<���>
������ ��θ� �ε��ϰ�, (6����)
������ ���� �����ϴ� ������� 3������ �ٽø���.

<�߰� ������>
SaveDataBase�� abstract �߻� Ŭ������ �����ߴ���,
��� SaveData ������ �θ�� Version�� Ȯ���ؾ��ϴµ�,
������ȭ(deserialize)�� ���� �����ع���. (text�� binary ���� ����)
�θ��� SaveDataBase�� �߻�Ŭ������ �ƴϵ��� �����ߴ��� �ذ��.
�Ƹ���, �ν��Ͻ�ȭ �Ҽ� ���� �߻�Ŭ������ �߻��� �����ε�.
*/

//=========================================================================================
// SaveData ���� �߰��� �ؾ��� ��.
// 1. CheckVersionAndDeserialize �޼ҵ� ����. (�����ε� �� ���� ��� Swith�� ����)
// 2. �� ������ �߰��Ǳ� �� ���� ������ SaveDataBase��� Ŭ������ VersionUpgrade() �籸���ϱ�.
//=========================================================================================

public struct AesKeyVI
{
    public byte[] key;
    public byte[] IV;
}

public static class SaveLoadSystem
{
    public enum Modes
    {
        Text,
        Binary,
    }

    private static List<string> binaryPath;
    private static List<string> txtPath;

    public static Modes CurrentMode { get; set; } = Modes.Text;
    public static readonly int CurrentVersion = 1;

    public static AesKeyVI aes = new AesKeyVI();

    public static void Init()
    {
        PathLoad();
        AESKeyLoad();
    }
    public static void PathLoad()
    {
        using (StreamReader reader = new StreamReader(new FileStream("binaryPath.json", FileMode.Open)))
        {
            string json = reader.ReadToEnd();
            binaryPath = JsonConvert.DeserializeObject<List<string>>(json);
            //Debug.Log(json);
        }
        using (StreamReader reader = new StreamReader(new FileStream("txtPath.json", FileMode.Open)))
        {
            string json = reader.ReadToEnd();
            txtPath = JsonConvert.DeserializeObject<List<string>>(json);
            //Debug.Log(json);
        }
    }

    public static void AESKeyLoad()
    {
        if(!File.Exists("AesKey.json"))
        {
            CreateAESKey();
            using (StreamWriter writer = new StreamWriter(new FileStream("AesKey.json", FileMode.Create)))
            {
                string json = JsonConvert.SerializeObject(aes);
                writer.WriteLine(json);
                Debug.Log($"Key Save!\n{json}");
            }
        }
        else
        {
            using (StreamReader reader = new StreamReader(new FileStream("AesKey.json", FileMode.Open)))
            {
                string json = reader.ReadToEnd();
                aes = JsonConvert.DeserializeObject<AesKeyVI>(json);
                Debug.Log($"Key Load!");
            }
        }
    }

    public static void CreateAESKey()
    {
        using (Aes myAes = Aes.Create())
        {
            aes.key = myAes.Key;
            aes.IV = myAes.IV;
        }
    }

    public static SaveDataBase Load(int slotIndex)
    {
        SaveDataBase data = new SaveData();
        if (CurrentMode == Modes.Text)
        {
            using (StreamReader reader = new StreamReader(new FileStream(txtPath[slotIndex], FileMode.Open)))
            {       
                string json = reader.ReadToEnd();
                data = CheckVersionAndDeserialize(json);
                Debug.Log(json);
            }
            Debug.Log("�ε�Ϸ�");

            data = VersionUpdate(data);

            return data;
        }
        else
        {
            byte[] decrypted;
            //1) ��ȣȭ�� byte[]�� ��������.
            using (BinaryReader reader = new BinaryReader(new FileStream(txtPath[slotIndex], FileMode.Open)))
            {
                decrypted = reader.ReadBytes((int)reader.BaseStream.Length); 
                
            }

            /*
            //2) byte[]�� ��ȣȭ�ϱ�.
            byte[] strByte;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = aes.key;
                aesAlg.IV = aes.IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(decrypted))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (BinaryReader reader = new BinaryReader(csDecrypt))
                        {
                            var len = reader.PeekChar();
                            strByte = reader.ReadBytes(reader.PeekChar()); //���̵� 0�̰�... peekchar�� -1�ε�... ��ü ��?
                        }
                    }
                }
            }
            */

            //2) byte[]�� ��ȣȭ�ϱ�.
            byte[] strByte;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = aes.key;
                aesAlg.IV = aes.IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var ms = new MemoryStream())
                using (var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(decrypted, 0, decrypted.Length);
                    cryptoStream.FlushFinalBlock();

                    strByte = ms.ToArray();
                }
            }



            //3) byte[]�� string����
            string json = Encoding.UTF8.GetString(strByte);

            //4) json�� �����ͷ�
            data = CheckVersionAndDeserialize(json);
            Debug.Log(json);

            data = VersionUpdate(data);

            return data;



            /*���̹��� BsonReader~*/
            //SaveDataBase db;
            //using (BsonReader reader = new BsonReader(new FileStream(binaryPath[slotIndex], FileMode.Open)))
            //{
            //    JsonSerializer serializer = new JsonSerializer();

            //    db = serializer.Deserialize<SaveDataBase>(reader);
            //}
            //data = CheckVersionAndDeserialize(db.Version, slotIndex);
            //Debug.Log("�ε�Ϸ�");

            //data = VersionUpdate(data);

            //return data;
        }
    }

    public static void Save(SaveDataBase data, int slotIndex)
    {
        if (CurrentMode == Modes.Text)
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(txtPath[slotIndex], FileMode.Create)))
            {
                string json = JsonConvert.SerializeObject(data);
                writer.WriteLine(json);
                Debug.Log(json);
            }
        }
        else if(CurrentMode == Modes.Binary)
        {
            //1) json string����
            string json = JsonConvert.SerializeObject(data);
            //2) string�� byte[]��
            byte[] strByte = Encoding.UTF8.GetBytes(json);
            //3) AES encrypt byte[] to byte[]
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = aes.key;
                aesAlg.IV = aes.IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (BinaryWriter writer = new BinaryWriter(csEncrypt))
                        {
                            writer.Write(strByte);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            //4) ��ȣȭ�� byte[]�� ���̳ʸ� json���Ϸ� ����.
            using (BinaryWriter writer = new BinaryWriter(new FileStream(txtPath[slotIndex], FileMode.Create)))
            {
                writer.Write(encrypted);
            }


            /*���̹��� BsonWriter~*/
            //using (BsonWriter writer = new BsonWriter(new FileStream(binaryPath[slotIndex], FileMode.Create)))
            //{
            //    JsonSerializer serializer = new JsonSerializer();
            //    serializer.Serialize(writer, data);
            //}
        }
        Debug.Log("����Ϸ�");
    }

    public static SaveDataBase CheckVersionAndDeserialize(string json)
    {
        JObject rss = JObject.Parse(json);
        var version = (int)rss["Version"];
        switch (version)
        {
            case 0:
                return JsonConvert.DeserializeObject<SaveData>(json);
            case 1:
                return JsonConvert.DeserializeObject<SaveData1>(json);
            default:
                return SaveData.DefaultValue;
        }
    }
    public static SaveDataBase CheckVersionAndDeserialize(int version, int slotIndex)
    {
        SaveDataBase db;
        using (BsonReader reader = new BsonReader(new FileStream(binaryPath[slotIndex], FileMode.Open)))
        {
            JsonSerializer serializer = new JsonSerializer();
            switch (version)
            {
                case 0:
                    db = serializer.Deserialize<SaveData>(reader);
                    break;
                case 1:
                    db = serializer.Deserialize<SaveData1>(reader);
                    break;
                default:
                    db = SaveData.DefaultValue;
                    break;
            }
        }
        return db;
    }

    public static SaveDataBase VersionUpdate(SaveDataBase data)
    {
        while (data.Version != CurrentVersion)
        {
            data = data.VersionUpgrade();
        }
        return data;
    }
}
