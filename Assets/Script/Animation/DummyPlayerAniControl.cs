using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayerAniControl : MonoBehaviour
{
    public Animator aniControl;
    void Start()
    {
        aniControl = GetComponent<Animator>();
    }
}
