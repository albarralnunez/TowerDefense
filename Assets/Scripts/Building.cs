using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

	public float timeBuild = 20;
	public GameObject smoke;
	public GameObject floor;
	public float ysize = 40;
	public GameObject soldier;
	public float timeSpawn = 20;

	float objY;
	float count =0;

	Castle castle;
	GameObject smokePart;
	GameObject floorPart;
	GameObject soldierPart =null;

	// Use this for initialization
	void Start () {
		GameObject c = GameObject.FindGameObjectWithTag("Castle");
		castle = (Castle) c.GetComponent("Castle");
		objY = transform.position.y;
		floorPart = (GameObject)Instantiate(floor, new Vector3(transform.position.x+floor.transform.position.x,floor.transform.position.y-ysize,transform.position.z+floor.transform.position.z), floor.transform.rotation);
		floorPart.transform.parent = transform;
		transform.position = new Vector3(transform.position.x, transform.position.y-ysize, transform.position.z);
		smokePart = (GameObject)Instantiate(smoke, new Vector3(transform.position.x+smoke.transform.position.x,smoke.transform.position.y,transform.position.z+smoke.transform.position.z), smoke.transform.rotation);
		InvokeRepeating("spawnSoldier", timeBuild+timeSpawn,timeSpawn);
	}
	
	// Update is called once per frame
	void Update () {
		if(count<timeBuild) { 
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
		}
	}
	void spawnSoldier() {
		if(soldierPart == null) {
			soldierPart= (GameObject) Instantiate(soldier, transform.position, soldier.transform.rotation);
			soldierPart.transform.localScale = new Vector3(10,10,10);
			castle.addPeople(1);
		}
	}
}
