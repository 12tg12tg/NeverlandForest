using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    public int id;
    public string nickname;

    public List<DataCharacter> characterList = new List<DataCharacter>();

    public List<DataCunsumable> consumableItemList =
        new List<DataCunsumable>();
    public List<DataWeapon> weaponItemList =
        new List<DataWeapon>();
    public List<DataMaterial> HaveMaterialList =
        new List<DataMaterial>();
    public List<DataRecipe> RecipeList =
      new List<DataRecipe>();
}
