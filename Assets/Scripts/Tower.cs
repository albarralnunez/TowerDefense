using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour {

	public float timeBuild = 20;
	public GameObject smoke;
	public GameObject ruins;
	public float ysize = 80;
	public GameObject healthBar;
	public int life = 30;
	
	private int curLife;
	
	float count =0;
	
	public bool isDestroyed, isHit;
	
	HealthBar hbar;
	
	Castle castle;
	GameObject smokePart;
	GameObject floorPart;
	GameObject ruinsPart;
	GameObject healthBarPart;
	
	// Use this for initialization
	void Start () {
		isHit = isDestroyed = false;
		curLife = life;
		healthBarPart = (GameObject)Instantiate(healthBar, new Vector3(transform.position.x+10, healthBar.transform.position.y, transform.position.z+5), healthBar.transform.rotation);
		healthBarPart.transform.parent = transform;
		healthBarPart.SetActive(false);
		hbar = (HealthBar) healthBarPart.GetComponent("HealthBar");
		transform.position = new Vector3(transform.position.x, transform.position.y-ysize, transform.position.z);
		smokePart = (GameObject)Instantiate(smoke, new Vector3(transform.position.x+smoke.transform.position.x,smoke.transform.position.y,transform.position.z+smoke.transform.position.z), smoke.transform.rotation);
	}
	
	// Update is called once per frame
	void Update () {
		if(count<timeBuild) { 
			transform.position = new Vector3(transform.position.x, transform.position.y + (ysize/timeBuild)*Time.deltaTime, transform.position.z);
			count += Time.deltaTime;
			if(count>=timeBuild) {
				ParticleSystem sys = (ParticleSystem)smokePart.GetComponent("ParticleSystem");
				sys.Stop();
				Destroy(smokePart, 5);
				transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			}
		}
	}
	
	public void hit( int dmg) {
		curLife -= dmg;
		isHit = true;
		healthBarPart.SetActive(true);
		hbar.setHP((float)curLife/(float)life);
		if(curLife <= 0) {
			isHit = false;
			isDestroyed = true;
			healthBarPart.SetActive(false);
			gameObject.renderer.enabled = false;
			gameObject.collider.enabled = false;
			ruinsPart = (GameObject) Instantiate(ruins, transform.position, ruins.transform.rotation);
			ruinsPart.transform.parent = transform;
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
		curLife +=dmg;
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


