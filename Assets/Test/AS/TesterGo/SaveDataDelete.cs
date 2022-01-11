using System.IO;
using UnityEngine;

public enum SaveDataName
{
    BackUp,
    TextWorldMapNodePath,
    TextWorldMapPlayerDataPath,
    TextDungeonMapPath,
}

public class SaveDataDelete : MonoBehaviour
{
    [Header("������ ������ Enum")]
    public SaveDataName[] saveDataName;

    public void DeleteFile()
    {
        for (int i = 0; i < saveDataName.Length; i++)
        {
            var path = Path.Combine(Application.persistentDataPath, saveDataName[i].ToString() + ".json");
            if(File.Exists(path))
                File.Delete(path);
        }
    }
}