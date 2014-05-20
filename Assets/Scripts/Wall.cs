using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

	public int life = 50;
	private int curLife;

	// Use this for initialization
	void Start () {
		curLife = life;
	}

	public void hit( int dmg) {
		curLife -= dmg;
		if(curLife <= 0) {
			gameObject.SetActive(false);
		}
	}
}
