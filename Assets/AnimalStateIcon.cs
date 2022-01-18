using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalStateIcon : MonoBehaviour
{
    public GameObject statePrefab;
    public Transform parent;

    private MeshRenderer icon;
    private Color colorOrange;

    private void Start()
    {
        var state = Instantiate(statePrefab);
        state.transform.SetParent(parent);
        icon = state.GetComponent<MeshRenderer>();
        icon.transform.position = parent.position + new Vector3(0f, 2f, 0f);
        icon.material.color = Color.green;

        ColorUtility.TryParseHtmlString("#FF6E00", out colorOrange);
    }

    public void IconColor(int escapePercent)
    {
        icon.material.color =
            escapePercent < 15 ? Color.green :
            escapePercent < 35 ? Color.yellow :
            escapePercent < 55 ? colorOrange : Color.red;
    }
}
