using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HuntTile : MonoBehaviour, IPointerClickHandler
{
    public Bush bush;
    public HuntPlayer huntPlayers;
    public Material[] materials;
    public MeshRenderer ren;
    public Vector2 index;

    [HideInInspector]
    public HuntingManager huntingManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!huntPlayers.IsTutorialClear)
        {
            if (huntPlayers.tutorialTile == null)
                return;
            if (huntPlayers.tutorialTile.index == index && huntingManager != null)
            {
                huntingManager.GetComponent<HuntTutorial>().TutorialStep++;
                Debug.Log(huntingManager.GetComponent<HuntTutorial>().TutorialStep);
                huntingManager = null;
            }
        }
        else if ((int)index.y != 6)
        {
            huntPlayers.Move(index, transform.position, bush != null);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && bush != null)
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
        if (other.CompareTag("Player") && bush != null)
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
