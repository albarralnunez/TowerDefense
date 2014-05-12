using UnityEngine;
using System.Collections;

public class GUIMouse : MonoBehaviour {
	enum State {Town, Soldier, Castle ,Building, None};

	GameObject last, construction;
	Rect guiRect = new Rect (0,0,100,100);
	Rect mainGuiRect = new Rect(Screen.width-200, 0, 200,30);

	Castle castle;

	private State state;

	void Start () {
		GameObject go = (GameObject) GameObject.FindGameObjectWithTag("Castle");
		castle = (Castle) go.GetComponent("Castle");
		state = State.None;
		last = construction = null;
	}
	
	// Update is called once per frame
	void Update () {
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
					if(hit.collider.tag == "Terrain") construction.transform.position = new Vector3(hit.point.x, 	hit.point.y, hit.point.z);	
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
					} else if (hit.collider.name == "SoldierObj") {
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
						Vector3 aux = hit.point - last.transform.parent.position;
						last.transform.parent.Find ("defPos"). localPosition = aux;
						//last.transform.parent.Find ("defPos").Translate(hit.point);
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
		switch (state) {
			case State.Town:
				Town t = (Town)last.GetComponent ("Town");
				int ppl = t.getPeople ();
				int lvl = t.getLevel ();
				GUI.Box (guiRect, "TOWN   lvl." + lvl.ToString ());
				GUI.Label (new Rect (5, 20, 90, 30), t.getPeople () + " people");
				GUI.Label (new Rect (5, 40, 90, 30), " gold/s");
				if (GUI.Button (new Rect (5, 60, 90, 30), "Lvl+ Walls")) t.wallLevelUp ();
			break;
			case State.Soldier:
			break;
			case State.Castle:
				GUI.Box (guiRect, "Castle");
			GUI.Label (new Rect (5, 20, 90, 30), (castle.getLife()).ToString() + " / " + (castle.getTotalHP()).ToString());
				if (GUI.Button (new Rect (5, 60, 90, 30), "Build Town")){
					construction = castle.buildTown();
					if(construction != null) {
						state = State.Building;
						Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
						construction.transform.position = new Vector3(pos.x, 0, pos.z);
					}
				}
			break;
		}
	}
}

