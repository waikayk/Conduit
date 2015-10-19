﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Block : MonoBehaviour {
	public Material onMat;
	public Material offMat;
	
	public Renderer render{get; set;}
	public Rigidbody blockPhysics{get; set;}
	public Dictionary<int, Block> connectedBlocks{get;set;}
	public bool isPulsing {get;set;}
	
	public virtual void Start(){
		connectedBlocks = new Dictionary<int, Block>();
		render = GetComponent<Renderer>();
		blockPhysics = GetComponent<Rigidbody>();
		isPulsing = false;
		
		render.material = offMat;
	}
	
	public void PulsePower(int sourceID, WaitForSeconds rate){
		if(isPulsing) return;
		StartCoroutine(Pulse(sourceID, rate));
	}
	
	private IEnumerator Pulse(int sourceID, WaitForSeconds rate){
		isPulsing = true;
		yield return rate;
		foreach(KeyValuePair<int, Block> element in connectedBlocks){
			//make sure to skip the source to prevent stack overflow
			if(element.Key == sourceID) continue;
			element.Value.PulsePower(gameObject.GetInstanceID(), rate);
		}
		
		render.material = onMat;
		yield return rate;
		
		render.material = offMat;
		yield return rate;
		isPulsing = false;
	}
	
	public IEnumerator Move(Vector3 adjustment, float speed = 1f){
		float t = 0;
		Vector3 moveFrom = transform.position;
		Vector3 moveTo = transform.position + adjustment;
		while(t < 1f){
			blockPhysics.MovePosition(
				Vector3.Lerp(moveFrom, moveTo, t)
			);
			yield return new WaitForEndOfFrame();
			t += Time.deltaTime * speed;
		}
		blockPhysics.MovePosition(moveTo);
	}
	
	public virtual void OnTriggerEnter(Collider other){
		if(other.gameObject.tag != "Connection") return;
		int otherID = other.gameObject.GetInstanceID();
		if(!connectedBlocks.ContainsKey(otherID)){
			connectedBlocks.Add(otherID, other.attachedRigidbody.GetComponent<Block>());
		}
		
	}
	
	public virtual void OnTriggerExit(Collider other){
		if(other.gameObject.tag != "Connection") return;
		int otherID = other.gameObject.GetInstanceID();
		if(connectedBlocks.ContainsKey(otherID)){
			connectedBlocks.Remove(otherID);
		}
	}
}