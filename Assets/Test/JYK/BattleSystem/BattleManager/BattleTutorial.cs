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
        // ���̺� ����
        bm.TutorialInit();

        // ���̺� �̸����� ��ư Ŭ�� ���� & ���� ����
        yield return null;

        // ���̺� �̸����� ��� Ŭ�� ���� & ���� ����

        yield return null;

        // �κ��丮 ����Ʈ�� Ŭ�� ���� & ���� ����
        yield return null;

        // (Ư��) Ÿ�ϼ��� Ŭ��
        yield return null;

        // �������۹�ư ��ġ & ����
        yield return null;

        // ����(��ɲ� �ɷ�)
        yield return null;

        // ��ɲ� ��ų ��� ����
        yield return null;

        // ���� �ǵ� ����
        yield return null;

        // ����(�������� �ɷ�)
        yield return null;

        // �������� ��ų ��� ����
        yield return null;

        // ���� ��� ���� ����
        yield return null;

        // ���� �ൿ(������ξ���)


        // ���� ���� ����
        yield return null;

        // ����(�������� �ɷ�)
        yield return null;

        // ���� ���� ������ �ϱ�(����)


        yield return null; /*�¸� ? �й�? ��ٸ���*/

        if(true )
        {
            //�¸� - ����
        }
        else
        {
            //�й� - â���� ���⼭���� �ٽ�
        }

        EndDutorial();
    }
}
