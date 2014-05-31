using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

	public float timeBuild = 20;
	public GameObject smoke;
	public GameObject floor;
	public GameObject ruins;
	public float ysize = 80;
	public GameObject soldier;
	public GameObject healthBar;
	public float timeSpawn = 20;
	public int life = 30;
	public int goldPerSec = 5;

	private int curLife;
	
	float count =0;

	public bool isDestroyed, isHit;

	HealthBar hbar;

	Castle castle;
	GameObject smokePart;
	GameObject floorPart;
	GameObject ruinsPart;
	GameObject healthBarPart;
	GameObject soldierPart =null;

	// Use this for initialization
	void Start () {
		isHit = isDestroyed = false;
		curLife = life;
		GameObject c = GameObject.FindGameObjectWithTag("Castle");
		castle = (Castle) c.GetComponent("Castle");
		floorPart = (GameObject)Instantiate(floor, new Vector3(transform.position.x+floor.transform.position.x,floor.transform.position.y-ysize,transform.position.z+floor.transform.position.z), floor.transform.rotation);
		floorPart.transform.parent = transform;
		healthBarPart = (GameObject)Instantiate(healthBar, new Vector3(transform.position.x+10, healthBar.transform.position.y, transform.position.z+5), healthBar.transform.rotation);
		healthBarPart.transform.parent = transform;
		healthBarPart.SetActive(false);
		hbar = (HealthBar) healthBarPart.GetComponent("HealthBar");
		transform.position = new Vector3(transform.position.x, transform.position.y-ysize, transform.position.z);
		smokePart = (GameObject)Instantiate(smoke, new Vector3(transform.position.x+smoke.transform.position.x,smoke.transform.position.y,transform.position.z+smoke.transform.position.z), smoke.transform.rotation);
		InvokeRepeating("spawnSoldier", timeBuild+timeSpawn,timeSpawn);
		InvokeRepeating("addGold", timeBuild,1);
	}

	void addGold() {
		castle.addGold(goldPerSec);
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
			Town tr = (Town)transform.parent.GetComponentInChildren<Town>();
			float xx = (float)Random.Range(-tr.getRadius(),tr.getRadius()); 
			float yy = (float)Random.Range(-tr.getRadius(),tr.getRadius());
			Vector3 aux = new Vector3(xx,0,yy);
			soldierPart= (GameObject) Instantiate(soldier, transform.position, soldier.transform.rotation);
			soldierPart.transform.FindChild("defPos").transform.position =  tr.transform.position+aux;
			castle.addPeople(1);
		}
	}

	public void hit( int dmg) {
		curLife -= dmg;
		isHit = true;
		healthBarPart.SetActive(true);
		hbar.setHP((float)curLife/(float)life);
		transform.parent.gameObject.SendMessage("buildingHit");
		if(curLife <= 0) {
			isHit = false;
			isDestroyed = true;
			healthBarPart.SetActive(false);
			gameObject.renderer.enabled = false;
			gameObject.collider.enabled = false;
			ruinsPart = (GameObject) Instantiate(ruins, transform.position, ruins.transform.rotation);
			ruinsPart.transform.parent = transform;
			CancelInvoke();
			transform.parent.gameObject.SendMessage("buildingDestroyed");
		}
	}

	public void rebuild() {
		for(int i=0; i< transform.childCount; ++i) {
			Destroy(transform.GetChild(i).gameObject);
		}
		gameObject.renderer.enabled = true;
		gameObject.collider.enabled = true;
		Start ();
	}

	public void heal(int dmg) {
		curLife+=dmg;
		hbar.setHP((float)curLife/(float)life);
		if(curLife >= life) {
			curLife = life;
			healthBarPart.SetActive(false);
			isHit = false;
		}
	}

	public void upgradeLife(int l) {
		life += l;
		curLife += l;
		hbar.setHP((float)curLife/(float)life);
	}
}
