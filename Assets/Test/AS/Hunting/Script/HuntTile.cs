using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HuntTile : MonoBehaviour, IPointerClickHandler
{
    public Bush bush;
    public HuntingManager huntingManager;
    public Material[] materials;
    public MeshRenderer ren;
    public Vector2 index;

    public void OnPointerClick(PointerEventData eventData)
    {
        var players = huntingManager.huntPlayers;
        if (!players.IsTutorialClear)
        {
            if (players.tutorialTile == null)
                return;
            var tuto = huntingManager.GetComponent<HuntTutorial>();
            if (players.tutorialTile.index == index && tuto.TutorialStep == 1)
            {
                tuto.TutorialStep++;
                Debug.Log(huntingManager.GetComponent<HuntTutorial>().TutorialStep);
            }
        }
        else if ((int)index.y != 6 && !huntingManager.animal.isRun)
        {
            players.Move(index, transform.position, bush.gameObject.activeSelf);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && bush.gameObject.activeSelf)
        {
            var trans = bush.transform;
            var count = trans.childCount;
            for (int i = 0; i < count; i++)
            {
                trans.GetChild(i).GetComponent<MeshRenderer>().material = materials[1];
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && bush.gameObject.activeSelf)
        {
            var trans = bush.transform;
            var count = trans.childCount;
            for (int i = 0; i < count; i++)
            {
                trans.GetChild(i).GetComponent<MeshRenderer>().material = materials[0];
            }
        }
    }

}
