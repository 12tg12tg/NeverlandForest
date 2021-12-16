using UnityEngine;

public class CreateConsumScriptableObject : ScriptableObject
{
    public SerializeDictionary<string, ConsumableTableElem> dicConsumObj = new SerializeDictionary<string, ConsumableTableElem>();
}