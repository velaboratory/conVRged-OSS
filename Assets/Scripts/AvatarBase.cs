using UnityEngine;

namespace conVRged
{
	public interface AvatarBase
	{
		public Transform Head { get; }
		public Transform LeftHand { get; }
		public Transform RightHand { get; }
	}
}