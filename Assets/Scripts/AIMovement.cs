using UnityEngine;
using System.Collections;

public class AIMovement : MonoBehaviour {

	Vector3 targetPos;
	Vector3 path;
	CharacterController cc;
	enum State {fight,walk};

	public float velocity = 15;
	public string target = "Castle";

	public GameObject blood;
	public GameObject healthBar;
	public GameObject body;
	public int life;
	public int damage;
	public int attackSpeed;
	private State state;

	private int curLife;
	private float attackTime;
	private GameObject attacker;
	private GameObject healthBarPart;
	private HealthBar hbar;
	Animator animator;

	// Use this for initialization
	void Start () {
		cc = (CharacterController) gameObject.GetComponent("CharacterController");
		targetPos = GameObject.FindGameObjectWithTag(target).transform.position;
		path = targetPos - transform.position;
		path.Normalize();
		path *= velocity;
		transform.rotation = Quaternion.LookRotation(path);

		healthBarPart = (GameObject)Instantiate(healthBar, new Vector3(transform.position.x, healthBar.transform.position.y, transform.position.z), healthBar.transform.rotation);
		healthBarPart.SetActive(false);
		hbar = (HealthBar) healthBarPart.GetComponent("HealthBar");
		curLife = life;
		attackTime = 0;
		state = State.walk;

		animator = (Animator) transform.FindChild("Mesh").GetComponent ("Animator");
	}
	
	// Update is called once per frame
	void Update () {
		if(state == State.walk) {
			healthBarPart.transform.position = new Vector3(transform.position.x, healthBar.transform.position.y, transform.position.z);
			cc.SimpleMove(path);
		}
		else if(state == State.fight){
			animator.enabled = false;
			attackTime += Time.deltaTime;
			if (attacker  == null) {
				state = State.walk;
				animator.enabled = true;
			}
			else if (attackSpeed <= attackTime) {
				attacker.SendMessage("hit",damage);
				attackTime = 0;
				if(attacker.tag == "Building" ) {
					Building b = (Building) attacker.GetComponent("Building");
					if(b.isDestroyed) {
						attacker = null;
						animator.enabled = true;
						state = State.walk;
					}
				}
			}
		}
	}

	public void hit(int damage) {
		curLife -= damage;
		healthBarPart.SetActive(true);
		hbar.setHP((float)curLife/(float)life);
		if(curLife<=0) {
			blood = (GameObject) Instantiate (blood, transform.position, blood.transform.rotation);
			Destroy (blood,5);
			Toolbox toolbox = Toolbox.Instance;
			toolbox.EnemyBusy.Remove (gameObject.GetInstanceID ());
			Destroy(healthBarPart);
			Instantiate (body, transform.position, transform.rotation);
			Destroy (gameObject);
		}
	}

	public int getTotalHP() {return life;}
	
	public int getLife() { return curLife;}

	//a la ke choque con un soldier, un building o un castle lo atacara
	void OnTriggerEnter(Collider col) {
		if(col.tag == "Soldier"||col.tag=="Building" || col.tag == "Castle" || col.tag == "Tower") {
			state = State.fight;
			attacker = col.gameObject;
		}
	}
}
