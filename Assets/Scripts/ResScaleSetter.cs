using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class ResScaleSetter : MonoBehaviour
{
	// Start is called before the first frame update
	private void OnEnable()
	{
		SceneManager.activeSceneChanged += OnSceneLoad;
	}

	private void OnDisable()
	{
		SceneManager.activeSceneChanged -= OnSceneLoad;
	}

	private void OnSceneLoad(Scene oldScene, Scene newScene)
	{
		string sceneName = newScene.name;

#if OCULUS_INTEGRATION
		// use manual ffr
		//OVRManager.fixedFoveatedRenderingLevel = sceneSettings.GetFFRLevel(sceneName);

		// use dynamic ffr
		OVRManager.fixedFoveatedRenderingLevel = OVRManager.FixedFoveatedRenderingLevel.HighTop;
		OVRManager.useDynamicFixedFoveatedRendering = true;
#endif

		// XRSettings.eyeTextureResolutionScale = GameManager.instance.SceneSettings.resolutionScale;
	}
}
