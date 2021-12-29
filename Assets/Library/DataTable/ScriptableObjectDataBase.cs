using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScriptableObjectDataBase : ScriptableObject
{
    public List<SerializeDictionary<string, string>> sc = new List<SerializeDictionary<string, string>>();
}