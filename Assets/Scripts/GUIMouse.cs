using UnityEngine;
using System.Collections;

public class GUIMouse : MonoBehaviour {
	enum State {Town, Soldier, Castle ,Building, None};

	GameObject last, construction;
	Rect guiRect = new Rect (0,0,140,140);
	Rect mainGuiRect = new Rect(Screen.width-200, 0, 200,30);

	Rect attrRect = new Rect (5,20,130,45);
	Rect attr1Rect = new Rect (10, 20, 130, 22);
	Rect attr2Rect = new Rect (10, 40, 130, 22);

	Rect upgradesRect = new Rect (5,70,130,65);
	Rect upgrade1Rect = new Rect (10, 75, 120, 25);
	Rect upgrade2Rect = new Rect (10, 105, 120, 25);

	Rect tooltipRect = new Rect (145, 0, 130,85);
	Rect tooltipRect2 = new Rect (150, 25, 120,55);
	Castle castle;

	int numTowns =0;

	bool pause = false;
	CameraControl cam;
	private State state;

	void Start () {
		GameObject go = (GameObject) GameObject.FindGameObjectWithTag("Castle");
		GameObject camera = (GameObject) GameObject.FindGameObjectWithTag("MainCamera");
		cam = (CameraControl) camera.GetComponent("CameraControl");
		castle = (Castle) go.GetComponent("Castle");
		state = State.None;
		last = construction = null;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.Escape)) {
			pause = !pause;
			if(pause) {
				cam.enabled = false;
				Time.timeScale = 0;
			}
			else {
				cam.enabled = true;
				Time.timeScale = 1;
			}
		}
		if(!pause) {
			if(state == State.Building) {
				if (Input.GetMouseButtonDown (0)) {
					construction.SendMessage("startBuilding");
					construction = null;
					state = State.None;
				}
				else {
					Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					RaycastHit hit;
					if (Physics.Raycast (ray, out hit)) {
						if(hit.collider.tag == "Terrain") construction.transform.position = new Vector3(hit.point.x-10,0, hit.point.z-5);	
					}
				}
			}
			else if (Input.GetMouseButtonDown (0)) {
				if (!(Input.mousePosition.x <= guiRect.width && Screen.height - Input.mousePosition.y <= guiRect.height)) {
					Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					RaycastHit hit;
					if (Physics.Raycast (ray, out hit)) {
						UnMark ();
						if (hit.collider.tag == "Building") {
							last = (GameObject)hit.transform.parent.gameObject;
							last.SendMessage("setHighlight", true);
							state = State.Town;
						} else if (hit.collider.tag == "BuildingFloor") {
							last = (GameObject)hit.transform.parent.transform.parent.gameObject;
							last.SendMessage("setHighlight", true);
							state = State.Town;
						} else if (hit.collider.tag == "Soldier") {
							last = (GameObject)hit.transform.gameObject;
							last.SendMessage("setHighlight", true);
							state = State.Soldier;
						}else if (hit.collider.tag == "Castle") {
							last = (GameObject)hit.transform.gameObject;
							last.SendMessage("setHighlight", true);
							state = State.Castle;
						}else {
							last = null;
							state = State.None;
						}
					}
				}
			} else if (Input.GetMouseButtonDown (1)) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit)) {
					if (last != null) {
						if (last.GetComponent("SoldierAI") != null) {
							//Vector3 aux = hit.point - last.transform.parent.position;
							last.transform.parent.Find ("defPos").position = hit.point;
							//last.transform.parent.Find ("defPos").Translate(hit.point);
						}
					}
					if(hit.collider.tag == "Building") {
						Building b = (Building)hit.collider.gameObject.GetComponent("Building");
						b.hit (10);
					}
				}
			}
		}
	}

	private void UnMark() {
		if(last!= null) last.SendMessage("setHighlight",false);
	}

	void OnGUI () {
		GUI.Box (mainGuiRect, "Gold: " + castle.getGold () + "    People: " + castle.getPeople());
		if(pause) {
			GUI.Box (new Rect(0,0,Screen.width, Screen.height), "");
			GUI.Box (new Rect((Screen.width-200)/2,(Screen.height-150)/2,200, 150),"PAUSED");
			if (GUI.Button (new Rect((Screen.width-195)/2,(Screen.height-150)/2+40,193, 50), "Resume")){
				cam.enabled = true;
				Time.timeScale = 1;
				pause = false;
			}
			if (GUI.Button (new Rect((Screen.width-195)/2,(Screen.height-150)/2+95,193, 50), "Exit to Menu")){
				Time.timeScale = 1;
				Application.LoadLevel("menu");
			}
		}
		else {
			switch (state) {
			case State.Town:
				Town t = (Town)last.GetComponent ("Town");
				GUI.Box (guiRect, "TOWN");
				GUI.Box (attrRect,"");
				GUI.Label (attr1Rect, "Size: "+ t.getHouses() + " houses");
				GUI.Label (attr2Rect, "Income: "+ t.getIncome() +" gold/s");
				GUI.Box (upgradesRect,"");
				if (GUI.Button (upgrade1Rect, new GUIContent ("Upgrade Walls", "1"))) {
					if(castle.getGold() >= 2500*(t.wallLevel+1)) {
						castle.addGold(-2500*(t.wallLevel+1));
						t.wallLevelUp ();
					}
				}
				if(castle.getGold() < 2500*(t.wallLevel+1)) GUI.Box (upgrade1Rect,"");
			    if (GUI.Button (upgrade2Rect, new GUIContent ("Repair", "2"))) {
				if(castle.getGold() >= 500*t.getHousesDestroyed()) {
						castle.addGold(-500*t.getHousesDestroyed());
						t.rebuild();
					}	
				}
				if(!t.needsRepairing || castle.getGold() <500*t.getHousesDestroyed()) GUI.Box (upgrade2Rect,"");
				if(GUI.tooltip == "1") {
				GUI.Box (tooltipRect, "Cost: " + 2500*(t.wallLevel+1) + " gold");
					GUI.Label(tooltipRect2, "Your town walls will be upgraded. (+50Hp)");
				}
				else if(GUI.tooltip == "2") {
					GUI.Box (tooltipRect, "Cost: " + 500*t.getHousesDestroyed() +" gold");
					GUI.Label(tooltipRect2, "Repair all the destroyed houses in your town.");
				}
			break;
			case State.Soldier: 
				Pathfinding.SoldierAI sai = (Pathfinding.SoldierAI) last.GetComponent ("SoldierAI");
				GUI.Box (guiRect, "SOLDIER");
				GUI.Box (attrRect,"");
				GUI.Label (attr1Rect, (sai.getLife()).ToString() + " / " + (sai.getTotalHP()).ToString());
				GUI.Box (upgradesRect,"");
				if (GUI.Button (upgrade1Rect, new GUIContent ("Build Tower", "1"))){}
				if(GUI.tooltip == "1") {
					GUI.Box (tooltipRect, "Cost: 200 gold");
					GUI.Label(tooltipRect2, "The tower will shoot the enemies, but it needs a soldier inside");
				}
			break;
			case State.Castle:
				GUI.Box (guiRect, "CASTLE");
				GUI.Box (attrRect,"");
				GUI.Label (attr1Rect, "Hp: "+(castle.getLife()).ToString() + " / " + (castle.getTotalHP()).ToString());
				GUI.Label (attr2Rect, "Income: "+(castle.getGoldPerSec()).ToString() + " gold/s");
				GUI.Box (upgradesRect,"");
				if (GUI.Button (upgrade1Rect, new GUIContent ("Build Town", "1"))){
					if(castle.getGold() >= 1000+(5000*numTowns)) {
						castle.addGold(-1000+(5000*numTowns));
						construction = castle.buildTown();
						state = State.Building;
						Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
						construction.transform.position = new Vector3(pos.x, 0, pos.z);
						++numTowns;
					}
				}
				if(castle.getGold() < 1000+(5000*numTowns)) GUI.Box (upgrade1Rect,"");
				if (GUI.Button (upgrade2Rect, new GUIContent ("Upgrade Walls", "2"))){
					if(castle.getGold() >= 5000*castle.lvlWalls) {
						castle.addGold(-5000*castle.lvlWalls);
						castle.upgradeLife(200);
					}
				}
				if(castle.getGold() < 15000*castle.lvlWalls) GUI.Box (upgrade2Rect,"");
				if(GUI.tooltip == "1") {
					GUI.Box (tooltipRect, "Cost: " + (1000+(5000*numTowns)) +" gold");
					GUI.Label(tooltipRect2, "The town will spawn soldiers and make money over time.");
				}
				else if(GUI.tooltip == "2") {
					GUI.Box (tooltipRect, "Cost: " + 5000*castle.lvlWalls + " gold");
					GUI.Label(tooltipRect2, "Your castle walls will be stronger. (+200Hp)");
				}
			break;
			}
		}
	}
}

