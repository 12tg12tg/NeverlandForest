using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTutorial : MonoBehaviour
{
    [SerializeField] private BattleManager bm;
    [SerializeField] private TileMaker tm;
    private BottomUIManager bottomUI;

    private void Start()
    {
        bottomUI = BottomUIManager.Instance;
    }

    public void StartDutorial()
    {
        StartCoroutine(CoBattleTutorial());
    }

    public void EndDutorial()
    {

    }

    IEnumerator CoBattleTutorial()
    {
        // 웨이브 생성
        bm.TutorialInit();

        // 웨이브 미리보기 버튼 클릭 유도 & 설명 띄우기
        yield return null;

        // 웨이브 미리보기 취소 클릭 유도 & 설명 띄우기

        yield return null;

        // 인벤토리 나무트랩 클릭 유도 & 설명 띄우기
        yield return null;

        // (특정) 타일선택 클릭
        yield return null;

        // 전투시작버튼 터치 & 설명
        yield return null;

        // 설명(사냥꾼 능력)
        yield return null;

        // 사냥꾼 스킬 사용 유도
        yield return null;

        // 몬스터 실드 설명
        yield return null;

        // 설명(약초학자 능력)
        yield return null;

        // 약초학자 스킬 사용 유도
        yield return null;

        // 랜턴 밝기 감소 설명
        yield return null;

        // 몬스터 행동(설명따로없음)


        // 랜턴 충전 유도
        yield return null;

        // 설명(약초학자 능력)
        yield return null;

        // 남은 전투 유저가 하기(설명문)


        yield return null; /*승리 ? 패배? 기다리기*/

        if(true )
        {
            //승리 - 종료
        }
        else
        {
            //패배 - 창띄우고 저기서부터 다시
        }

        EndDutorial();
    }
}
