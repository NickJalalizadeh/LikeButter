using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakeScreenshot : MonoBehaviour {

	public RawImage galleryButton;
	public Animator animator;
	public Transform wonderMoments;
	public GameObject cellContainer;

	private int captureCount = 0;
	private GalleryCell[] galleryCells;
	private Camera cam;
	private bool grab;

	void Start() {
		cam = GetComponent<Camera> ();
		galleryCells = cellContainer.GetComponentsInChildren<GalleryCell> ();
	}

	private void OnPostRender()
	{
		if (grab)
		{
			if (CaptureWM (25f)) {
				Texture2D screenshot = GetScreen ();
				galleryButton.texture = screenshot;
				galleryCells [captureCount].Fill (screenshot);
				captureCount = (captureCount + 1) % galleryCells.Length;
			}

			grab = false;
		}	
	}

	private Texture2D GetScreen() {
		//Create a new texture with the width and height of the camera view
		Texture2D tex = new Texture2D(Screen.height, Screen.height, TextureFormat.RGB24, false);
		int startX = (Screen.width - Screen.height) / 2;
		//Read the pixels in the Rect starting at 0,0 and ending at the screen's width and height
		tex.ReadPixels(new Rect(startX, 0, Screen.height, Screen.height), 0, 0, false);
		tex.Apply();
		return tex;
	}

	private bool InRange(Transform target, float minDistance, float minBounds = 0.3f, float maxBounds = 0.7f) {
		Vector3 targetCenter = target.GetComponent<Renderer> ().bounds.center;
		Vector3 screenPoint = cam.WorldToViewportPoint(targetCenter);
		
		if ( // Check if in view
			screenPoint.z > 0 && 
			screenPoint.x >= minBounds && screenPoint.x <= maxBounds && 
			screenPoint.y >= minBounds && screenPoint.y <= maxBounds
		) { 
			// Check if within range
			return screenPoint.z <= minDistance; 
		}
		return false;
	}

	private bool CaptureWM(float minThreshold) {
		foreach (Transform wm in wonderMoments) {
			if (InRange(wm, minThreshold)) {
				WonderMoment wmScript = wm.gameObject.GetComponent<WonderMoment> ();
				if (!wmScript.isCaptured ()) {
					// Set capture flag on WM
					wmScript.Capture ();
					animator.SetTrigger ("Captured");
					// Increase Life Force
					LifeForce.UpdateMeter(0.1f);
					return true;
				}
			}
		}
		return false;
	}

	public void Capture() {
		grab = true;
	}
}
