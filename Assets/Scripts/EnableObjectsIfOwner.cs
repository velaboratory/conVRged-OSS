using UnityEngine;
using VelNet;

public class EnableObjectsIfOwner : MonoBehaviour
{
    public GameObject[] objects;

    public NetworkObject netObject;
    public bool enableIfOwner = true;

    private void Update()
    {
        if (objects == null) return;
        if (netObject == null) return;
        
        bool active = netObject.IsMine == enableIfOwner;

        foreach (GameObject o in objects)
        {
            o.SetActive(active);
        }
    }
}
