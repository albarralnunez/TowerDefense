using UnityEngine;
using System.Collections;

public class colliderSoldier : MonoBehaviour {

	void OnTriggerEnter(Collider a) {
		if (a.tag == "Enemy") {
			gameObject.transform.parent.SendMessage("SetToFight", a.gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
