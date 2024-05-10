using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]

public class CameraFollow : MonoBehaviour
{
    [SerializeField] public GameObject FollowTarget;
    [SerializeField] private float smoothTime = 0.25f;
    [SerializeField] private Vector3 offset = new Vector3 (0, 10, -10f);
    private Vector3 velocity = Vector3.zero;

    void FixedUpdate ()
    {
        FollowEntity(FollowTarget, smoothTime);
    }

    void FollowEntity(GameObject target, float smoothTime)
    {
        Vector3 targetPosition = target.transform.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
