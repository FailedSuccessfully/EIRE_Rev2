using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class HurtBox : MonoBehaviour
{
    public float force;
    BoxCollider col;
    int targetLayer;
    void Awake()
    {
        col = GetComponent<BoxCollider>();
        col.isTrigger = true;
        col.enabled = false;

    }
    // Start is called before the first frame update
    void Start()
    {
        //targetLayer = GetComponentInParent<CharacterDriver>().Target.gameObject.layer;
    }
    public void SetLayer(int layer)
    {
        targetLayer = layer;
    }

    public void RecieveOn() => col.enabled = true;
    public void RecieveOff() => col.enabled = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == targetLayer && other.gameObject.TryGetComponent<CharacterDriver>(out CharacterDriver enemy))
        {
            Vector3 point = col.ClosestPoint(other.transform.position);
            Vector3 direction = (other.transform.position - transform.position).normalized;
            other.attachedRigidbody.AddForceAtPosition(direction * force, point, ForceMode.Impulse);

        }
    }
}
