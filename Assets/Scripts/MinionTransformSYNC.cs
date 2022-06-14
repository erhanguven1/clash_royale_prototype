using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MinionTransformSYNC : NetworkBehaviour
{
    public Vector3 networkPosition;
    public Vector3 networkRotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            UpdateTransformOnClient(transform.position, transform.eulerAngles);
        }

        if (isClient)
        {
            transform.position = networkPosition;
            transform.eulerAngles = networkRotation;
        }
    }

    [ClientRpc]
    void UpdateTransformOnClient(Vector3 _pos, Vector3 _rot)
    {
        networkPosition = _pos;
        networkRotation = _rot;
    }
}
