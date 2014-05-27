using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {

	public GameObject bar;


	public void setHP(float	lifePercent) {
		bar.transform.localScale = new Vector3(lifePercent, 1,1);
	}
}
