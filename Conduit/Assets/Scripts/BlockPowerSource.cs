using UnityEngine;
using System.Collections;

public class BlockPowerSource : Block {
	public float pulseRate = 0.7f;
	
	public override void Start (){
		base.Start();
		StartCoroutine(RepeatPulse());
	}
	
	IEnumerator RepeatPulse(){
		int sourceID = gameObject.GetInstanceID();
		WaitForSeconds rate = new WaitForSeconds(pulseRate);
		while(true){
			PulsePower(sourceID, rate);
			yield return rate;
		}
	}
}
