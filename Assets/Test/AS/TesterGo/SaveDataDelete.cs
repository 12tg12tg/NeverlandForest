using System.IO;
using UnityEditor;
using UnityEngine;

public enum SaveDataName
{
    BackUp,
    TextWorldMapNodePath,
    TextWorldMapPlayerDataPath,
}

public class SaveDataDelete : MonoBehaviour
{
    [Header("������ ������ Enum")]
    public SaveDataName saveDataName;
    private readonly string local = "C:/Users/Administrator/AppData/LocalLow/TeamSummit/GEN2022/";


    public void DeleteFile()
    {
        var path = Path.Combine(local, saveDataName.ToString() + ".json");
        if (File.Exists(path))
            File.Delete(path);
    }
}