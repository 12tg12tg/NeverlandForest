using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReconfirmPanelManager : MonoBehaviour
{
    [Header("Popups")]
    public GameObject rewardNotEmptyPopup;
    public GameObject inventoryFullPopup;

    [Header("ÆÇ³Úµé")]
    public GameObject battleReconfirm;
    public GameObject huntReconfirm;
    public GameObject gatheringInDungeonReconfirm;
    public GameObject bonFireReconfirm;
    public GameObject bagisFullReconfirm;
    public GameObject randomEventReconfirm;
 

    //public void Start()
    //{
    //    gameObject.SetActive(false);
    //    AllClose();
    //}
    public void AllClose()
    {
        battleReconfirm.SetActive(false);
        huntReconfirm.SetActive(false);
        gatheringInDungeonReconfirm.SetActive(false);
        bonFireReconfirm.SetActive(false);
        bagisFullReconfirm.SetActive(false);
        randomEventReconfirm.SetActive(false);
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
    public void OpenBonFireReconfirm()
    {
        AllClose();
        bonFireReconfirm.SetActive(true);
    }
    public void OpenBagReconfirm()
    {
        AllClose();
        bagisFullReconfirm.SetActive(true);
    }
    public void OpenBagReconfirmInGathering()
    {
        AllClose();
        gatheringInDungeonReconfirm.SetActive(true);
    }
    public void OpenRandomEventfirm()
    {
        gameObject.SetActive(true);
        AllClose();
        randomEventReconfirm.SetActive(true);
    }
}
