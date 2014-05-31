using UnityEngine;
using System.Collections.Generic;


public class Town : MonoBehaviour {

	enum State {Building, Attacking, Repairing, Waiting, Destroyed};

	public GameObject[] buildings;
	public GameObject[] walls;
	int wallUpgrade = 50;
	public GameObject selection;
	public GameObject floor;

	public float spaceBetweenBuildings = 15;
	public float secondsToBuild = 10;
	public int people = 5;
	public int radius = 100;

	public float secondsToRepair = 1;
	public int healPower = 10;

	public int[] levelReq;
	Queue<GameObject> sideBuildings;
	Queue<GameObject> wallsBuilt;

	public bool needsRepairing = false;
	int numhouses = 0;
	float countAttack = 0;
	int level =1;
	public int wallLevel =0;
	float timecont =0;
	GameObject selectionHighlight;
	int goldPerSec=0;
	State state;

	// Use this for initialization
	void Start () {
		state = State.Waiting;
		sideBuildings= new Queue<GameObject>();
		wallsBuilt= new Queue<GameObject>();
	}

	void Awake () {
		floor = (GameObject) Instantiate (floor, transform.position, transform.rotation);
		floor.transform.parent = transform;
		floor.collider.enabled = false;	
	}

	public int getHousesDestroyed() {
		return transform.childCount-numhouses;
	}

	public void startBuilding() {
		Destroy(floor);
		int indx = (int)Random.Range(0,buildings.Length);
		++numhouses;
		selectionHighlight = (GameObject) Instantiate(selection, transform.position, transform.rotation);
		//selectionHighlight.transform.parent = gameObject.transform;
		selectionHighlight.transform.localScale = new Vector3(radius, 1, radius);
		selectionHighlight.SetActive(false);
		GameObject building = (GameObject) Instantiate(buildings[indx], new Vector3(transform.position.x-10, transform.position.y, transform.position.z-5), buildings[indx].transform.rotation);
		building.transform.parent = transform;
//		AstarPath.active.UpdateGraphs (building.collider.bounds,5); //TODO!
		sideBuildings.Enqueue(building);
		state = State.Building;
		Building bld = (Building)building.GetComponent("Building");
		goldPerSec = bld.goldPerSec; 
	}

	void Update () {
		if(state == State.Building) {
			timecont += Time.deltaTime;
			if(timecont>= secondsToBuild) {
				Queue<GameObject> sideBuildingsAux = new Queue<GameObject>();
				timecont = 0;
				Vector3 pos = new Vector3(0,0,0);
				bool found = false;
				while(!found && sideBuildings.Count>0) {
					bool obstacle =false;
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
						if(found && !canBuild(pos)){
							found =false;
							obstacle = true;
						}
					}
					if(!found) {
						if(obstacle)sideBuildingsAux.Enqueue(sideBuildings.Peek());
						sideBuildings.Dequeue();
					}
					else {
						int b = Random.Range(0,buildings.Length);
						++numhouses;
						GameObject building = (GameObject) Instantiate(buildings[b], new Vector3(pos.x, 0, pos.z), buildings[b].transform.rotation);
						building.transform.parent = transform;
						//AstarPath.active.UpdateGraphs (building.collider.bounds,5); //TODO:!!!
						sideBuildings.Enqueue(building);
						if(transform.childCount-2 > levelReq[level-1]) {
							++level;
							radius += 50;
							selectionHighlight.transform.localScale = new Vector3(radius, 1, radius);
							if(level >=4) state = State.Waiting;
						}
					}
				}
				for(int i=0; i<sideBuildingsAux.Count;++i) sideBuildings.Enqueue(sideBuildingsAux.Dequeue());
				if(wallLevel>0) {
					destroyWalls();
					buildWalls();
				}
			}
		}
		else if(state == State.Repairing) {
			timecont += Time.deltaTime;
			if(timecont>= secondsToRepair) {
				timecont =0;
				bool found = false;
				for(int i=0; i< transform.childCount && !found; ++i) {
					Building b = (Building) transform.GetChild(i).gameObject.GetComponent("Building");
					if(b.isHit) {
						b.heal (healPower);
						found = true;
					}
				}
				if (!found) state = State.Building;
			}
		}
		else if(state == State.Attacking) {
			countAttack += Time.deltaTime;
			if(countAttack >= 10) state= State.Repairing;
		}
	}

	bool canBuild(Vector3 pos){
		pos = new Vector3(pos.x, pos.y+500, pos.z);
		Vector3 down = transform.TransformDirection(Vector3.down);
		RaycastHit hit;
		Debug.DrawRay(pos, down*1000, Color.white, 20f, true);
		if (Physics.Raycast(pos, down, out hit)){
			if(hit.point.y > 0)return false;
		}
		return true;
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

	public void buildingDestroyed() {
		--numhouses;
		needsRepairing = true;
		if(numhouses<=0) state= State.Destroyed;
	}

	public int getRadius() {
		return radius;
	}

	public int getHouses() {
		if(state == State.Building) return numhouses-1;
		else return numhouses;
	}

	public int getIncome() {
		if(state == State.Building) return (numhouses-1)*goldPerSec;
		else return numhouses*goldPerSec;
	}

	public void wallLevelUp() {
		++wallLevel; 
		if(wallLevel>walls.Length) wallLevel = walls.Length;
		for(int i=0; i< transform.childCount; ++i) {
			Building b = (Building) transform.GetChild(i).gameObject.GetComponent("Building");
			b.upgradeLife(wallUpgrade);
		}
		destroyWalls ();
		buildWalls ();
	}

	void destroyWalls() {
		while(wallsBuilt.Count>0) {
			Destroy (wallsBuilt.Dequeue());
		}
	}

	public void rebuild() {
		for(int i=0; i< transform.childCount; ++i) {
			Building b = (Building) transform.GetChild(i).gameObject.GetComponent("Building");
			if(b.isDestroyed) {
				++numhouses;
				b.rebuild();
			}
		}
		needsRepairing = false;
		state = State.Building;
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
