using UnityEngine;
using UnityEngine.UI;

public class MeshButton : Button
{
	public MeshRenderer targetMeshRenderer;

	private Vector3 origPos;
	private Vector3 axis = Vector3.up;
	private float distance = .0015f;

	protected override void Awake()
	{
		origPos = transform.localPosition;
		targetMeshRenderer = GetComponent<MeshRenderer>();
	}

	protected override void DoStateTransition(SelectionState state, bool instant)
	{
		base.DoStateTransition(state, instant);
		if (Application.isPlaying)
		{
			switch (state) {
				case SelectionState.Disabled:
					if (targetMeshRenderer) {
						//targetMeshRenderer.material.color = colors.disabledColor;
						targetMeshRenderer.enabled = false;

					}
					//transform.localScale = new Vector3(1, 1, .5f);

					//transform.localPosition = origPos;
					break;
				case SelectionState.Normal:
					if (targetMeshRenderer) {
						//targetMeshRenderer.material.color = colors.normalColor;
						targetMeshRenderer.enabled = false;
					}
					//transform.localScale = new Vector3(1, 1, 1);

					//transform.localPosition = origPos;
					break;
				case SelectionState.Highlighted:
					if (targetMeshRenderer) {
						//targetMeshRenderer.material.color = colors.highlightedColor;
						targetMeshRenderer.enabled = true;
					}
					//transform.localScale = new Vector3(.9f, .9f, .9f);

					//transform.localPosition = origPos;
					//transform.Translate(axis * distance * .5f);
					break;
				case SelectionState.Pressed:
					if (targetMeshRenderer) {
						//targetMeshRenderer.material.color = colors.pressedColor;
						targetMeshRenderer.enabled = true;
					}
					//transform.localScale = new Vector3(.8f, .8f, .8f);

					//transform.localPosition = origPos;
					//transform.Translate(axis * distance);
					break;
				case SelectionState.Selected:
					if (targetMeshRenderer) {
						//targetMeshRenderer.material.color = colors.highlightedColor;
						targetMeshRenderer.enabled = true;
					}
					break;
			}
		}
	}
}