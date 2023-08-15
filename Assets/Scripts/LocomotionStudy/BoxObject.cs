using System;
using unityutilities.VRInteraction;
using VelNet;

namespace LocomotionStudy
{
    public class BoxObject : NetworkComponent
    {
        public int id;
        public VRGrabbable grabbable;
        public Action OnFound;


        public override void ReceiveBytes(byte[] message)
        {
            throw new NotImplementedException();
        }
    }
}
