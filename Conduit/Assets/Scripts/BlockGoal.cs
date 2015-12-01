using UnityEngine;
using System.Collections;

public class BlockGoal : Block {
	public int loadLevel = -1;
	
	public bool isPowered{get; set;}
	
	public override void PulsePower(int sourceID, WaitForSeconds rate){
		if(isPulsing || isNonConductive) return;
		base.PulsePower(sourceID, rate);
		
		if(!isPowered){
			print ("Goal Reached");
			
			//TODO - Move level progression code to GameController
			if(loadLevel >= 0) StartCoroutine(DelayedLoadLevel());
			
			isPowered = true;
		}
		else{
			print ("Beep");
		}
	}
	
	IEnumerator DelayedLoadLevel(){
		yield return new WaitForSeconds(0.5f);
		Application.LoadLevel(loadLevel);
	}
}
