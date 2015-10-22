using UnityEngine;
using System.Collections;

public class BlockPanel : Block {
	void OnTriggerEnter(Collider other){
		if(other.transform.tag == "Block" || other.transform.tag == "Player"){
			isNonConductive = true;
		}
	}
	
		void OnTriggerExit(Collider other){
		if(other.transform.tag == "Block" || other.transform.tag == "Player"){
			isNonConductive = false;
		}
	}
}
