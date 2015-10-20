using UnityEngine;
using System.Collections;

public class Connections : MonoBehaviour {
	public Block parentBlock;
	
	void Start(){
		if(!parentBlock) parentBlock = transform.parent.GetComponent<Block>();
	}
	
	public virtual void OnTriggerEnter(Collider other){
		if(other.gameObject.tag != "Connection") return;
		Connections otherConnection = other.attachedRigidbody.GetComponent<Connections>();
		int otherID = otherConnection.parentBlock.gameObject.GetInstanceID();
		if(!parentBlock.connectedBlocks.ContainsKey(otherID)){
			parentBlock.connectedBlocks.Add(otherID, otherConnection.parentBlock);
		}
		
	}
	
	public virtual void OnTriggerExit(Collider other){
		if(other.gameObject.tag != "Connection") return;
		Connections otherConnection = other.attachedRigidbody.GetComponent<Connections>();
		int otherID = otherConnection.parentBlock.gameObject.GetInstanceID();
		if(parentBlock.connectedBlocks.ContainsKey(otherID)){
			parentBlock.connectedBlocks.Remove(otherID);
		}
	}
}
