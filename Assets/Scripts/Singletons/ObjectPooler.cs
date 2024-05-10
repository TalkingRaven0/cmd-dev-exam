using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler instance;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        instance = this;
    }

    private Dictionary<string, Queue<PooledObject>> objectPoolContainer = new Dictionary<string, Queue<PooledObject>>();

    public GameObject RequestObjectInstance(GameObject requestedObject, Vector3 position, GameObject parent=null)
    {
        if (requestedObject.GetComponent<PooledObject>() == null) {
            Debug.LogError("Requested Object is not a Pooled Object");
            return null;
        }

        PooledObject returnObject;
        Queue<PooledObject> objectPool;

        try
        {
            objectPool = objectPoolContainer[requestedObject.GetComponent<PooledObject>().GetType().ToString()];
        } catch
        {
            objectPool = new Queue<PooledObject>();
            objectPoolContainer.Add(requestedObject.GetComponent<PooledObject>().GetType().ToString(), objectPool);
        }

        try
        {
            returnObject = objectPool.Dequeue();

        } catch
        {
            returnObject = Instantiate(requestedObject).GetComponent<PooledObject>();
        }
        returnObject.Instantiate(position, parent);
        return returnObject.gameObject;
    }

    public void ReturnToPool(PooledObject pooledObject)
    {
        Queue<PooledObject> value;
        try
        {
            pooledObject.transform.SetParent(gameObject.transform, true);
        }
        catch
        {
            return;
        }

        if (objectPoolContainer.TryGetValue(pooledObject.GetType().ToString(), out value))
        {
            objectPoolContainer[pooledObject.GetType().ToString()].Enqueue(pooledObject);
        }
        else
        {
            value = new Queue<PooledObject>();
            value.Enqueue(pooledObject);
            objectPoolContainer.Add(pooledObject.GetType().ToString(), value);
        }
    }
}
