using System.IO;
using UnityEngine;

public enum SaveDataName
{
    BackUp,
    TextWorldMapDataPath,
    TextWorldMapPlayerDataPath,
    TextDungeonMapPath,
    TextRandomEventPath,
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
    public void AllSaveDataDelete()
    {
        var path = Application.persistentDataPath;
        var dir = new DirectoryInfo(path);
        FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            file.Attributes = FileAttributes.Normal;
        }
        Directory.Delete(path, true);
    }
    public void OpenFolder() => System.Diagnostics.Process.Start(Application.persistentDataPath);
}