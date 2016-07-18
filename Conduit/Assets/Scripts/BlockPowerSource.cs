using UnityEngine;
using System.Collections;

public class BlockPowerSource : Block {
	public float pulseRate = 4f;
	public float pulseSpeed = 2f;
	
	public override void Start (){
		base.Start();
		StartCoroutine(RepeatPulse());
	}
	
	IEnumerator RepeatPulse(){
		int sourceID = gameObject.GetInstanceID();
		while(true){
			PulsePower(sourceID, new WaitForSeconds(0.5f/pulseSpeed));
			yield return new WaitForSeconds(1f/pulseRate);
		}
	}
}
