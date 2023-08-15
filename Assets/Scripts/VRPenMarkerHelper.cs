using UnityEngine;
using unityutilities.VRInteraction;

public class VRPenMarkerHelper : MonoBehaviour {
    public VRGrabbable grabbable;
    public SharedMarker marker;
    
    private void Start() {
        grabbable.Grabbed += marker.takeOwnership;
        grabbable.Released += marker.relinquishOwnership;
    }
}
