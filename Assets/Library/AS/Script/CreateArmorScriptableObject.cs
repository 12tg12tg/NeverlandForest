using UnityEngine;

public class CreateArmorScriptableObject : ScriptableObject
{
    public SerializeDictionary<string, ArmorTableElem> dicArmorObj = new SerializeDictionary<string, ArmorTableElem>();
}