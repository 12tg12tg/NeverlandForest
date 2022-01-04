//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;
//using UnityEngine.SceneManagement;

//public class HuntPlayer : MonoBehaviour
//{
//    public HuntTilesMaker tileMaker;
//    public HuntTile[] tiles;

//    private Vector2 currentIndex;
//    private int huntPercent;
//    private int bushHuntPercent;
//    private int totalHuntPercent;

//    public Animal animal;
//    public Coroutine coMove;

//    public TMP_Text text;

//    public TMP_Text result;

//    private void Start()
//    {
//        var count = tileMaker.transform.childCount;
//        tiles = new HuntTile[count];
//        for (int i = 0; i < count; i++)
//        {
//            tiles[i] = tileMaker.transform.GetChild(i).GetComponent<HuntTile>();
//        }

//        var basicPercent = Random.Range(20, 31); // 20-30% ���� �ο�
//        var staminaPercent = 15; // ���¹̳��� ���� Ȯ�� �ο�(���� ���¹̳� �̱��� �����̹Ƿ� ����ġ��)

//        huntPercent += basicPercent + staminaPercent;

//        currentIndex = Vector2.right; // �ε��� 1,0 �̶�� �ǹ�.. ���� ���� ���� ���

//        for (int i = 0; i < tiles.Length; i++)
//        {
//            if (tiles[i].index.Equals(currentIndex)) // ��ȿ���ε�..
//            {
//                transform.position = tiles[i].transform.position + new Vector3(0f, 1f, 0f);
//            }
//        }

//        HuntPercentagePrint();
//    }

//    public void Move(Vector2 index, Vector3 pos)
//    {
//        // �÷��̾ �̵��ϸ� ��� �� Ȯ�� ����(��ĭ ������ �̵� �� 10����, ���󹰿� ������ �� 10����)
//        // ������ �÷��̾ �߰� �� Ȯ�� ����
//        bushHuntPercent = 0;

//        // ���� ��ġ���� �ڷ� �̵�, 2ĭ ������ �̵�, �밢�� 2ĭ �̵� ����
//        if (index.y <= currentIndex.y - 1 ||
//            index.y >= currentIndex.y + 2 ||
//            Mathf.Abs(index.x - currentIndex.x) > 1)
//            return;

//        // index�� y �� �񱳸� ���ؼ� ������ ��ĭ ���� �ߴ��� �Ǵ� ����
//        if (index.y.Equals(currentIndex.y + 1))
//        {
//            HuntPercentageUp();
//            animal.EscapingPercentageUp(); // ������ ����ĥ Ȯ�� ��
//        }
//        var newPos = pos + new Vector3(0f, 1f, 0f);

//        currentIndex = coMove == null ? index : currentIndex;
//        coMove ??= StartCoroutine(Utility.CoTranslate(transform, transform.position, newPos, 1f, () => coMove = null));

//        animal.Escaping();
//        HuntPercentagePrint();
//    }

//    private void HuntPercentagePrint()
//    {
//        totalHuntPercent = huntPercent + bushHuntPercent;
//        text.text = $"Hunting Percentage: {totalHuntPercent}%" + "\n" +
//                    $"Escape Percentage {animal.EscapePercent}%";
//    }

//    public void OnBush(Vector2 index)
//    {
//        // �ν��� �� Ȯ������ ������ �ٸ� ��Ұ� ������ �߰� �� ����
//        if (currentIndex.Equals(index))
//        {
//            bushHuntPercent = 10;
//        }
//        HuntPercentagePrint();
//    }

//    public void Hunting()
//    {
//        if (Random.Range(0f, 1f) < totalHuntPercent * 0.01f)
//        {
//            Debug.Log("����");
//            result.text = "Hunting Success";
//        }
//        else
//        {
//            Debug.Log("����");
//            result.text = "Hunting Fail";
//        }
//        StartCoroutine(Utility.CoSceneChange("2ENO_RandomMap", 3f));
//    }

//    private void HuntPercentageUp() => huntPercent += 10;
//}
