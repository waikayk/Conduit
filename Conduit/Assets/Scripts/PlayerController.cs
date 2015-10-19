using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public float normalSpeed = 5f;
	public float fastSpeed = 15f;
	public float lookSpeed = 7f;
	
	private Rigidbody playerPhysics;
	private Coroutine currentRoutine;
	private bool isAttached = false;
	
	private Block touchingBlock;
	private FixedJoint fixedJoint;
	
	void Start () {
		playerPhysics = GetComponent<Rigidbody>();
		
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
		while(true){
			Vector3 inputAxis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
			bool isShifted = Input.GetButton("Boost");
			
			if(inputAxis != Vector3.zero && OrthoCheck(inputAxis)){
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

		StopCoroutine(currentRoutine);
		
		playerPhysics.interpolation = RigidbodyInterpolation.Interpolate;
		touchingBlock.blockPhysics.interpolation = RigidbodyInterpolation.Interpolate;
		touchingBlock.blockPhysics.isKinematic = false;
		
		//Attach
		//fixedJoint = gameObject.AddComponent<FixedJoint>();
		//fixedJoint.connectedBody = touchingBlock.blockPhysics;
		
		currentRoutine = StartCoroutine(ProcessBlockMovement());	
	}
	
	public void Detach(){
		isAttached = false;
		
		StopCoroutine(currentRoutine);
		
		playerPhysics.interpolation = RigidbodyInterpolation.Interpolate;
		touchingBlock.blockPhysics.isKinematic = true;
		
		//Detach
		//Destroy(fixedJoint);
		
		currentRoutine = StartCoroutine(ProcessPlayerMovement());
	}
	
	private bool OrthoCheck(Vector3 input){
		//if the dot product of input and a direction is equal to 1 or -1, it lies on the same axis
		float a = Mathf.Abs(Vector3.Dot(Vector3.forward, input));
		float b = Mathf.Abs(Vector3.Dot(Vector3.right, input));
		if(Mathf.Approximately(a, 1f) || Mathf.Approximately(b, 1f))
			return true;
		else
			return false;
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
			t += Time.deltaTime * speed;
		}
		playerPhysics.MovePosition(moveTo);
	}
	
	void OnCollisionEnter(Collision other){
		OnCollisionStay(other);
	}
	
	void OnCollisionStay(Collision other){
		if(isAttached) return;
		//For smooth looking movements
		if(other.transform.tag == "Block" || other.transform.tag == "Immovable"){
			playerPhysics.interpolation = RigidbodyInterpolation.None;
		}
	}
	
	void OnCollisionExit(Collision other){
		if(isAttached) return;
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
