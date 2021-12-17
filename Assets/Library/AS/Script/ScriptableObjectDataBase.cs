using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectDataBase : ScriptableObject
{
    public List<SerializeDictionary<string, string>> sc = new List<SerializeDictionary<string, string>>();
}