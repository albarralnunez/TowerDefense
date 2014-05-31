using UnityEngine;
using System.Collections;

public class WaveControll : MonoBehaviour {
	public int totalCreaturesPerWave = 15;
	private int _wave = 1;
	int livingCreatures = 0; 
	public float waitToWave = 2; 
	private float _waveTime;
	public GameObject yourCreature; 
	public Transform[] spawnPoints;
	public float waitTimeSpawnMinion;
	public bool waveOn = false;

	void Update() {
		livingCreatures = transform.childCount;
		if (waitToWave <= _waveTime && !waveOn) {	
			_waveTime = 0; //reset time
			waveOn = true;
			InvokeRepeating("Spawn",0,waitTimeSpawnMinion);
		}
		else if (livingCreatures >= totalCreaturesPerWave) {
			CancelInvoke();
			waveOn = false;
		}
		else {
			_waveTime += Time.deltaTime;
		}

	}

	void Spawn (){
		for (int j = 0; j < spawnPoints.Length ; ++ j) {
			GameObject go = (GameObject)Instantiate (yourCreature, spawnPoints[j].position, Quaternion.identity);
			go.transform.parent = gameObject.transform;
		}	
	}
}