using UnityEngine;
using System.Collections;

public class colliderSoldier : MonoBehaviour {

	public void OnTrigerEnter(Collider a) {
		if (a.tag == "Enemy") {
			gameObject.transform.parent.FindChild("SoldierObj").SendMessage("SetToFight", a.gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
