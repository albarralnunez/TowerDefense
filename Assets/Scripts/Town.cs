using UnityEngine;
using System.Collections;


public class Town : MonoBehaviour {

	public GameObject[] buildings;
	public float spaceBetweenBuildings = 70;
	public float secondsToBuild = 10;
	public int initialVillagers = 5;


	int level =1;
	float timecont =0;

	// Use this for initialization
	void Start () {
		int indx = Random.Range(0,buildings.Length);
		GameObject building = (GameObject) Instantiate(buildings[indx], transform.position, buildings[indx].transform.rotation);
		building.transform.parent = transform;
	}
	
	// Update is called once per frame
	void Update () {
		timecont += Time.deltaTime;
		if(timecont>= secondsToBuild) {
			timecont = 0;
			bool found = false;
			int rnd = Random.Range(0,transform.childCount);
			Vector3 pos = Vector3.zero;
			while(!found) {
				int indx = Random.Range (0,4);
				for(int i=0; i< 4; ++i) {
					pos = transform.GetChild(rnd).position;
					switch(indx) {
						case 0: pos.x += spaceBetweenBuildings; break;
						case 1: pos.z -= spaceBetweenBuildings; break;
						case 2: pos.x += spaceBetweenBuildings; break;
						case 3: pos.z -= spaceBetweenBuildings; break;
					}
					indx++; if(indx>3) indx =0;
					found =true;
					for(int j = 0; j< transform.childCount && found; ++j) {
						Vector3 posch = transform.GetChild(j).position;
						if(j!= rnd && pos.x == posch.x && pos.z == posch.z) found = false;
					}
				}
				++rnd; if(rnd> transform.childCount) rnd =0;
			}
			int b = Random.Range(0,buildings.Length);
			GameObject building = (GameObject) Instantiate(buildings[b], new Vector3(pos.x, 0, pos.z), buildings[b].transform.rotation);
			building.transform.parent = transform;
		}
	}
}
