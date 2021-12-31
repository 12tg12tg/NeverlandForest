using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HuntTile : MonoBehaviour, IPointerClickHandler
{
    public Bush cloak;
    public GameObject blue;
    public Material[] materials;
    public HuntPlayer player;
    public MeshRenderer ren;
    public Vector2 index;
    private bool isBlue = true;

    public void OnPointerClick(PointerEventData eventData)
    {
        //player.Move(index, transform.position);
        if (isBlue)
        {
            Blue();
            isBlue = false;
        }
        else
        {
            DestroyBlue();
            isBlue = true;
        }
        //if (cloak != null)
        //    player.OnBush(index);

        //ren.enabled = true;
    }

    public void Blue()
    {
        var mf = GetComponent<MeshFilter>();
        var mc = GetComponent<MeshCollider>();
        blue.GetComponent<MeshCollider>().sharedMesh = mc.sharedMesh;
        blue.GetComponent<MeshFilter>().mesh = mf.mesh;
        blue.transform.position = Vector3.zero;
        Instantiate(blue, transform);
    }

    public void DestroyBlue()
    {
        Destroy(transform.Find("Blue(Clone)").gameObject);
    }
}
