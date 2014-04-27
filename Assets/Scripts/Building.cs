using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

	public float timeBuild = 10;
	public float ysize = 50;

	float objY;
	float count =0;
	// Use this for initialization
	void Start () {
		objY = transform.position.y;
		transform.position = new Vector3(transform.position.x, transform.position.y-ysize, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		count += Time.deltaTime;
		if(count<timeBuild) { 
			transform.position = new Vector3(transform.position.x, transform.position.y + (ysize/timeBuild)*Time.deltaTime, transform.position.z);
		}
	}
}
