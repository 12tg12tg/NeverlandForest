using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBlock : MonoBehaviour
{
    public Collider floorBlock;
    
    public void SetOriginal(Transform original)
    {
        transform.position = original.position;
        transform.rotation = original.rotation;
    }
}
