using Newtonsoft.Json.Bson;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using SaveDataCurrentVersion = SaveData1;

public class SaveLoadTest : MonoBehaviour
{
    public int currentVersion;
    public int num;
    public string str;
    public bool boolean;
    public List<int> list;

    public Text jsonText;

    public Text verUI;
    public Text intUI;
    public Text stringUI;
    public Toggle boolUI;
    public Color color = Color.white;

    public Transform listParent;
    public GameObject listElementPrefab;
    public List<ListElement> listUIs;

    //public Dropdown dropdown;
    public int curSaveIndex;

    private void Start()
    {
        SaveLoadSystem.Init();
    }

    public void SaveTest()
    {
        Debug.Log("��������");
        var data = new SaveDataCurrentVersion()
        {
            Version = currentVersion,
            intData = num,
            stringData = str,
            boolData = true,
            listData = list,
            color = color
        };
        SaveLoadSystem.Save(data, curSaveIndex);
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        jsonText.text = json;
    }

    public void LoadTest()
    {
        Debug.Log("�ε�����");
        SaveDataBase data = SaveLoadSystem.Load(curSaveIndex);

        SaveDataCurrentVersion curVerData = (SaveDataCurrentVersion)data;

        //if (currentVersion == 0)    //������ ������ ������Ʈ�Ǹ�, �̷� if������ �ʿ����. ����, ���� �Դٰ����ϱ����� �׽�Ʈ������ ����� ��.
        //{                                                               //�ֽ� ������ �����͸� �޴� ������ �ֵ��� �����ϸ� ��.
        //    SaveData testVer = (SaveData)data;
        //    currentVersion = curVerData.Version;
        //    num = testVer.intData;
        //    str = testVer.stringData;
        //    boolean = testVer.boolData;
        //    list = testVer.listData;
        //}
        //else if (currentVersion == 1)
        //{
        //    SaveData1 testVer = (SaveData1)data;
        //    currentVersion = testVer.Version;
        //    num = testVer.intData;
        //    str = testVer.stringData;
        //    boolean = testVer.boolData;
        //    list = testVer.listData;
        //    color = testVer.color;
        //}

        //������ �ݿ� ����
        {
            currentVersion = curVerData.Version;
            num = curVerData.intData;
            str = curVerData.stringData;
            boolean = curVerData.boolData;
            list = curVerData.listData;
            color = curVerData.color;
        }
    }

    private void Update()
    {
        verUI.text = currentVersion.ToString();
        intUI.text = num.ToString();
        stringUI.text = str;
        stringUI.color = color;
        boolUI.isOn = boolean;

        if (list.Count != listUIs.Count)
        {
            var childrens = listParent.GetComponentsInChildren<ListElement>();
            foreach (var item in childrens)
            {
                Destroy(item.gameObject);
            }

            listUIs.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                var go = Instantiate(listElementPrefab, listParent);
                listUIs.Add(go.GetComponent<ListElement>());
            }
        }

        for (int i = 0; i < listUIs.Count; i++)
        {
            listUIs[i].index.text = i.ToString();
            listUIs[i].number.text = list[i].ToString();
        }
    }

    public void SlotChange(int index) { curSaveIndex =index; }
    public void intUp() { num++; }
    public void intDown() { num--; }
    public void stringUpdate(string str) { this.str = str; }
    public void boolUpdate(bool boolean) { this.boolean = boolean; }
    public void SetSaveType(bool isBinary)
    {
        if (isBinary)
            SaveLoadSystem.CurrentMode = SaveLoadSystem.Modes.Binary;
        else
            SaveLoadSystem.CurrentMode = SaveLoadSystem.Modes.Text;
    }
}
