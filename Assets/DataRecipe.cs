using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataRecipe : DataItem
{
    public RecipeTableElem ItemTableElem
    {
        get
        {
            return itemTableElem as RecipeTableElem;
        }
    }
}
