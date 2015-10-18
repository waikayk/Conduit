using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Block : MonoBehaviour {
	public Material onMat;
	public Material offMat;
	
	public Renderer render{get; set;}
	public Rigidbody blockPhysics{get; set;}
	public Dictionary<int, Block> connectedBlocks{get;set;}
	public bool isPulsing {get;set;}
	
	//private Vector3 moveTo = Vector3.zero;
	
	public virtual void Start(){
		connectedBlocks = new Dictionary<int, Block>();
		render = GetComponent<Renderer>();
		blockPhysics = GetComponent<Rigidbody>();
		isPulsing = false;
		
		render.material = offMat;
	}
	
//	public virtual void FixedUpdate(){
//		if(moveTo != Vector3.zero){
//			blockPhysics.MovePosition(transform.position + moveTo);
//			moveTo = Vector3.zero;
//		}
//	}
	
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
	
//	public virtual void OnCollisionStay(Collision other){
//		if(other.gameObject.tag == "Player" && OrthoCheck(other.transform.forward)){
//			moveTo = other.transform.forward;
//			//blockPhysics.MovePosition(transform.position + other.transform.forward * Time.deltaTime);
//		}
//		if(other.gameObject.tag == "Block"){
//			moveTo = Vector3.zero;
//		}
//	}
	
	
	public virtual void OnCollisionEnter(Collision other){
		if(other.transform.tag == "Player"){
			blockPhysics.mass = 2f;
		}
		else if (other.transform.tag == "Block"){
			blockPhysics.mass = 1000f;
			blockPhysics.Sleep();
		}
	}
	
	public virtual void OnCollisionExit(Collision other){
		if(other.transform.tag == "Player"){
			blockPhysics.mass = 1000f;
		}
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
}
