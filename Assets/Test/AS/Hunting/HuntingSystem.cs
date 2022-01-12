using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum HuntingEvent
{
    PlayerMove,
    AnimalEscape,
    Hunting,
}
public class HuntingSystem : MonoBehaviour
{
    public PlayerHuntingUnit playerUnit;
    public HuntTilesMaker tileMaker;
    public Image getItemImage;
    private HuntTile[] tiles;

    private int huntPercent;
    private int bushHuntPercent;
    private int totalHuntPercent;
    private bool isHunted = false;

    public Animal animal;
    public GameObject result;
    public WorldMap worldMap;
    private void Start()
    {
        EventBus<HuntingEvent>.Subscribe(HuntingEvent.PlayerMove, OnBush);
        EventBus<HuntingEvent>.Subscribe(HuntingEvent.PlayerMove, HuntPercentageUp);
        EventBus<HuntingEvent>.Subscribe(HuntingEvent.Hunting, Hunting);

        var count = tileMaker.transform.childCount;
        tiles = new HuntTile[count];
        for (int i = 0; i < count; i++)
        {
            tiles[i] = tileMaker.transform.GetChild(i).GetComponent<HuntTile>();
        }

        var basicPercent = Random.Range(20, 31); // 20-30% 사이 부여
        var staminaPercent = 15; // 스태미나에 따른 확률 부여(현재 스태미나 미구현 상태이므로 고정치로)

        huntPercent += basicPercent + staminaPercent;

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].index.Equals(playerUnit.CurrentIndex))
            {
                playerUnit.transform.position = tiles[i].transform.position;
            }
        }
        if (worldMap != null)
        {
            worldMap.InitWorldMiniMap(); 
        }
    }
    private void OnDestroy()
    {
        EventBus<HuntingEvent>.Unsubscribe(HuntingEvent.PlayerMove, OnBush);
        EventBus<HuntingEvent>.Unsubscribe(HuntingEvent.PlayerMove, HuntPercentageUp);
        EventBus<HuntingEvent>.Unsubscribe(HuntingEvent.Hunting, Hunting);
    }

    private void OnBush(object[] vals)
    {
        bushHuntPercent = (bool)vals[1] && vals.Length.Equals(2) ? 5 : 0;
    }

    private void HuntPercentageUp(object[] vals)
    {
        huntPercent = (bool)vals[0] && vals.Length.Equals(2) ? huntPercent + 10 : huntPercent;
    }
    private void GetItem()
    {
        var newItem = new DataAllItem();
        var tempItemNum = newItem.itemId = 5;
        newItem.LimitCount = 3;
        newItem.OwnCount = Random.Range(1, 5);
        var stringId = $"{tempItemNum}";
        var item = DataTableManager.GetTable<AllItemDataTable>().GetData<AllItemTableElem>(stringId);
        newItem.itemTableElem = item;
        getItemImage.sprite = item.IconSprite;

        Vars.UserData.AddItemData(newItem);
        //if (!Vars.UserData.HaveAllItemList2.ContainsKey(item.name))
        //{
        //    Vars.UserData.HaveAllItemList2.Add(item.name, newItem);
        //}
        //else
        //{
        //    Vars.UserData.HaveAllItemList2[newItem.ItemTableElem.name].OwnCount += newItem.OwnCount;
        //}
    }

    public void Shooting()
    {
        totalHuntPercent = huntPercent + bushHuntPercent;
        var rnd = Random.Range(0f, 1f);
        var succeeded = rnd < totalHuntPercent * 0.01f;
        var pos = animal.transform.position;
        pos.y = succeeded ? pos.y : pos.y * 5f;
        isHunted = succeeded;
        Debug.Log($"내 확률:{totalHuntPercent * 0.01f} > 랜덤 확률:{rnd}  ////  타겟 위치:{pos}");
        playerUnit.ShootArrow(pos);
    }
    public void LookOnTarget() => playerUnit.ShootAnimation(animal.transform.position);
    private void Hunting(object[] vals) // 성공 실패 던저주기
    {
        var textTMP = result.GetComponentInChildren<TMP_Text>();
        if (isHunted)
        {
            animal.AnimalDead();
            textTMP.text = "Hunting Success";
            GetItem();
        }
        else
        {
            animal.AnimalRunAway();
            textTMP.text = "Hunting Fail";
        }
        animal.AnimalMove(isHunted, () => {
            result.SetActive(true);
            StartCoroutine(Utility.CoSceneChange("AS_RandomMap", 3f));
        });
    }
}
