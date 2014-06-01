using UnityEngine;
using System.Collections;

public class AoE : MonoBehaviour {

	void OnTriggerEnter(Collider col) {
		if(col.tag == "Enemy") transform.parent.SendMessage("AttackEnemy", col.gameObject);
	}

	void OnTriggerExit(Collider col) {
		if(col.tag == "Enemy") transform.parent.SendMessage("StopAttackEnemy", col.gameObject);
	}
}

