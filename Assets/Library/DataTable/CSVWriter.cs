using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class CSVWriter
{
    public List<string> items = new List<string>();
    public static void Writer(string path, string[] tableTitle, List<Dictionary<string, string>> data)
    {
        var csvPath = $@"Assets/Resources/{path}.csv";
        if (!File.Exists(csvPath))
        {
            Debug.Log($"{csvPath}에 파일이 없습니다");
            return;
        }
        
        using (StreamWriter outStream = new StreamWriter(csvPath, false, Encoding.UTF8))
        {
            string[][] keyData = new string[data.Count + 1][];
            switch (path)
            {
                case "Tables/CharacterDataTable":
                    break;
                case "Tables/CharacterLevelTable":
                    break;
                case "Tables/ConsumDataTable":
                    keyData[0] = tableTitle;
                    outStream.WriteLine(string.Join(",", keyData[0]));
                    foreach (var elem in data)
                    {
                        var temp = new string[tableTitle.Length];
                        temp[0] = elem["ID"];
                        temp[1] = elem["ICON_ID"];
                        temp[2] = elem["PREFAB_ID"];
                        temp[3] = elem["NAME"];
                        temp[4] = elem["DESC"];
                        temp[5] = elem["COST"];
                        temp[6] = elem["STAT_HP"];
                        temp[7] = elem["STAT_MP"];
                        temp[8] = elem["STAT_STR"];
                        temp[9] = elem["DURATION"];
                        outStream.WriteLine(string.Join(",", temp));
                    }
                    outStream.Close();
                    break;
                case "Tables/DefDataTable":
                    break;
                case "Tables/WeaponDataTable":
                    break;
            }
        }
    }
}
