﻿using UnityEngine;
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

	void Awake(){
		World.instance.playerControl = this;
	}

	void Start(){
		playerPhysics = GetComponent<CharacterController>();
		attachmentIndicator.SetActive(false);
		canAttachIndicator.SetActive(false);
		
		currentRoutine = StartCoroutine(ProcessPlayerMovement());
	}
	
	void Update(){
		if(Input.GetButtonDown("Attach")){
			if(isAttached) Detach();
			else Attach();
		}
		else if(Input.GetButtonDown("Flip") && touchingBlock != null){
			touchingBlock.FlipPolarity();
		}
	}
	
	IEnumerator ProcessPlayerMovement(){

		while(true){
		Vector3 inputAxis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
		bool isShifted = Input.GetButton("Boost");
		
		if(inputAxis != Vector3.zero){
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(inputAxis, Vector3.up), 15f);
			playerPhysics.Move(inputAxis * (isShifted ? fastSpeed : normalSpeed) * Time.deltaTime + Vector3.down);
		}
			yield return null;
		}
	}
	
	IEnumerator ProcessBlockMovement(){
		yield return new WaitForSeconds(0.1f);

		while(true){
			Vector3 inputAxis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
			bool isShifted = Input.GetButton("Boost");
			
			if(inputAxis != Vector3.zero && CanMoveInDirection(inputAxis)){
				inputAxis = GetNearestOrthogonalDir(inputAxis);
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

		StopCoroutine(currentRoutine);
		
		transform.rotation = Quaternion.LookRotation(GetNearestOrthogonalDir(transform.forward), Vector3.up);
		
		currentRoutine = StartCoroutine(ProcessBlockMovement());	
	}
	
	public void Detach(){
		isAttached = false;
		attachmentIndicator.SetActive(false);
		canAttachIndicator.SetActive(true);
		
		StopCoroutine(currentRoutine);

		touchingBlock.blockPhysics.isKinematic = true;
		
		currentRoutine = StartCoroutine(ProcessPlayerMovement());
	}
	
	private bool CanMoveInDirection(Vector3 input){
		//if the dot product of input and a direction is equal to 1 or -1, it lies on the same axis
		float dot = Vector3.Dot(transform.forward, input);
		
		//Direction Check: Does it lie on the same axis as forward?
		if(Mathf.Abs(dot) < 0.99f) return false;
		
		//Racycast Check: Is there room in front or behind? Check in fork formation, in case of "tunnels"
		Ray ray;
		Vector3 orthoDir;
		if(Mathf.Sign(dot) > 0){
			//Forward, center prong
			ray = new Ray(touchingBlock.transform.position, input);
			if(Physics.Raycast(ray, 1.99f)) return false;
		}
		else {
			//Back, center prong
			ray = new Ray(transform.position, input);
			if(Physics.Raycast(ray, 1.49f)) return false;
			//Side prongs specifically for the player
			orthoDir = GetNearestOrthogonalDir(transform.forward);
			ray = new Ray(transform.position + GetSideProng(orthoDir, 1f, 0.5f), input);
			if(Physics.Raycast(ray, 1.49f)) return false;
			ray = new Ray(transform.position + GetSideProng(orthoDir, -1f, 0.5f), input);
			if(Physics.Raycast(ray, 1.49f)) return false;
		}
		
		//Side prings for the block
		orthoDir = GetNearestOrthogonalDir(transform.forward);
		ray = new Ray(touchingBlock.transform.position + GetSideProng(orthoDir, 1f, 0.75f), input);
		if(Physics.Raycast(ray, 1.99f)) return false;
		ray = new Ray(touchingBlock.transform.position + GetSideProng(orthoDir, -1f, 0.75f), input);
		if(Physics.Raycast(ray, 1.99f)) return false;
		
		return true;
	}
	
	private Vector3 GetNearestOrthogonalDir(Vector3 dir){
		float dot = Vector3.Dot(dir, Vector3.forward);
		if(dot > 0.5f) return Vector3.forward;
		else if (dot < -0.5f) return Vector3.back;
		
		dot = Vector3.Dot(dir, Vector3.right);
		if(dot >= 0.5f) return Vector3.right;
		else if (dot <= -0.5f) return Vector3.left;
		
		return Vector3.up;
	}
	
	private Vector3 GetSideProng(Vector3 axis, float sign, float magnitude = 1f){
		if(axis == Vector3.forward || axis == Vector3.back){
			return Vector3.right * sign * magnitude;
		}
		else if(axis == Vector3.right || axis == Vector3.left){
			return Vector3.forward * sign * magnitude;
		}
		else{
			Debug.LogError("Axis was not in an orthogonal direction.");
			return Vector3.zero;
		}
	}
	
	private IEnumerator Nudge(Vector3 adjustment, float speed){
		float t = 0;
		Vector3 moveFrom = transform.position;
		Vector3 moveTo = transform.position + adjustment;
		while(t < 1f){
			transform.position = (
				Vector3.Lerp(moveFrom, moveTo, t)
			);
			yield return null;
			t += Time.deltaTime * speed/2f;
		}
		transform.position = (moveTo);
	}
}
