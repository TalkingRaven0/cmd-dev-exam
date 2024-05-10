using UnityEngine;

public abstract class PooledObject : MonoBehaviour
{
    public virtual void PoolDestroy()
    {
        gameObject.SetActive(false);
        ObjectPooler.instance.ReturnToPool(this);
    }

    public virtual GameObject Instantiate(Vector3 position, GameObject parent=null)
    {
        transform.position = position;
        gameObject.SetActive(true);
        transform.SetParent(parent.transform);
        return gameObject;
    }
}
