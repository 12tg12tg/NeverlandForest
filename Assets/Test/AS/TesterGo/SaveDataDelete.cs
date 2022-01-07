using System.IO;
using UnityEngine;

public enum SaveDataName
{
    BackUp,
    TextWorldMapNodePath,
    TextWorldMapPlayerDataPath,
}

public class SaveDataDelete : MonoBehaviour
{
    [Header("삭제할 데이터 Enum")]
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