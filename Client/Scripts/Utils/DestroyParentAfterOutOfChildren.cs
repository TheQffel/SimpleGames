using UnityEngine;

public class DestroyParentAfterOutOfChildren : MonoBehaviour
{
    void Update()
    {
        if(transform.childCount == 0)
        {
            Destroy(this);
        }
    }
}
