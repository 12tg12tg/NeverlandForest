using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;
using PlayerSaveDataCurrentVersion = PlayerSaveData_1;
using OptionSaveDataCurrentVersion = OptionSaveData_0;
using RecipeSaveDataCurrentVersion = RecipeSaveData_0;
using TimeSaveDataCurrentVersion = TimeSaveData_0;
using DungeonSaveDataCurrentVersion = DungeonMapSaveData_0;
using WorldMapNodeSaveDataCurrentVersion = WorldMapNodeData_0;
using WorldMapPlayerSaveDataCurrentVersion = WorldMapPlayerData_0;
using ConsumableSaveDataCurrentVersion = ConsumableSaveData_0;

//=========================================================================================
// SaveData 버전 추가시 해야할 일. + Save가 하나 추가될 때 마다
// 1. using 구문 업데이트.
// 2. Manager의 필드변수 자료형 바꾸고, Save / Load 함수 수정.
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
    public enum SaveType
    {
        Player,
        Option,
        Recipe,
        Time,
        DungeonMap,
        WorldMapNode,
        WorldMapPlayerData,
        ConsumableData,
    }

    private static bool isInit;

    public static AesKeyVI aes = new AesKeyVI();

    public static void Init()
    {
        if(!isInit)
        {
            isInit = true;
            AESKeyLoad();
        }
    }
    public static string MakePath(Modes mode, SaveType type)
    {
        var modeStr = mode.ToString();
        var typeStr = type.ToString();
        return Path.Combine(Application.persistentDataPath, $"{modeStr}{typeStr}Path.json");
    }

    public static void AESKeyLoad()
    {
        var path = Path.Combine(Application.persistentDataPath, "AesKey.json");
        if (!File.Exists(path))
        {
            CreateAESKey();
            using (StreamWriter writer = new StreamWriter(new FileStream(path, FileMode.Create)))
            {
                string json = JsonConvert.SerializeObject(aes);
                writer.WriteLine(json);
                Debug.Log($"Key Save!\n{json}");
            }
        }
        else
        {
            using (StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open)))
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

    public static SaveDataBase Load(Modes mode, SaveType saveType)
    {
        SaveDataBase data = null;
        var path = MakePath(mode, saveType);
        if (!File.Exists(path))
        {
            return null;
        }
        if (mode == Modes.Text)
        {
            using (StreamReader reader = new StreamReader(new FileStream(path, FileMode.Open)))
            {       
                string json = reader.ReadToEnd();
                data = CheckVersionAndDeserialize(saveType, json);
            }
            Debug.Log("로드완료");

            data = VersionUpdate(saveType, data);

            return data;
        }
        else
        {
            byte[] decrypted;
            //1) 암호화된 byte[]을 꺼내오기.
            using (BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                decrypted = reader.ReadBytes((int)reader.BaseStream.Length);           
            }

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
            data = CheckVersionAndDeserialize(saveType, json);

            data = VersionUpdate(saveType, data);

            return data;
        }
    }

    public static void Save(SaveDataBase data, Modes mode, SaveType saveType)
    {
        var path = MakePath(mode, saveType);
        if (!File.Exists(path))
        {
            using (File.Create(path)) { }
        }
        var tempPath = Path.Combine(Application.persistentDataPath, "Temp.json");
        var backupPath = Path.Combine(Application.persistentDataPath, "BackUp.json");
        if (mode == Modes.Text)
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(tempPath, FileMode.Create)))
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                writer.WriteLine(json);
                //Debug.Log(json);
            }
            File.Replace(tempPath, path, backupPath);
        }
        else if (mode == Modes.Binary)
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
            using (BinaryWriter writer = new BinaryWriter(new FileStream(tempPath, FileMode.Create)))
            {
                writer.Write(encrypted);
            }

            File.Replace(tempPath, path, backupPath);
        }
        Debug.Log("저장완료");
    }

    public static SaveDataBase CheckVersionAndDeserialize(SaveType saveType, string json)
    {
        JObject rss = JObject.Parse(json);
        var version = (int)rss["Version"];
        switch (saveType)
        {
            case SaveType.Player:

                switch (version)
                {
                    case 0:
                        return JsonConvert.DeserializeObject<PlayerSaveData_0>(json);
                    case 1:
                        return JsonConvert.DeserializeObject<PlayerSaveData_1>(json);
                    default:
                        return null;
                }

            case SaveType.Option:

                switch (version)
                {
                    case 0:
                        return null;
                    case 1:
                        return null;
                    default:
                        return null;
                }

            case SaveType.Recipe:

                switch (version)
                {
                    case 0:
                       
                        return JsonConvert.DeserializeObject<RecipeSaveDataCurrentVersion>(json);
                    default:
                        return null;
                }

            case SaveType.Time:
                switch (version)
                {
                    case 0:

                        return JsonConvert.DeserializeObject<TimeSaveDataCurrentVersion>(json);
                    default:
                        return null;
                }
            case SaveType.DungeonMap:
                switch (version)
                {
                    case 0:
                        return JsonConvert.DeserializeObject<DungeonSaveDataCurrentVersion>(json);
                    default:
                        return null;
                }
            case SaveType.WorldMapNode:
                switch (version)
                {
                    case 0:
                        return JsonConvert.DeserializeObject<WorldMapNodeSaveDataCurrentVersion>(json);
                    default:
                        return null;
                }
            case SaveType.WorldMapPlayerData:
                switch (version)
                {
                    case 0:
                        return JsonConvert.DeserializeObject<WorldMapPlayerSaveDataCurrentVersion>(json);
                    default:
                        return null;
                }
            case SaveType.ConsumableData:
                switch (version)
                {
                    case 0:
                        return JsonConvert.DeserializeObject<ConsumableSaveDataCurrentVersion>(json);
                    default:
                        return null;
                }
            default:
                return null;
        }

    }

    public static SaveDataBase VersionUpdate(SaveType saveType, SaveDataBase data)
    {
        switch (saveType)
        {
            case SaveType.Player:
                while (!(data is PlayerSaveDataCurrentVersion))
                {
                    data = data.VersionUp();
                }
                return data;

            case SaveType.Option:
                while (!(data is OptionSaveDataCurrentVersion))
                {
                    data = data.VersionUp();
                }
                return data;

            case SaveType.Recipe:
                while (!(data is RecipeSaveDataCurrentVersion))
                {
                    data = data.VersionUp();
                }
                return data;

            case SaveType.Time:
                while (!(data is TimeSaveDataCurrentVersion))
                {
                    data = data.VersionUp();
                }
                return data;
            case SaveType.DungeonMap:
                while (!(data is DungeonSaveDataCurrentVersion))
                {
                    data = data.VersionUp();
                }
                return data;
            case SaveType.WorldMapNode:
                while (!(data is WorldMapNodeSaveDataCurrentVersion))
                {
                    data = data.VersionUp();
                }
                return data;
            case SaveType.ConsumableData:
                while (!(data is ConsumableSaveDataCurrentVersion))
                {
                    data = data.VersionUp();
                }
                return data;
            case SaveType.WorldMapPlayerData:
                while (!(data is WorldMapPlayerSaveDataCurrentVersion))
                {
                    data = data.VersionUp();
                }
                return data;
            default:
                return null;
        }
    }
}