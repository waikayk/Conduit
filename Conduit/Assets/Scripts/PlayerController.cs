using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	//public Collider playerCollider;
	public float normalSpeed = 5f;
	public float fastSpeed = 15f;
	public GameObject attachmentIndicator;
	public GameObject canAttachIndicator;
	
	private CharacterController playerPhysics;
	private Coroutine currentRoutine;

	[HideInInspector]
	public bool isAttached = false;
	public Block touchingBlock{get;set;}
	
	void Start () {
		playerPhysics = GetComponent<CharacterController>();
		attachmentIndicator.SetActive(false);
		canAttachIndicator.SetActive(false);
		
		//currentRoutine = StartCoroutine(ProcessPlayerMovement());
	}
	
	void Update(){
		if(Input.GetButtonDown("Attach")){
			if(isAttached) Detach();
			else Attach();
		}

		if(!isAttached){
			ProcessPlayerMovement();
		}
	}
	
	void ProcessPlayerMovement(){
		//Tried making this a co-routine but none of the Waiting options will be as smooth as Update()
		Vector3 inputAxis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
		bool isShifted = Input.GetButton("Boost");
		
		if(inputAxis != Vector3.zero){
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(inputAxis, Vector3.up), 15f);
			playerPhysics.Move(inputAxis * (isShifted ? fastSpeed : normalSpeed) * Time.deltaTime + Vector3.down);
		}
	}
	
	IEnumerator ProcessBlockMovement(){
		yield return new WaitForSeconds(0.1f);

		while(true){
			Vector3 inputAxis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
			bool isShifted = Input.GetButton("Boost");
			
			if(inputAxis != Vector3.zero && CanMoveInDirection(inputAxis)){
				StartCoroutine(Nudge(inputAxis, isShifted ? fastSpeed : normalSpeed));
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
		canAttachIndicator.SetActive(false);

		//StopCoroutine(currentRoutine);
		
		transform.rotation = Quaternion.LookRotation(GetNearestOrthogonalDir(), Vector3.up);
		
		currentRoutine = StartCoroutine(ProcessBlockMovement());	
	}
	
	public void Detach(){
		isAttached = false;
		attachmentIndicator.SetActive(false);
		
		StopCoroutine(currentRoutine);

		touchingBlock.blockPhysics.isKinematic = true;
		
		touchingBlock = null;
		
		//currentRoutine = StartCoroutine(ProcessPlayerMovement());
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
	
	private Vector3 GetNearestOrthogonalDir(){
		float dot = Vector3.Dot(transform.forward, Vector3.forward);
		if(dot > 0.5f) return Vector3.forward;
		else if (dot < -0.5f) return Vector3.back;
		
		dot = Vector3.Dot(transform.forward, Vector3.right);
		if(dot >= 0.5f) return Vector3.right;
		else if (dot <= -0.5f) return Vector3.left;
		
		return Vector3.up;
	}
	
	private IEnumerator Nudge(Vector3 adjustment, float speed){
		float t = 0;
		Vector3 moveFrom = transform.position;
		Vector3 moveTo = transform.position + adjustment;
		while(t < 1f){
			transform.position = (
				Vector3.Lerp(moveFrom, moveTo, t)
			);
			yield return new WaitForEndOfFrame();
			t += Time.deltaTime * speed/2f;
		}
		transform.position = (moveTo);
	}
}
