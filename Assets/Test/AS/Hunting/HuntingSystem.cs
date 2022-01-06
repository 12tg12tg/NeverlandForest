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

        var basicPercent = Random.Range(20, 31); // 20-30% ���� �ο�
        var staminaPercent = 15; // ���¹̳��� ���� Ȯ�� �ο�(���� ���¹̳� �̱��� �����̹Ƿ� ����ġ��)

        huntPercent += basicPercent + staminaPercent;

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].index.Equals(playerUnit.CurrentIndex)) // ��ȿ���ε�..
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
        if (vals.Length != 2)
            return;

        if ((bool)vals[1])
        {
            bushHuntPercent = 10;
        }
        else
        {
            bushHuntPercent = 0;
        }

        HuntPercentagePrint();
    }

    public void Hunting()
    {
        if (Random.Range(0f, 1f) < totalHuntPercent * 0.01f)
        {
            Debug.Log("����");
            result.text = "Hunting Success";
        }
        else
        {
            Debug.Log("����");
            result.text = "Hunting Fail";
        }
        StartCoroutine(Utility.CoSceneChange("AS_RandomMap", 3f));
    }

    // ���ٹ����� ��� ����..
    //huntPercent = 
    //(bool) vals[0].Length != 1 ? huntPercent + 10 : huntPercent;

    private void HuntPercentageUp(object[] vals)
    {
        if (vals.Length != 2)
            return;
        if((bool)vals[0])
        {
            huntPercent += 10;
        }

        HuntPercentagePrint();
    }
}
