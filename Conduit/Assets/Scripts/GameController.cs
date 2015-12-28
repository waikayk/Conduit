using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	void Awake(){
		World.instance.master = this;
	}
}
