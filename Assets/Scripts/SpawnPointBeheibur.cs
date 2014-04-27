using UnityEngine;
using System.Collections;

public class SpawnPointBeheibur : MonoBehaviour {

	// Use this for initialization
	void Start () {
		InvokeRepeating("Spawn", 5,5);
	}

	// Update is called once per frame
	void Update () {
	}

	void Spawn() {
		GameObject go = (GameObject)Instantiate(Resources.Load<GameObject> ("Bot"));
		Pathfinding.MineBotAI comp = go.GetComponent<Pathfinding.MineBotAI>();
		comp.target = GameObject.Find("Target").transform;		
	}
}