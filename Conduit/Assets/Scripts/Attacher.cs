using UnityEngine;
using System.Collections;

public class Attacher : MonoBehaviour {
	public PlayerController playerController;

	void OnTriggerEnter(Collider other){
		if(other.transform.tag == "Block" && !playerController.isAttached){
			playerController.canAttachIndicator.SetActive(true);
			playerController.touchingBlock = other.attachedRigidbody.GetComponent<Block>();
		}
	}
	
	void OnTriggerExit(Collider other){
		if(other.transform.tag == "Block" && !playerController.isAttached){
			playerController.canAttachIndicator.SetActive(false);
			playerController.touchingBlock = null;
		}
	}
}
