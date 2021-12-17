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
        var csvPath = $@"Assets/Library/AS/Resources/{path}.csv";
        if (!File.Exists(csvPath))
        {
            Debug.Log("파일이 없습니다");
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
                    //var armor = dataTableBase as ArmorTable;
                    //keyData[0] = tableTitle;
                    //outStream.WriteLine(string.Join(",", keyData[0]));
                    //foreach (var data in dataTableBase.data)
                    //{
                    //    var elem = data.Value as ArmorTableElem;
                    //    var temp = new string[armor.tableTitle.Length];
                    //    temp[0] = elem.id;
                    //    temp[1] = elem.name;
                    //    temp[2] = elem.description;
                    //    temp[3] = elem.iconID;
                    //    temp[4] = elem.prefabsID;
                    //    temp[5] = elem.type;
                    //    temp[6] = elem.cost.ToString();
                    //    temp[7] = elem.defence.ToString();
                    //    temp[8] = elem.weight.ToString();
                    //    temp[9] = elem.stat_str.ToString();
                    //    temp[10] = elem.stat_con.ToString();
                    //    temp[11] = elem.stat_int.ToString();
                    //    temp[12] = elem.stat_luk.ToString();
                    //    temp[13] = elem.evade.ToString();
                    //    temp[14] = elem.block.ToString();
                    //    outStream.WriteLine(string.Join(",", temp));
                    //}
                    //outStream.Close();
                    break;
                case "Tables/WeaponDataTable":
                    break;
            }
        }
    }
}
