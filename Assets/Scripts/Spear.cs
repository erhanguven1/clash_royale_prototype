using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    public Transform originPos;
    Transform target;

    public void AttackTarget(Transform _target)
    {
        target = _target;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            return;
        }

        transform.LookAt(target);
        transform.position += Vector3.forward;

        if (Vector3.Distance(target.position, transform.position) < .1f)
        {
            gameObject.SetActive(false);
            transform.position = originPos.position;
        }
    }
}
