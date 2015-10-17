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
	
	void Start(){
		//Clamp to min/max
		Zoom(0);
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
		float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
		if(mouseWheel != 0){
			Zoom(mouseWheel);
		}
	}
	
	void Zoom(float input){
		float zoomHeight = camObject.transform.localPosition.y - (input * zoomSensitivity);
		zoomHeight = Mathf.Clamp(zoomHeight, zoomMin, zoomMax);
		camObject.transform.localPosition = new Vector3(0, zoomHeight, 0);
	}
}
