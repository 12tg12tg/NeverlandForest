using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAniControl : MonoBehaviour
{
    public Animator aniControl;
    void Start()
    {
        aniControl = GetComponent<Animator>();
    }
}
