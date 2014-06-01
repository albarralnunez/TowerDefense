using UnityEngine;
using System.Collections;

public class body : MonoBehaviour {

	GameObject bod; 
	public GameObject puddle;
	// Use this for initialization
	void Start () {
		bod = (GameObject) transform.FindChild("body").gameObject;
		puddle = (GameObject) Instantiate(puddle, new Vector3(transform.position.x , 0.5f, transform.position.z), Quaternion.Euler(new Vector3(puddle.transform.rotation.x, Random.Range(0f, 360f), puddle.transform.rotation.z)));
		Destroy (gameObject, 1.5f);
		Destroy (puddle, 20);
	}
	
	// Update is called once per frame
	void Update () {
		puddle.transform.localScale = new Vector3(puddle.transform.localScale.x +0.08f , puddle.transform.localScale.y, puddle.transform.localScale.z+0.08f);
		bod.transform.localScale = bod.transform.localScale*0.9f;
	}
}
