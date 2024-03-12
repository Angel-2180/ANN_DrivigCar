using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCamera : MonoBehaviour
{
    [SerializeField]
    float Speed = 10f;

    [SerializeField]
    float YDistance = 200f;

    Transform targetTr = null;
    public Transform TargetTr
    {
        set
        {
            targetTr = value;
            if (targetTr == null) return;
            Vector3 targetPos = targetTr.position;
            targetPos.y = YDistance;
            transform.position = targetPos;
        }
    }

    void LateUpdate ()
    {
        if (targetTr == null)
            return;
        Vector3 targetPos = targetTr.position;
        targetPos.y = YDistance;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * Speed);
	}
}
