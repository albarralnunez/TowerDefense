using UnityEngine;
using System.Collections;

public class GUIMouse : MonoBehaviour {

	GameObject last;
	Rect guiRect = new Rect (0,0,100,100);

	bool isHighlighted = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			if(!isHighlighted || !(Input.mousePosition.x<=guiRect.width && Screen.height-Input.mousePosition.y <= guiRect.height)){
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit)) {
					if(hit.collider.tag == "Building") {
						last = (GameObject) hit.transform.parent.gameObject;
						Town t = (Town)last.GetComponent("Town");
						t.setHighlight(true);
						isHighlighted = true;
					}
					else if(hit.collider.tag == "BuildingFloor") {
						last = (GameObject) hit.transform.parent.transform.parent.gameObject;
						Town t = (Town)last.GetComponent("Town");
						t.setHighlight(true);
						isHighlighted = true;
					}
					else if(isHighlighted){
						Town t = (Town) last.GetComponent("Town");
						t.setHighlight(false);
						isHighlighted = false;
					}
				}
			}
		}
	}

	void OnGUI () {
		if(isHighlighted){
			Town t = (Town)last.GetComponent("Town");
			int ppl = t.getPeople();
			int lvl = t.getLevel();
			GUI.Box (guiRect, "TOWN   lvl." + lvl.ToString());
			GUI.Label (new Rect (5, 20, 90, 30), t.getPeople() + " people");
			GUI.Label (new Rect (5, 40, 90, 30),  " gold/s");
			if (GUI.Button(new Rect(5,60,90,30),"Lvl+ Walls")) t.wallLevelUp();
		}
	}
}

