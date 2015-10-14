using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	public float speed = 15f;
	public float lookSpeed = 7f;
	
	private Rigidbody playerPhysics;
	
	void Start () {
		playerPhysics = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate () {
		ProcessPlayerInput();
	}
	
	void ProcessPlayerInput(){
		//Apply Player Axis Input
		Vector3 inputAxis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
		//Vector3 moveAxis = transform.right * inputAxis.x + transform.forward * inputAxis.z;
		if(inputAxis != Vector3.zero){
			playerPhysics.MovePosition(transform.position + inputAxis * Time.fixedDeltaTime * speed);
			playerPhysics.MoveRotation(Quaternion.LookRotation(inputAxis, Vector3.up));
		}
		
//		Vector3 mouseAxis = new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"));
//		if(mouseAxis != Vector3.zero){
//			playerPhysics.MoveRotation(transform.rotation * Quaternion.AngleAxis(mouseAxis.x * lookSpeed, Vector3.up));
//		}
	}
}
