using UnityEngine;
using System.Collections;

public class GUIMenu : MonoBehaviour {

	enum State {Main, Start, HowTo , Credits};

	State state = State.Main;


	Rect buttonStart 	=  new Rect (100, 50, 200, 55);
	Rect buttonHowto 	=  new Rect (100, 120, 200, 55);
	Rect buttonCredits	=  new Rect (100, 190, 200, 55);
	Rect buttonExit 	=  new Rect (100, 260, 200, 55);
	Rect buttonBack		=  new Rect (100, 260, 200, 55);

	Rect text = new Rect (100, 50, 200, 260);
	Rect buttonSmallMap 	=  new Rect (100, 120, 95, 55);
	Rect buttonBigMap		=  new Rect (205, 120, 95, 55);

	// Use this for initialization
	void Start () {
		Screen.fullScreen = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		switch(state) {
		case State.Main:
			if(GUI.Button (buttonStart, "Start")) state = State.Start;
			if(GUI.Button (buttonHowto, "How To Play")) state = State.HowTo;
			if(GUI.Button (buttonCredits, "Credits"))state = State.Credits;
			if(GUI.Button (buttonExit, "Exit"))Application.Quit();
			break;
		case State.HowTo:
			GUI.Label (text, "You've got a castle to protect!\n\nThe zombies spawn in their castles and you " +
				"must destroy them before they can hurt you.\n\nBuild towns to spawn soldiers who will die for you, and to " +
			    "have money to build even more defences. \n\nKill all the zombies to win.\nGood luck!");
			if(GUI.Button (buttonBack, "Back")) state = State.Main;
			break;
		case State.Credits:
			GUI.Label (text,"Programmers: Daniel Albarral, Blai Samitier \n\nWe do not own any of the models, textures nor images of this game. All the credit goes to their respective authors."); 
			if(GUI.Button (buttonBack, "Back")) state = State.Main;				
			break;
		case State.Start:
			GUI.Label (text,"Choose the size of the map to begin!"); 
			if(GUI.Button (buttonSmallMap, "Small Map"))Application.LoadLevel("mapultrapeke");
			if(GUI.Button (buttonBigMap, "Big Map")) Application.LoadLevel("mappeke");
			if(GUI.Button (buttonBack, "Back")) state = State.Main;
			break;
		}
	}
}
