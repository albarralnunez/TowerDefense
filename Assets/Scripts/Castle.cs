using UnityEngine;
using System.Collections;

public class Castle : MonoBehaviour {

	public GameObject town;
	public GameObject selection;
	public int radius = 150;
	public int townCost = 900;
	public int hp = 20000;


	int gold = 1000;
	int people = 0;
	int life;
	GameObject selectionHighlight;

	void Start() {
		life = hp;
	}

	void Awake () {
		selectionHighlight = (GameObject) Instantiate(selection, new Vector3(transform.position.x-23, transform.position.y,transform.position.z), transform.rotation);
		selectionHighlight.transform.localScale = new Vector3(radius, 1, radius);
		selectionHighlight.SetActive(false);	
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

	public void addPeople(int peopl) {
		people += peopl;
	}

	public void addGold(int gld) {
		gold += gld;
	}

	public int getTotalHP() {
		return hp;
	}

	public int getLife() {
		return life;
	}

	public void hit(int attack) {
		life -= attack;
	}

	public GameObject buildTown() {
		if((gold-townCost)>=0) {
			gold -= townCost;
			return (GameObject) Instantiate (town , new Vector3(0,0,0), town.transform.rotation);
		}else return null;
	}
	
}
