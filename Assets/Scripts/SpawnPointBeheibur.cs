using UnityEngine;
using System.Collections;

public class SpawnPointBeheibur : MonoBehaviour {

	public int minions;

	// Use this for initialization
	void Start () {
		InvokeRepeating("Spawn", minions,minions);
	}

	// Update is called once per frame
	void Update () {
	}

	void Spawn() {
		GameObject go = (GameObject)Instantiate(Resources.Load<GameObject> ("Bot"),gameObject.transform.position, Quaternion.identity);
		go.transform.parent = gameObject.transform;
		Pathfinding.MineBotAI comp = go.GetComponent<Pathfinding.MineBotAI>();
		comp.target = GameObject.Find("Target").transform;
	}
}