using UnityEngine;
using System.Collections;

public class Castle : MonoBehaviour {

	public GameObject town;
	public GameObject selection;
	public int radius = 150;
	public int life = 20000;
	public GameObject healthBar;
	public int lvlWalls =1;
	int gold = 1000;
	int goldPerSec = 5;
	int people = 0;
	int curLife;
	HealthBar hbar;
	GameObject selectionHighlight;
	GameObject healthBarPart;


	void Start() {
		curLife = life;
	}

	void Awake () {
		selectionHighlight = (GameObject) Instantiate(selection, new Vector3(transform.position.x-23, transform.position.y,transform.position.z), transform.rotation);
		selectionHighlight.transform.localScale = new Vector3(radius, 1, radius);
		selectionHighlight.SetActive(false);	
		healthBarPart = (GameObject)Instantiate(healthBar, new Vector3(transform.position.x, healthBar.transform.position.y, transform.position.z), healthBar.transform.rotation);
		healthBarPart.transform.parent = transform;
		healthBarPart.SetActive(false);
		hbar = (HealthBar) healthBarPart.GetComponent("HealthBar");
		InvokeRepeating("addGold",0f,1f);
	}
	
	public void setHighlight(bool active) {
		selectionHighlight.SetActive(active);
	}

	public int getGold() {
		return gold;
	}
	public int getPeople() {
		return people;
	}
	public int getGoldPerSec(){
		return goldPerSec;
	}

	void addGold(){
		gold += goldPerSec;
	}

	public void addPeople(int peopl) {
		people += peopl;
	}

	public void addGold(int gld) {
		gold += gld;
	}

	public int getTotalHP() {
		return life;
	}

	public void upgradeLife(int up) {
		++lvlWalls;
		life += up;
		curLife +=up;
		hbar.setHP((float)curLife/(float)life);
	}

	public int getLife() {
		return curLife;
	}

	public void hit(int attack) {
		curLife -= attack;
		healthBarPart.SetActive(true);
		hbar.setHP((float)curLife/(float)life);
	}

	public GameObject buildTown() {
		return (GameObject) Instantiate (town , new Vector3(0,0,0), town.transform.rotation);
	}
	
}
