using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTableElem : DataTableElemBase
{
	public string iconId;
	public string profileId;
	public string prefabId;
	public string name;
	public string description;
	public float ad;
	public float ap;
	public float df;
	public float hp;
	public float mp;
	public int statStr;
	public int statDex;
	public int statInt;
	public int statLuk;
	private Sprite iconSprite;

	public Sprite IconSprite
	{
		get { return iconSprite; }
	}

	private Sprite profileSprite;

	public Sprite ProfileSprite
	{
		get { return profileSprite; }
	}
	public CharacterTableElem(Dictionary<string, string> data)
	{
		id = data["ID"];
		iconId = data["ICON_ID"];
		profileId = data["PROFILE_ID"];
		prefabId = data["PREFAB_ID"];
		name = data["NAME"];
		description = data["DESC"];
		ad = float.Parse(data["AD"]);
		ap = float.Parse(data["AP"]);
		df = float.Parse(data["DF"]);
		hp = float.Parse(data["HP"]);
		mp = float.Parse(data["MP"]);
		statStr = int.Parse(data["STAT_STR"]);
		statDex = int.Parse(data["STAT_DEX"]);
		statInt = int.Parse(data["STAT_INT"]);
		statLuk = int.Parse(data["STAT_LUK"]);

		iconSprite = Resources.Load<Sprite>($"Sprites/Characters/Icons/{iconId}");
		profileSprite = Resources.Load<Sprite>($"Sprites/Characters/Profiles/{profileId}");
	}
}



public class CharacterTable : DataTableBase
{
	public CharacterTable()
	{
		csvFilePath = @"Tables\CharacterDataTable";
	}
	public override void Load()
	{
		data.Clear();
		var list = CSVReader.Read(csvFilePath);
		foreach (var line in list)
		{
			var elem = new CharacterTableElem(line);
			data.Add(elem.id, elem);
		}
	}
}
