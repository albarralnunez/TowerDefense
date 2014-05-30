using UnityEngine;
using System.Collections;

public class WaveControll : MonoBehaviour {
	int totalCreaturesPerWave = 5;
	private int _wave = 1;
	int livingCreatures = 0; 
	float waitToWave = 2; 
	private float _waveTime;
	public Transform yourCreature; 
	public Transform[] spawnPoints;
	public float waitTimeSpawnMinion;
	private int _jAux;

	void Update() {
		if (livingCreatures == 0) {	
			if (waitToWave <= _waveTime) {	
				_waveTime = 0; //reset time
				livingCreatures = totalCreaturesPerWave;
				for (int i = 0; i < totalCreaturesPerWave; i++){
					for (int j = 0; j < spawnPoints.Length ; ++ j) {
						_jAux = j;
						Invoke("Spawn", 1);
					}
				}
			}
		} else {
			_waveTime += Time.deltaTime; 
		}
	}

	void Spawn (){
		Instantiate (yourCreature, spawnPoints[_jAux].position, Quaternion.identity); 
	}
}