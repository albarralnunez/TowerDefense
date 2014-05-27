using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour {

	public float timeBuild = 20;
	public GameObject smoke;
	public float ysize = 80;

	public int life = 30;
	
	private int curLife;
	
	float objY;
	float count =0;

	GameObject smokePart;
	GameObject floorPart;
	
	// Use this for initialization
	void Start () {
		curLife = life;
		objY = transform.position.y;
		transform.position = new Vector3(transform.position.x, transform.position.y-ysize, transform.position.z);
		smokePart = (GameObject)Instantiate(smoke, new Vector3(transform.position.x+smoke.transform.position.x,smoke.transform.position.y,transform.position.z+smoke.transform.position.z), smoke.transform.rotation);
	}
	
	// Update is called once per frame
	void Update () {
		/*if(count<timeBuild) { 
			floorPart.transform.position = new Vector3(floorPart.transform.position.x, floorPart.transform.position.y + (ysize/timeBuild)*Time.deltaTime, floorPart.transform.position.z);
			transform.position = new Vector3(transform.position.x, transform.position.y + (ysize/timeBuild)*Time.deltaTime, transform.position.z);
			count += Time.deltaTime;
			if(count>=timeBuild) {
				ParticleSystem sys = (ParticleSystem)smokePart.GetComponent("ParticleSystem");
				sys.Stop();
				Destroy(smokePart, 5);
				floorPart.transform.position = new Vector3(floorPart.transform.position.x, floor.transform.position.y, floorPart.transform.position.z);
				transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			}
		}*/
	}
	
	public void hit( int dmg) {
		curLife -= dmg;
		if(curLife <= 0) {

			gameObject.SetActive(false);
			CancelInvoke();
		}
	}
}

