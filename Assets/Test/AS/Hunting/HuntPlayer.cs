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

//        var basicPercent = Random.Range(20, 31); // 20-30% 사이 부여
//        var staminaPercent = 15; // 스태미나에 따른 확률 부여(현재 스태미나 미구현 상태이므로 고정치로)

//        huntPercent += basicPercent + staminaPercent;

//        currentIndex = Vector2.right; // 인덱스 1,0 이라는 의미.. 최초 시작 왼쪽 가운데

//        for (int i = 0; i < tiles.Length; i++)
//        {
//            if (tiles[i].index.Equals(currentIndex)) // 비효율인듯..
//            {
//                transform.position = tiles[i].transform.position + new Vector3(0f, 1f, 0f);
//            }
//        }

//        HuntPercentagePrint();
//    }

//    public void Move(Vector2 index, Vector3 pos)
//    {
//        // 플레이어가 이동하면 사냥 할 확률 증가(한칸 앞으로 이동 시 10프로, 은폐물에 숨었을 때 10프로)
//        // 동물이 플레이어를 발견 할 확률 증가
//        bushHuntPercent = 0;

//        // 현재 위치에서 뒤로 이동, 2칸 앞으로 이동, 대각선 2칸 이동 막음
//        if (index.y <= currentIndex.y - 1 ||
//            index.y >= currentIndex.y + 2 ||
//            Mathf.Abs(index.x - currentIndex.x) > 1)
//            return;

//        // index의 y 값 비교를 통해서 앞으로 한칸 전진 했는지 판단 가능
//        if (index.y.Equals(currentIndex.y + 1))
//        {
//            HuntPercentageUp();
//            animal.EscapingPercentageUp(); // 동물이 도망칠 확률 업
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
//        // 부쉬일 때 확률업을 제외한 다른 요소가 있으면 추가 될 예정
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
//            Debug.Log("성공");
//            result.text = "Hunting Success";
//        }
//        else
//        {
//            Debug.Log("실패");
//            result.text = "Hunting Fail";
//        }
//        StartCoroutine(Utility.CoSceneChange("2ENO_RandomMap", 3f));
//    }

//    private void HuntPercentageUp() => huntPercent += 10;
//}
