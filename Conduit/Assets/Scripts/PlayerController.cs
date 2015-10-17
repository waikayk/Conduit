using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public float slowSpeed = 5f;
	public float speed = 15f;
	public float lookSpeed = 7f;
	
	private Rigidbody playerPhysics;
	
	void Start () {
		playerPhysics = GetComponent<Rigidbody>();
	}
	
	void Update () {
		ProcessPlayerInput();
	}
	
	void ProcessPlayerInput(){
		//Apply Player Axis Input
		Vector3 inputAxis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
		bool isShifted = Input.GetButton("Boost");
		//Vector3 moveAxis = transform.right * inputAxis.x + transform.forward * inputAxis.z;
		if(inputAxis != Vector3.zero){
			playerPhysics.MoveRotation(Quaternion.LookRotation(inputAxis, Vector3.up));
			playerPhysics.MovePosition(transform.position + inputAxis * Time.deltaTime * (isShifted ? slowSpeed: speed));
		}
	}
	
	void OnCollisionEnter(Collision other){
		OnCollisionStay(other);
	}
	
	void OnCollisionStay(Collision other){
		//For smooth looking movements
		if(other.transform.tag == "Block" || other.transform.tag == "Immovable"){
			playerPhysics.interpolation = RigidbodyInterpolation.None;
		}
	}
	
	void OnCollisionExit(Collision other){
		if(other.transform.tag == "Block" || other.transform.tag == "Immovable"){
			playerPhysics.interpolation = RigidbodyInterpolation.Interpolate;
		}
	}
}
