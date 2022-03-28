using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubDriver : MonoBehaviour
{
    public virtual void SetContext(GameObject parentDriver)
    {
        gameObject.layer = parentDriver.layer;
        transform.parent = parentDriver.transform;
        transform.localRotation = Quaternion.identity;
    }
}
