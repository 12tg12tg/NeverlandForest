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
<List로 3슬롯 저장정보를 한번에 Json화 했을 때의 문제점>

SaveDataList 필요
1. BinaryData로 저장시 List자체를 저장하고나면, Deserialize로는 불러올 수 없음.
2. TextData도 필요해짐. List 제네릭인자를 SaveDataBase로 바꿨더니 Deserialize가 불가능해졌기 때문.
때문에 JObject로 버전을 확인 후 편집해야하는데, List만을 Serialize/Deserialize해서는 JObject로 생성할 수 없었음.
따라서 Binary와 마찬가지로 인스턴스화해서 저장하고 로드해야함.

리스트를 SerializeObject로 Json화 하면, JObject로는 사용할 수 없다.
클래스를 SerializeObject로 Json화 하면, JObject로 사용할 수 있다.

<결론>
각각의 경로를 로드하고, (6가지)
파일을 따로 저장하는 방식으로 3번부터 다시만듬.

<추가 문제점>
SaveDataBase를 abstract 추상 클래스로 구현했더니,
모든 SaveData 버전의 부모라서 Version을 확인해야하는데,
역직렬화(deserialize)가 전부 실패해버림. (text와 binary 공통 문제)
부모인 SaveDataBase를 추상클래스가 아니도록 수정했더니 해결됨.
아마도, 인스턴스화 할수 없는 추상클래스라서 발생한 문제인듯.
*/

//=========================================================================================
// SaveData 버전 추가시 해야할 일.
// 1. CheckVersionAndDeserialize 메소드 수정. (오버로딩 두 가지 모두 Swith문 수정)
// 2. 새 버전이 추가되기 전 가장 마지막 SaveDataBase상속 클래스에 VersionUpgrade() 재구현하기.
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
            Debug.Log("로드완료");

            data = VersionUpdate(data);

            return data;
        }
        else
        {
            byte[] decrypted;
            //1) 암호화된 byte[]을 꺼내오기.
            using (BinaryReader reader = new BinaryReader(new FileStream(txtPath[slotIndex], FileMode.Open)))
            {
                decrypted = reader.ReadBytes((int)reader.BaseStream.Length); 
                
            }

            /*
            //2) byte[]를 복호화하기.
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
                            strByte = reader.ReadBytes(reader.PeekChar()); //길이도 0이고... peekchar도 -1인데... 대체 왜?
                        }
                    }
                }
            }
            */

            //2) byte[]를 복호화하기.
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



            //3) byte[]를 string으로
            string json = Encoding.UTF8.GetString(strByte);

            //4) json을 데이터로
            data = CheckVersionAndDeserialize(json);
            Debug.Log(json);

            data = VersionUpdate(data);

            return data;



            /*바이바이 BsonReader~*/
            //SaveDataBase db;
            //using (BsonReader reader = new BsonReader(new FileStream(binaryPath[slotIndex], FileMode.Open)))
            //{
            //    JsonSerializer serializer = new JsonSerializer();

            //    db = serializer.Deserialize<SaveDataBase>(reader);
            //}
            //data = CheckVersionAndDeserialize(db.Version, slotIndex);
            //Debug.Log("로드완료");

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
            //1) json string으로
            string json = JsonConvert.SerializeObject(data);
            //2) string을 byte[]로
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

            //4) 암호화된 byte[]을 바이너리 json파일로 쓰기.
            using (BinaryWriter writer = new BinaryWriter(new FileStream(txtPath[slotIndex], FileMode.Create)))
            {
                writer.Write(encrypted);
            }


            /*바이바이 BsonWriter~*/
            //using (BsonWriter writer = new BsonWriter(new FileStream(binaryPath[slotIndex], FileMode.Create)))
            //{
            //    JsonSerializer serializer = new JsonSerializer();
            //    serializer.Serialize(writer, data);
            //}
        }
        Debug.Log("저장완료");
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
