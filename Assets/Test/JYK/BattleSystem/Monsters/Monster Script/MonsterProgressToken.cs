using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterProgressToken : MonoBehaviour
{
    public Image image;
    public void TokenOff()
    {
        image.enabled = false;
    }
    public void TokenOn()
    {
        image.enabled = true;
    }
}
