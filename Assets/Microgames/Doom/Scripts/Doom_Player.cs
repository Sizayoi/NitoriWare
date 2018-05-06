using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doom_Player : MonoBehaviour
{

    public static int hp = 100;

    void Update()
    {
        transform.Rotate(Vector3.up, Input.GetAxis("Mouse X"));
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.forward, out hit, 100f, 1 << LayerMask.NameToLayer("MicrogameLayer1")))
                hit.collider.GetComponent<Doom_Enemy>().DamageSelf();
        }
    }
}
