using UnityEngine;
using System.Collections;

public class World{
	#region Singleton Pattern
	private static World instanceInternal;
	public static World instance { 
		get{
			if (instanceInternal == null){
				new World();
			}
			return instanceInternal;
		}
	}

	public World(){
		if (instanceInternal != null){
			//Debug.Log ("Cannot have two instances of singleton. Ignoring...");
			return;
		}
		instanceInternal = this;
		Initialize();
	}
	#endregion
	
	public GameController master;
	public PlayerController playerControl;
	public CameraController cameraControl;

	void Initialize(){

	}
}
