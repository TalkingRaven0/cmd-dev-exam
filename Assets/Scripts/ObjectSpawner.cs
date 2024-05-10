using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ObjectSpawner : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject PoolableObject;
    [SerializeField] private GameObject spawnTarget;

    public void OnInteract()
    {
        ObjectPooler.instance.RequestObjectInstance(PoolableObject, spawnTarget.transform.position, gameObject);
    }
}
