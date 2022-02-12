using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;
using PlayerSaveDataCurrentVersion = PlayerSaveData_1;
using OptionSaveDataCurrentVersion = OptionSaveData_0;
using RecipeSaveDataCurrentVersion = RecipeSaveData_0;
using DungeonSaveDataCurrentVersion = DungeonMapSaveData_0;
using WorldMapNodeSaveDataCurrentVersion = WorldMapData_0;
using WorldMapPlayerSaveDataCurrentVersion = WorldMapPlayerData_0;
using ConsumableSaveDataCurrentVersion = ConsumableSaveData_0;
using CraftSaveDataCurrentVersion = CraftSaveData_0;
using RandomEventSaveDataCurrentVersion = RandomEventSaveData_0;
using ExperienceSaveDataCurrentVersion = ItemExperienceSaveData_0;
using BattleSaveDataCurrentVersion = BattleSaveData_0;
using SceneSaveDataCurrentVersion = SceneSaveData_0;
using MemoSaveDataCurrentVersion = memoSaveData_0;
using ItemSaveDataCurrentVersion = ItemListSaveData_0;

//=========================================================================================
// SaveData ���� �߰��� �ؾ��� ��. + Save�� �ϳ� �߰��� �� ����
// 1. using ���� ������Ʈ.
// 2. Manager�� �ʵ庯�� �ڷ��� �ٲٰ�, Save / Load �Լ� ����.
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
        DungeonMap,
        WorldMapData,
        WorldMapPlayerData,
        ConsumableData,
        Craft,
        RandomEvent,
        ItemExperience,
        Battle,
        Scene,
        Memo,
        item,
        Count
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
            Debug.Log("�ε�Ϸ�");

            data = VersionUpdate(saveType, data);

            return data;
        }
        else
        {
            byte[] decrypted;
            //1) ��ȣȭ�� byte[]�� ��������.
            using (BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                decrypted = reader.ReadBytes((int)reader.BaseStream.Length);           
            }

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
            using (BinaryWriter writer = new BinaryWriter(new FileStream(tempPath, FileMode.Create)))
            {
                writer.Write(encrypted);
            }

            File.Replace(tempPath, path, backupPath);
        }
        Debug.Log("����Ϸ�");
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
            case SaveType.DungeonMap:
                switch (version)
                {
                    case 0:
                        return JsonConvert.DeserializeObject<DungeonSaveDataCurrentVersion>(json);
                    default:
                        return null;
                }
            case SaveType.WorldMapData:
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
            case SaveType.Craft:
                switch (version)
                {
                    case 0:
                        return JsonConvert.DeserializeObject<CraftSaveDataCurrentVersion>(json);
                    default:
                        return null;
                }
            case SaveType.ItemExperience:
                switch (version)
                {
                    case 0:
                        return JsonConvert.DeserializeObject<ExperienceSaveDataCurrentVersion>(json);
                    default:
                        return null;
                }
            case SaveType.RandomEvent:
                switch (version)
                {
                    case 0:
                        return JsonConvert.DeserializeObject<RandomEventSaveDataCurrentVersion>(json);
                    default:
                        return null;
                }
            case SaveType.Battle:
                switch (version)
                {
                    case 0:
                        return JsonConvert.DeserializeObject<BattleSaveDataCurrentVersion>(json);
                    default:
                        return null;
                }
            case SaveType.Scene:
                switch (version)
                {
                    case 0:
                        return JsonConvert.DeserializeObject<SceneSaveDataCurrentVersion>(json);
                    default:
                        return null;
                }

            case SaveType.Memo:
                switch (version)
                {
                    case 0:
                        return JsonConvert.DeserializeObject<MemoSaveDataCurrentVersion>(json);
                    default:
                        return null;
                }
            case SaveType.item:
                switch (version)
                {
                    case 0:
                        return JsonConvert.DeserializeObject<ItemSaveDataCurrentVersion>(json);
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
            case SaveType.DungeonMap:
                while (!(data is DungeonSaveDataCurrentVersion))
                {
                    data = data.VersionUp();
                }
                return data;
            case SaveType.WorldMapData:
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
            case SaveType.Craft:
                while (!(data is CraftSaveDataCurrentVersion))
                {
                    data = data.VersionUp();
                }
                return data;
            case SaveType.RandomEvent:
                while (!(data is RandomEventSaveDataCurrentVersion))
                {
                    data = data.VersionUp();
                }
                return data;
            case SaveType.ItemExperience:
                while (!(data is ExperienceSaveDataCurrentVersion))
                {
                    data = data.VersionUp();
                }
                return data;
            case SaveType.Battle:
                while (!(data is BattleSaveDataCurrentVersion))
                {
                    data = data.VersionUp();
                }
                return data;
            case SaveType.Scene:
                while (!(data is SceneSaveDataCurrentVersion))
                {
                    data = data.VersionUp();
                }
                return data;

            case SaveType.Memo:
                while (!(data is MemoSaveDataCurrentVersion))
                {
                    data = data.VersionUp();
                }
                return data;
            case SaveType.item:
                while (!(data is ItemSaveDataCurrentVersion))
                {
                    data = data.VersionUp();
                }
                return data;
            default:
                return null;
        }
    }
}