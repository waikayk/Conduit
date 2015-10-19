using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public float normalSpeed = 5f;
	public float fastSpeed = 15f;
	public float lookSpeed = 7f;
	public GameObject attachmentIndicator;
	
	private Rigidbody playerPhysics;
	private Coroutine currentRoutine;
	private bool isAttached = false;
	
	private Block touchingBlock;
	private FixedJoint fixedJoint;
	
	void Start () {
		playerPhysics = GetComponent<Rigidbody>();
		attachmentIndicator.SetActive(false);
		
		currentRoutine = StartCoroutine(ProcessPlayerMovement());
	}
	
	void Update(){
		if(Input.GetMouseButtonDown(0)){
			if(isAttached) Detach();
			else Attach();
		}
	}
	
	IEnumerator ProcessPlayerMovement(){
		while(true){
			Vector3 inputAxis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
			bool isShifted = Input.GetButton("Boost");
			
			if(inputAxis != Vector3.zero){
				playerPhysics.MoveRotation(Quaternion.LookRotation(inputAxis, Vector3.up));
				playerPhysics.MovePosition(transform.position + inputAxis * Time.deltaTime * (isShifted ? fastSpeed: normalSpeed));
			}
			yield return new WaitForEndOfFrame();
		}
	}
	
	IEnumerator ProcessBlockMovement(){
		yield return new WaitForSeconds(0.1f);
		while(true){
			Vector3 inputAxis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
			bool isShifted = Input.GetButton("Boost");
			
			if(inputAxis != Vector3.zero && CanMoveInDirection(inputAxis)){
				StartCoroutine(Move(inputAxis, isShifted ? fastSpeed : normalSpeed));
				yield return StartCoroutine(
					touchingBlock.Move(inputAxis, isShifted? fastSpeed : normalSpeed)
				);	
			}
			yield return new WaitForEndOfFrame();
		}
	}
	
	public void Attach(){
		if(touchingBlock == null) return;
		
		isAttached = true;
		attachmentIndicator.SetActive(true);

		StopCoroutine(currentRoutine);
		
		GetComponent<Collider>().enabled = false;
		playerPhysics.isKinematic = true;
		touchingBlock.gameObject.GetComponent<Collider>().enabled = false;
		
		currentRoutine = StartCoroutine(ProcessBlockMovement());	
	}
	
	public void Detach(){
		isAttached = false;
		attachmentIndicator.SetActive(false);
		
		StopCoroutine(currentRoutine);
		
		GetComponent<Collider>().enabled = true;
		playerPhysics.isKinematic = false;

		touchingBlock.gameObject.GetComponent<Collider>().enabled = true;
		touchingBlock.blockPhysics.isKinematic = true;
		
		currentRoutine = StartCoroutine(ProcessPlayerMovement());
	}
	
	private bool CanMoveInDirection(Vector3 input){
		//if the dot product of input and a direction is equal to 1 or -1, it lies on the same axis
		float dot = Vector3.Dot(transform.forward, input);
		
		//Direction Check: Does it lie on the same axis as forward?
		if(Mathf.Abs(dot) < 0.99f) return false;
		
		//Racycast Check: Is there room in front or behind?
		
		if(Mathf.Sign(dot) > 0){
			//Forward
			Ray ray = new Ray(touchingBlock.transform.position, input);
			return !Physics.Raycast(ray, 1.25f);
		}
		else {
			//Back
			Ray ray = new Ray(transform.position, input);
			return !Physics.Raycast(ray, 0.7f);
		}
	}
	
	private IEnumerator Move(Vector3 adjustment, float speed){
		float t = 0;
		Vector3 moveFrom = transform.position;
		Vector3 moveTo = transform.position + adjustment;
		while(t < 1f){
			playerPhysics.MovePosition(
				Vector3.Lerp(moveFrom, moveTo, t)
			);
			yield return new WaitForEndOfFrame();
			t += Time.deltaTime * speed/2f;
		}
		playerPhysics.MovePosition(moveTo);
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
	
	void OnTriggerEnter(Collider other){
		if(other.transform.tag == "Block" && !isAttached){
			touchingBlock = other.gameObject.GetComponent<Block>();
		}
	}
	
	void OnTriggerExit(Collider other){
		if(other.transform.tag == "Block" && !isAttached){
			touchingBlock = null;
		}
	}
}
