using UnityEngine;
using UnityEngine.UI;

public class MenuButton : Button
{
	public MeshRenderer targetMeshRenderer;

	protected override void DoStateTransition(SelectionState state, bool instant)
	{
		base.DoStateTransition(state, instant);
		if (!Application.isPlaying) return;

		switch (state)
		{
			case SelectionState.Disabled:
				if (targetMeshRenderer)
					targetMeshRenderer.material.color = colors.disabledColor;
				transform.localScale = new Vector3(1, 1, .5f);
				break;
			case SelectionState.Normal:
				if (targetMeshRenderer)
					targetMeshRenderer.material.color = colors.normalColor;
				transform.localScale = new Vector3(1, 1, 1);
				break;
			case SelectionState.Highlighted:
				if (targetMeshRenderer)
					targetMeshRenderer.material.color = colors.highlightedColor;
				transform.localScale = new Vector3(.9f, .9f, .9f);
				break;
			case SelectionState.Pressed:
				if (targetMeshRenderer)
					targetMeshRenderer.material.color = colors.pressedColor;
				transform.localScale = new Vector3(.8f, .8f, .8f);
				break;
		}
	}
}