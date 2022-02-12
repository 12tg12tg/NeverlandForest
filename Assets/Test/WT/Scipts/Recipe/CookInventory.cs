using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CookMaterialTypes
{   None,
    Fire,
    Condiment,
    Material,
    Food
}

public class CookInventory : MonoBehaviour
{
    private CookMaterialTypes currentMaterial;
    public CookMaterialTypes CurrentMaterial
    {
        get { return currentMaterial; }
    }
    public UICookInventoryList listinfo;

    public void SetInventoryType(DataMaterial data)
    {
        switch (data.ItemTableElem.type)
        {
            case "NONE":
                currentMaterial = CookMaterialTypes.None;
                break;
            case "FIRE":
                currentMaterial = CookMaterialTypes.Fire;
                break;
            case "CONDIMENT":
                currentMaterial = CookMaterialTypes.Condiment;
                break;
            case "FOODINGREDIENT":
                currentMaterial = CookMaterialTypes.Material;
                break;
            case "FOOD":
                currentMaterial = CookMaterialTypes.Food;
                break;
            default:
                break;
        }

        
    }

    public void Start()
    {
    }
}
