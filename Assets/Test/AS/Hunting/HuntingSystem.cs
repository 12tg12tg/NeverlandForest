using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum HuntingEvent
{
    PlayerMove,
    AnimalEscape,
}
public class HuntingSystem : MonoBehaviour
{
    public PlayerHuntingUnit playerUnit;
    public HuntTilesMaker tileMaker;
    private HuntTile[] tiles;

    private int huntPercent;
    private int bushHuntPercent;
    private int totalHuntPercent;

    public Animal animal;
    public TMP_Text text;

    public TMP_Text result;

    private void Start()
    {
        EventBus<HuntingEvent>.Subscribe(HuntingEvent.PlayerMove, OnBush);
        EventBus<HuntingEvent>.Subscribe(HuntingEvent.PlayerMove, HuntPercentageUp);

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
            if (tiles[i].index.Equals(playerUnit.CurrentIndex)) // 비효율인듯..
            {
                playerUnit.gameObject.transform.position = tiles[i].transform.position/* + new Vector3(0f, 1f, 0f)*/;
            }
        }
        HuntPercentagePrint();
    }
    private void OnDestroy()
    {
        EventBus<HuntingEvent>.Unsubscribe(HuntingEvent.PlayerMove, OnBush);
        EventBus<HuntingEvent>.Unsubscribe(HuntingEvent.PlayerMove, HuntPercentageUp);
    }


    private void HuntPercentagePrint()
    {
        totalHuntPercent = huntPercent + bushHuntPercent;
        text.text = $"Hunting Percentage: {totalHuntPercent}%" + "\n" +
                    $"Escape Percentage {animal.EscapePercent}%";
    }

    private void OnBush(object[] vals)
    {
        bushHuntPercent = (bool)vals[1] && vals.Length.Equals(2) ? 5 : 0;
        HuntPercentagePrint();
    }

    public void Hunting()
    {
        if (Random.Range(0f, 1f) < totalHuntPercent * 0.01f)
        {
            Debug.Log("성공");
            result.text = "Hunting Success";
        }
        else
        {
            Debug.Log("실패");
            result.text = "Hunting Fail";
        }
        StartCoroutine(Utility.CoSceneChange("AS_RandomMap", 3f));
    }

    private void HuntPercentageUp(object[] vals)
    {
        huntPercent = (bool)vals[0] && vals.Length.Equals(2) ? huntPercent + 10 : huntPercent;
        HuntPercentagePrint();
    }
}
