using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class CSVWriter
{
    public List<string> items = new List<string>();
    public static void Writer(string path, DataTableBase dataTableBase)
    {
        var csvPath = $@"Assets/AS/Resources/{path}.csv";
        if (!File.Exists(csvPath))
        {
            Debug.Log("파일이 없습니다");
            return;
        }
        File.Delete(csvPath);
        using (StreamWriter outStream = new StreamWriter(csvPath, true, Encoding.UTF8))
        {
            string[][] keyData = new string[dataTableBase.GetSizeCount() + 1][];
            switch (dataTableBase)
            {
                case ConsumableTable:
                    var consum = dataTableBase as ConsumableTable;
                    keyData[0] = consum.tableTitle;
                    outStream.WriteLine(string.Join(",", keyData[0]));
                    foreach (var data in dataTableBase.data)
                    {
                        var elem = data.Value as ConsumableTableElem;
                        var temp = new string[consum.tableTitle.Length];
                        temp[0] = elem.id;
                        temp[1] = elem.iconID;
                        temp[2] = elem.prefabsID;
                        temp[3] = elem.name;
                        temp[4] = elem.description;
                        temp[5] = elem.cost.ToString();
                        temp[6] = elem.hp.ToString();
                        temp[7] = elem.mp.ToString();
                        temp[8] = elem.statStr.ToString();
                        temp[9] = elem.duration.ToString();
                        outStream.WriteLine(string.Join(",", temp));
                    }
                    outStream.Close();
                    break;
                case ArmorTable:
                    var armor = dataTableBase as ArmorTable;
                    keyData[0] = armor.tableTitle;
                    outStream.WriteLine(string.Join(",", keyData[0]));
                    foreach (var data in dataTableBase.data)
                    {
                        var elem = data.Value as ArmorTableElem;
                        var temp = new string[armor.tableTitle.Length];
                        temp[0] = elem.id;
                        temp[1] = elem.name;
                        temp[2] = elem.description;
                        temp[3] = elem.iconID;
                        temp[4] = elem.prefabsID;
                        temp[5] = elem.type;
                        temp[6] = elem.cost.ToString();
                        temp[7] = elem.defence.ToString();
                        temp[8] = elem.weight.ToString();
                        temp[9] = elem.stat_str.ToString();
                        temp[10] = elem.stat_con.ToString();
                        temp[11] = elem.stat_int.ToString();
                        temp[12] = elem.stat_luk.ToString();
                        temp[13] = elem.evade.ToString();
                        temp[14] = elem.block.ToString();
                        outStream.WriteLine(string.Join(",", temp));
                    }
                    outStream.Close();
                    break;
            }
        }
    }
}
