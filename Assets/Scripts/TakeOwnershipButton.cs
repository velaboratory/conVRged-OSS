using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VelNet;

public class TakeOwnershipButton : MonoBehaviour
{
    public NetworkObject netObject;

    public void TakeOwnership()
    {
        netObject.TakeOwnership();
    }
    
}
