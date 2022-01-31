using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReconfirmPanelManager : MonoBehaviour
{
    public GameObject battleReconfirm;
    public GameObject huntReconfirm;
    public GameObject gatheringInDungeonReconfirm;
    public GameObject bonFireReconfirm;
    public GameObject bagisFullReconfirm;
    public GameObject randomEventReconfirm;

    public DiaryManager diaryManager;

    public void Awake()
    {
        gameObject.SetActive(false);
        AllClose();
    }
    public void AllClose()
    {
        battleReconfirm.SetActive(false);
        huntReconfirm.SetActive(false);
        gatheringInDungeonReconfirm.SetActive(false);
        bonFireReconfirm.SetActive(false);
        bagisFullReconfirm.SetActive(false);
        randomEventReconfirm.SetActive(false);
        diaryManager.AllClose();
    }

    public void OpenBattleReconfirm()
    {
        AllClose();
        battleReconfirm.SetActive(true);
    }
    public void OpenHuntReconfirm()
    {
        AllClose();
        huntReconfirm.SetActive(true);
    }
    public void OpenGatheringInDungeonReconfirm()
    {
        AllClose();
        gatheringInDungeonReconfirm.SetActive(true);
    }
    public void OpenBonFireReconfirm()
    {
        AllClose();
        bonFireReconfirm.SetActive(true);
    }
    public void OpenBagReconfirm()
    {
        bagisFullReconfirm.SetActive(true);
    }
    public void OpenRandomEventfirm()
    {
        AllClose();
        randomEventReconfirm.SetActive(true);
    }
}
