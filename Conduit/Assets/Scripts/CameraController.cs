using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public Camera camObject;
	public float zoomMin = 5f;
	public float zoomMax = 30f;
	public float zoomSensitivity = 1f;
	
	public Transform followThis;
	public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

	private bool isZoomedOut = true;
	private Coroutine routine;

	void Awake(){
		World.instance.cameraControl = this;
	}

	void Start(){
		//Clamp to min/max
		InstantZoom(0);
		//Zoom Toggle, for effect
		ZoomToggle();
	}
	
	void Update(){
		Follow();
		ProcessInput();
	}
	
	void Follow(){
		if(!followThis) return;
		Vector3 targetPosition = followThis.position;//.TransformPoint(new Vector3(0, 5, -10));
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
	}
	
	void ProcessInput(){
//		float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
//		if(mouseWheel != 0){
//			Zoom(mouseWheel);
//		}
		if(Input.GetButtonDown("Zoom")){
			ZoomToggle();
		}
	}
	
	void InstantZoom(float input){
		float zoomHeight = camObject.transform.localPosition.y - (input * zoomSensitivity);
		zoomHeight = Mathf.Clamp(zoomHeight, zoomMin, zoomMax);
		camObject.transform.localPosition = new Vector3(0, zoomHeight, 0);
	}
	
	void ZoomToggle(){
		if(routine != null) StopCoroutine(routine);
		routine = StartCoroutine(ZoomTo(isZoomedOut ? zoomMin : zoomMax));
		isZoomedOut = !isZoomedOut;
	}

	IEnumerator ZoomTo(float height, float duration = 0.5f){
		Vector3 zoomFrom = camObject.transform.localPosition;
		Vector3 zoomTo = new Vector3(0, Mathf.Clamp(height, zoomMin, zoomMax), 0);
		for(float t = 0; t <= 1f; t += Time.deltaTime/duration){
			camObject.transform.localPosition = Vector3.Lerp(zoomFrom, zoomTo, t);
			yield return null;
		}
		camObject.transform.localPosition = zoomTo;
	}
}
