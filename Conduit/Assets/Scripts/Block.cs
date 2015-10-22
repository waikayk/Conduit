using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Block : MonoBehaviour {
	public Renderer renderComponent;
	public Collider blockCollider;
	public Material onMat;
	public Material offMat;
	public bool isNonConductive = false;
	
	public Rigidbody blockPhysics{get; set;}
	public Dictionary<int, Block> connectedBlocks{get;set;}
	public bool isPulsing {get;set;}
	
	public virtual void Start(){
		connectedBlocks = new Dictionary<int, Block>();
		blockPhysics = GetComponent<Rigidbody>();
		isPulsing = false;
		
		renderComponent.material = offMat;
	}
	
	public void PulsePower(int sourceID, WaitForSeconds rate){
		if(isPulsing || isNonConductive) return;
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
		
		renderComponent.material = onMat;
		yield return rate;
		
		renderComponent.material = offMat;
		yield return rate;
		isPulsing = false;
	}
	
	public IEnumerator Move(Vector3 adjustment, float speed = 1f){
		float t = 0;
		Vector3 moveFrom = transform.position;
		Vector3 moveTo = transform.position + adjustment;
		moveTo = new Vector3(Mathf.Round(moveTo.x), moveTo.y, Mathf.Round(moveTo.z));
		while(t < 1f){
			blockPhysics.MovePosition(
				Vector3.Lerp(moveFrom, moveTo, t)
			);
			yield return new WaitForEndOfFrame();
			t += Time.deltaTime * speed/2f;
		}
		blockPhysics.MovePosition(moveTo);
	}
}
