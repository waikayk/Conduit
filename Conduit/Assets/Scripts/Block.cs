using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Block : MonoBehaviour {
	public Renderer renderComponent;
	public Collider blockCollider;
	public Material onMat;
	public Material offMat;
	public bool isNonConductive = false;
	public bool hasPolarity = true;
	
	public Rigidbody blockPhysics{get; set;}
	public Dictionary<int, Block> connectedBlocks{get;set;}
	public bool isPulsing {get;set;}

	private bool isFlipped = false;
	
	public virtual void Start(){
		connectedBlocks = new Dictionary<int, Block>();
		blockPhysics = GetComponent<Rigidbody>();
		isPulsing = false;
		
		renderComponent.material = offMat;
	}
	
	public virtual void PulsePower(int sourceID, WaitForSeconds delay){
		if(isPulsing || isNonConductive) return;
		StartCoroutine(Pulse(sourceID, delay));
	}
	
	private IEnumerator Pulse(int sourceID, WaitForSeconds delay){
		isPulsing = true;
		//yield return rate;
//		foreach(KeyValuePair<int, Block> element in connectedBlocks){
//			//make sure to skip the source to prevent stack overflow
//			if(element.Key == sourceID) continue;
//			element.Value.PulsePower(gameObject.GetInstanceID(), rate);
//		}
		
		renderComponent.material = onMat;
		yield return delay;
		
		foreach(KeyValuePair<int, Block> element in connectedBlocks){
			//make sure to skip the source to prevent stack overflow
			if(element.Key == sourceID) continue;
			element.Value.PulsePower(gameObject.GetInstanceID(), delay);
		}
		
		renderComponent.material = offMat;
		yield return delay;
		isPulsing = false;
	}
	
	public IEnumerator Move(Vector3 adjustment, float speed = 1f){
		float t = 0;
		Vector3 moveFrom = transform.position;
		Vector3 moveTo = transform.position + adjustment;
		moveTo = new Vector3(Mathf.Round(moveTo.x), moveTo.y, Mathf.Round(moveTo.z));
		while(t < 1f){
			transform.position = (
				Vector3.Lerp(moveFrom, moveTo, t)
			);
			yield return null;
			t += Time.deltaTime * speed/2f;
		}
		transform.position = (moveTo);
	}

	public void FlipPolarity(){
		if(!hasPolarity) return;

		if(!isFlipped){
			transform.eulerAngles += new Vector3(0, 90f, 0);
		}
		else{
			transform.eulerAngles -= new Vector3(0, 90f, 0);
		}

		isFlipped = !isFlipped;
	}
}
