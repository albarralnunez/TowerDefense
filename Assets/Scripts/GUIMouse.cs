using UnityEngine;
using System.Collections;

public class GUIMouse : MonoBehaviour {
	enum State {Town, Soldier,None};

	GameObject last;
	Rect guiRect = new Rect (0,0,100,100);
	private int state;
	//bool isHighlighted = false;
	// Use this for initialization
	void Start () {
		state = (int)State.None;
		last = new GameObject ();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			if(!(Input.mousePosition.x<=guiRect.width && Screen.height-Input.mousePosition.y <= guiRect.height)){
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit)) {
					UnMark();
					if(hit.collider.tag == "Building") {
						last = (GameObject) hit.transform.parent.gameObject;
						Town t = (Town)last.GetComponent("Town");
						t.setHighlight(true);
						//isHighlighted = true;
						state = (int)State.Town;
					}
					else if(hit.collider.tag == "BuildingFloor") {
						last = (GameObject) hit.transform.parent.transform.parent.gameObject;
						Town t = (Town)last.GetComponent("Town");
						t.setHighlight(true);
						//isHighlighted = true;
						state = (int)State.Town;
					}
					else if (hit.collider.name == "SoldierObj") {
						last = (GameObject) hit.transform.gameObject;
						Pathfinding.SoldierAI s = (Pathfinding.SoldierAI)last.GetComponent("SoldierAI");
						s.setHighlight(true);
						//isHighlighted = true;
						state = (int)State.Soldier;
					}
					else {
						state = (int)State.None;
					}
				}
			}
		}
	}

	private void UnMark() {
		//Unmark
		if (last.GetComponent("SoldierAI") != null) {
			Pathfinding.SoldierAI s = (Pathfinding.SoldierAI)last.GetComponent("SoldierAI");
			s.setHighlight(false);
		}
		if (last.GetComponent("Town") != null) {
			Town t = (Town)last.GetComponent("Town");
			t.setHighlight(false);
		}
	}

	void OnGUI () {
		switch (state) {
			case (int)State.Town:
				Town t = (Town)last.GetComponent ("Town");
				int ppl = t.getPeople ();
				int lvl = t.getLevel ();
				GUI.Box (guiRect, "TOWN   lvl." + lvl.ToString ());
				GUI.Label (new Rect (5, 20, 90, 30), t.getPeople () + " people");
				GUI.Label (new Rect (5, 40, 90, 30), " gold/s");
				if (GUI.Button (new Rect (5, 60, 90, 30), "Lvl+ Walls"))
				t.wallLevelUp ();
			break;
			case (int)State.Soldier:

			break;
		}
	}
}

