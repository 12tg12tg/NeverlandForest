//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using UnityEngine;

//public class CSVWriter
//{
//    public List<string> items = new List<string>();
//    public void ImportCSV()
//    {
//        items.Clear();
//        List<Dictionary<string, string>> data = CSVReader.Read("TestCSV");
//        for (var i = 0; i < data.Count; i++)
//        {
//            ItemData item = new ItemData();

//            item.ID = data[i]["ID"];
//            item.imageID = data[i]["ImageID"];
//            item.name = data[i]["Name"];
//            item.hp = int.Parse(data[i]["HP"]);
//            item.hpLeveling = int.Parse(data[i]["HPLeveling"]);
//            item.def = int.Parse(data[i]["Def"]);
//            item.defLeveling = int.Parse(data[i]["DefLeveling"]);
//            item.elementMastery = int.Parse(data[i]["ElementMastery"]);
//            item.elementMasteryLeveling = int.Parse(data[i]["ElementMasteryLeveling"]);
//            item.elementResistance = int.Parse(data[i]["ElementResistance"]);
//            item.elementResistanceLeveling = int.Parse(data[i]["ElementResistanceLeveling"]);
//            switch (data[i]["ItemLargeType"])
//            {
//                case "Weapon":
//                    item.itemType = ItemType.Weapon;
//                    break;
//                case "Armor":
//                    item.itemType = ItemType.Armor;
//                    break;
//                case "Artifect":
//                    item.itemType = ItemType.Artifect;
//                    break;
//            }
//            switch (data[i]["ItemSmallType"])
//            {
//                case "TwoHandSword":
//                    item.itemType |= ItemType.Two_Hand_Sword;
//                    break;
//                case "Bow":
//                    item.itemType |= ItemType.Bow;
//                    break;
//                case "Chest":
//                    item.itemType |= ItemType.Chest;
//                    break;
//                case "Pants":
//                    item.itemType |= ItemType.Pants;
//                    break;
//                case "Helmet":
//                    item.itemType |= ItemType.helmet;
//                    break;
//            }
//            items.Add(item);
//            Debug.Log((int)item.itemType);
//        }
//    }

//    public void ExportCSV(string filePath)
//    {
//        var rowData = new string[items.Count + 1][];
//        //string filePath = @"Assets\Resources\TestCSV2.csv";
//        File.Delete(filePath);
//        string[] rowDataTemp = new string[13];
//        rowDataTemp[0] = "ID";
//        rowDataTemp[1] = "ImageID";
//        rowDataTemp[2] = "Name";
//        rowDataTemp[3] = "HP";
//        rowDataTemp[4] = "HPLeveling";
//        rowDataTemp[5] = "Def";
//        rowDataTemp[6] = "DefLeveling";
//        rowDataTemp[7] = "ElementMastery";
//        rowDataTemp[8] = "ElementMasteryLeveling";
//        rowDataTemp[9] = "ElementResistance";
//        rowDataTemp[10] = "ElementResistanceLeveling";
//        rowDataTemp[11] = "ItemLargeType";
//        rowDataTemp[12] = "ItemSmallType";
//        rowData[0] = rowDataTemp;

//        StreamWriter outStream = new StreamWriter(filePath, true, Encoding.UTF8);
//        outStream.WriteLine(string.Join(",", rowData[0]));

//        for (int i = 0; i < items.Count; i++)
//        {
//            var temp = new string[12];
//            temp[0] = items[i].ID;
//            temp[1] = items[i].imageID;
//            temp[2] = items[i].name;
//            temp[3] = Convert.ToString(items[i].hp);
//            temp[4] = Convert.ToString(items[i].hpLeveling);
//            temp[5] = Convert.ToString(items[i].def);
//            temp[6] = Convert.ToString(items[i].defLeveling);
//            temp[7] = Convert.ToString(items[i].elementMastery);
//            temp[8] = Convert.ToString(items[i].elementMasteryLeveling);
//            temp[9] = Convert.ToString(items[i].elementResistance);
//            temp[10] = Convert.ToString(items[i].elementResistanceLeveling);
//            temp[11] = Convert.ToString(items[i].itemType).Replace(" ", "");        //type = Wepon, Two_Hand_Sword
//            rowData[i + 1] = temp;
//            outStream.WriteLine(string.Join(",", temp));
//        };
//        outStream.Close();
//    }
//}
