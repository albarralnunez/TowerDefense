using UnityEngine;
using System.Collections.Generic;


public class Town : MonoBehaviour {

	enum State {Constructing, Building, Attacking, Repairing, Waiting, Destroyed};

	public GameObject[] buildings;
	public GameObject[] walls;
	public GameObject selection;
	public GameObject floor;

	public float spaceBetweenBuildings = 15;
	public float secondsToBuild = 10;
	public int people = 5;
	public int radius = 100;

	public int[] levelReq;
	Queue<GameObject> sideBuildings;
	Queue<GameObject> wallsBuilt;

	int numhouses = 0;
	float countAttack = 0;
	int level =1;
	int wallLevel =0;
	float timecont =0;
	GameObject selectionHighlight;
	State state;

	// Use this for initialization
	void Start () {
		state = State.Constructing;
		sideBuildings= new Queue<GameObject>();
		wallsBuilt= new Queue<GameObject>();
	}

	void Awake () {
		floor = (GameObject) Instantiate (floor, transform.position, transform.rotation);
		floor.transform.parent = transform;
		selectionHighlight = (GameObject) Instantiate(selection, selection.transform.position, transform.rotation);
		selectionHighlight.transform.parent = gameObject.transform;
		selectionHighlight.transform.localScale = new Vector3(radius, 1, radius);
		selectionHighlight.SetActive(false);	
	}

	public void startBuilding() {
		Destroy(floor);
		int indx = (int)Random.Range(0,buildings.Length);
		++numhouses;
		GameObject building = (GameObject) Instantiate(buildings[indx], transform.position, buildings[indx].transform.rotation);
		building.transform.parent = transform;
		AstarPath.active.UpdateGraphs (building.collider.bounds,5); //TODO!
		sideBuildings.Enqueue(building);
		state = State.Building;
	}

	void Update () {
		if(state == State.Building) {
			timecont += Time.deltaTime;
			if(timecont>= secondsToBuild) {
				timecont = 0;
				Vector3 pos = new Vector3(0,0,0);
				bool found = false;
				while(!found) {
					GameObject sideBuilding = sideBuildings.Peek();
					for(int i=0; i< 4 && !found; ++i) {
						pos = sideBuilding.transform.position;
						switch(i) {
							case 0: pos.x -= spaceBetweenBuildings; break;
							case 1: pos.z -= spaceBetweenBuildings; break;
							case 2: pos.x += spaceBetweenBuildings; break;
							case 3: pos.z += spaceBetweenBuildings; break;
						}
						found = true;
						for(int j = 0; j< transform.childCount && found; ++j) {
							Vector3 posch = transform.GetChild(j).position;
							if(pos.x == posch.x && pos.z == posch.z) found = false;
						}
					}
					if(!found) sideBuildings.Dequeue();
					else {
						int b = Random.Range(0,buildings.Length);
						++numhouses;
						GameObject building = (GameObject) Instantiate(buildings[b], new Vector3(pos.x, 0, pos.z), buildings[b].transform.rotation);
						building.transform.parent = transform;
						AstarPath.active.UpdateGraphs (building.collider.bounds,5); //TODO:!!!
						sideBuildings.Enqueue(building);
						if(transform.childCount-2 > levelReq[level-1]) {
							++level;
							radius += 50;
							selectionHighlight.transform.localScale = new Vector3(radius, 1, radius);
							if(level >=4) state = State.Waiting;
						}
					}
				}
				if(wallLevel>0) {
					destroyWalls();
					buildWalls();
				}
			}
		}
		else if(state == State.Repairing) {}
		else if(state == State.Attacking) {
			countAttack += Time.deltaTime;
			if(countAttack >= 10) state= State.Repairing;
		}
	}

	public void setHighlight(bool active) {
		selectionHighlight.SetActive(active);
	}

	public int getLevel() {
		return level;
	}

	public void buildingHit() {
		state = State.Attacking;
		countAttack =0;
	}


	public int getRadius() {
		return radius;
	}

	public int getPeople() {
		return Mathf.Max(people, people+transform.childCount-2);
	}

	public void wallLevelUp() {
		++wallLevel; 
		if(wallLevel>walls.Length) wallLevel = walls.Length;
		destroyWalls ();
		buildWalls ();
	}

	void destroyWalls() {
		while(wallsBuilt.Count>0) {
			Destroy (wallsBuilt.Dequeue());
		}
	}

	void buildWalls() {
		foreach(GameObject building in sideBuildings) {
			Vector3 pos = new Vector3 (0,0,0);
			for(int i=0; i< 4; ++i) {
				pos = building.transform.position;
				switch(i) {
				case 0: pos.x -= spaceBetweenBuildings; break;
				case 1: pos.z -= spaceBetweenBuildings; break;
				case 2: pos.x += spaceBetweenBuildings; break;
				case 3: pos.z += spaceBetweenBuildings; break;
				}
				bool found = true;
				for(int j = 0; j< transform.childCount && found; ++j) {
					Vector3 posch = transform.GetChild(j).position;
					if(pos.x == posch.x && pos.z == posch.z) found = false;
				}
				if(found) {
					GameObject aux = selectionHighlight;
					switch(i) {
					case 0: 
						aux = (GameObject)Instantiate(walls[wallLevel-1], new Vector3(pos.x+walls[wallLevel-1].transform.position.x+spaceBetweenBuildings/2, 0, pos.z+walls[wallLevel-1].transform.position.z), Quaternion.Euler(new Vector3(0,90,0))); 		
					break;
					case 1:
						aux = (GameObject)Instantiate(walls[wallLevel-1], new Vector3(pos.x+walls[wallLevel-1].transform.position.x, 0, pos.z+walls[wallLevel-1].transform.position.z+spaceBetweenBuildings/2), Quaternion.Euler(new Vector3(0,0,0)));
					break;
					case 2: 
						aux = (GameObject)Instantiate(walls[wallLevel-1], new Vector3(pos.x+walls[wallLevel-1].transform.position.x-spaceBetweenBuildings/2, 0, pos.z+walls[wallLevel-1].transform.position.z), Quaternion.Euler(new Vector3(0,90,0)));
					break;
					case 3: 
						aux = (GameObject)Instantiate(walls[wallLevel-1], new Vector3(pos.x+walls[wallLevel-1].transform.position.x, 0, pos.z+walls[wallLevel-1].transform.position.z-+spaceBetweenBuildings/2), Quaternion.Euler(new Vector3(0,0,0)));
					break;
					}
					wallsBuilt.Enqueue(aux); 
					//AstarPath.active.UpdateGraphs(aux.transform.Find("1").collider.bounds); //TODO!!
					//AstarPath.active.UpdateGraphs(aux.transform.Find("2").collider.bounds); //TODO!!
				}
			}
		}

	}

}
