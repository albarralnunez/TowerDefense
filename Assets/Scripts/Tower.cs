using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour {

	public float timeBuild = 20;
	public GameObject smoke;
	public GameObject ruins;
	public float ysize = 80;
	public GameObject healthBar;
	public int life = 30;
	public int dmg = 10;
	private int curLife;
	public float dps = 1.0f;
	float count =0;
	
	public bool isDestroyed, isHit;
	
	HealthBar hbar;
	
	Castle castle;
	GameObject smokePart;
	GameObject floorPart;
	GameObject ruinsPart;
	GameObject healthBarPart;
	GameObject enemyAttacking = null;
	bool attacking = false;
	LineRenderer line;
	// Use this for initialization
	void Start () {
		isHit = isDestroyed = false;
		curLife = life;
	   
		line = (LineRenderer) gameObject.GetComponent("LineRenderer");
		line.SetPosition(0, new Vector3(transform.position.x+10, 5, transform.position.z+5));
		line.enabled = false;

		healthBarPart = (GameObject)Instantiate(healthBar, new Vector3(transform.position.x+10, healthBar.transform.position.y, transform.position.z+5), healthBar.transform.rotation);
		healthBarPart.transform.parent = transform;
		healthBarPart.SetActive(false);
		hbar = (HealthBar) healthBarPart.GetComponent("HealthBar");
		transform.position = new Vector3(transform.position.x, transform.position.y-ysize, transform.position.z);
		smokePart = (GameObject)Instantiate(smoke, new Vector3(transform.position.x+smoke.transform.position.x,smoke.transform.position.y,transform.position.z+smoke.transform.position.z), smoke.transform.rotation);
		transform.collider.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(count<timeBuild) { 
			transform.position = new Vector3(transform.position.x, transform.position.y + (ysize/timeBuild)*Time.deltaTime, transform.position.z);
			count += Time.deltaTime;
			if(count>=timeBuild) {
				ParticleSystem sys = (ParticleSystem)smokePart.GetComponent("ParticleSystem");
				sys.Stop();
				Destroy(smokePart, 5);
				transform.collider.enabled = true;
				transform.FindChild("Highlight").gameObject.collider.enabled = true;
				transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			}
		}
	}
	
	public void hit( int dmg) {
		curLife -= dmg;
		isHit = true;
		healthBarPart.SetActive(true);
		hbar.setHP((float)curLife/(float)life);
		if(curLife <= 0) {
			isHit = false;
			isDestroyed = true;
			healthBarPart.SetActive(false);
			gameObject.renderer.enabled = false;
			gameObject.collider.enabled = false;
			ruinsPart = (GameObject) Instantiate(ruins, transform.position, ruins.transform.rotation);
			ruinsPart.transform.parent = transform;
		}
	}

	public void setHighlight(bool active) {
		transform.FindChild("Highlight").gameObject.renderer.enabled = active;
	}

	public void rebuild() {
		for(int i=0; i< transform.childCount; ++i) {
			Destroy(transform.GetChild(i).gameObject);
		}
		gameObject.renderer.enabled = true;
		gameObject.collider.enabled = true;
		Start ();
	}
	
	public void heal(int dmg) {
		curLife +=dmg;
		hbar.setHP((float)curLife/(float)life);
		if(curLife >= life) {
			curLife = life;
			healthBarPart.SetActive(false);
			isHit = false;
		}
	}
	
	public void upgradeLife(int l) {
		life += l;
		curLife += l;
		hbar.setHP((float)curLife/(float)life);
	}

	void AttackEnemy(GameObject enemy) {
		if(!attacking) {
			enemyAttacking = enemy;
			InvokeRepeating("attack", 0, dps);
			attacking = true;
		}
	}

	void attack() {
		if(enemyAttacking == null)  {
			CancelInvoke();		
			attacking = false;
			enemyAttacking = null;
		}
		else {
			line.enabled = true;
			line.SetPosition(1, enemyAttacking.transform.position);
			Pathfinding.MineBotAI enem = (Pathfinding.MineBotAI) enemyAttacking.GetComponent("MineBotAI");
			Invoke ("disableLine", 0.05f);
			enem.hit(dmg);
		}
	}
	void disableLine() {
		line.enabled = false;
	}

	void StopAttackEnemy(GameObject enemy) {
		if(enemy.transform == enemyAttacking.transform) {
			CancelInvoke();		
			attacking = false;
			enemyAttacking = null;
		}
	}	
}


