using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreateScriptableObject : ScriptableObject
{
    public SerializeDictionary<string, DataTableElemBase> dicScriptableObj = new SerializeDictionary<string, DataTableElemBase>();
    public SerializeDictionary<string, int> dic = new SerializeDictionary<string, int>();
    public List<TestList> testList = new List<TestList>();
    public int count;
    public string id;
    public int hp;
    public int mp;
}