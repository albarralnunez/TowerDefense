using UnityEngine;
using System.Collections;
using Pathfinding.RVO;

namespace Pathfinding {
	/** AI controller specifically made for the spider robot.
	 * The spider robot (or mine-bot) which is got from the Unity Example Project
	 * can have this script attached to be able to pathfind around with animations working properly.\n
	 * This script should be attached to a parent GameObject however since the original bot has Z+ as up.
	 * This component requires Z+ to be forward and Y+ to be up.\n
	 * 
	 * It overrides the AIPath class, see that class's documentation for more information on most variables.\n
	 * Animation is handled by this component. The Animation component refered to in #anim should have animations named "awake" and "forward".
	 * The forward animation will have it's speed modified by the velocity and scaled by #animationSpeed to adjust it to look good.
	 * The awake animation will only be sampled at the end frame and will not play.\n
	 * When the end of path is reached, if the #endOfPathEffect is not null, it will be instantiated at the current position. However a check will be
	 * done so that it won't spawn effects too close to the previous spawn-point.
	 * \shadowimage{mine-bot.png}
	 * 
	 * \note This script assumes Y is up and that character movement is mostly on the XZ plane.
	 */
	[RequireComponent(typeof(Seeker))]
	public class MineBotAI : AIPath {

		enum State {fight,walk};

		/** Animation component.
		 * Should hold animations "awake" and "forward"
		 */
		//public Animation anim;
		public float distanceAlert = 20.0F;
		/** Minimum velocity for moving */
		public float sleepVelocity = 0.4F;
			
		/** Speed relative to velocity with which to play animations */
		//public float animationSpeed = 0.2F;
		
		/** Effect which will be instantiated when end of path is reached.
		 * \see OnTargetReached */
		public GameObject endOfPathEffect;

		public GameObject healthBar;
		public int life;
		public int damage;
		public int attackSpeed;
		private State state;

		private int curLife;
		private float attackTime;
		private GameObject attacker;
		private GameObject healthBarPart;
		private HealthBar hbar;
		public Animator anim;
		

		public void SetToFight(GameObject a) {
			state = State.fight;
			anim.SetBool("Fight",true);
			attacker = a;
		}

		public void hit(int damage) {
			curLife -= damage;
			healthBarPart.SetActive(true);
			hbar.setHP((float)curLife/(float)life);
			if(curLife<=0) {
				Toolbox toolbox = Toolbox.Instance;
				toolbox.EnemyBusy.Remove (gameObject.GetInstanceID ());
				Destroy(healthBarPart);
<<<<<<< HEAD
				Instantiate (body, transform.position, body.transform.rotation);
=======
>>>>>>> f300176d3b07d5e7729dbc2ff9d8092638ec8f77
				Destroy (gameObject);
			}
		}

		public int getTotalHP() {return life;}
		
		public int getLife() { return curLife;}

		public new void Start () {
			healthBarPart = (GameObject)Instantiate(healthBar, new Vector3(transform.position.x, healthBar.transform.position.y, transform.position.z), healthBar.transform.rotation);
			healthBarPart.SetActive(false);
			hbar = (HealthBar) healthBarPart.GetComponent("HealthBar");


			curLife = life;
			attackTime = 0;
			state = State.walk;

			target = GameObject.Find("Target").transform;

			//Call Start in base script (AIPath)
			base.Start ();
		}
		
		/** Point for the last spawn of #endOfPathEffect */
		protected Vector3 lastTarget;
		
		/**
		 * Called when the end of path has been reached.
		 * An effect (#endOfPathEffect) is spawned when this function is called
		 * However, since paths are recalculated quite often, we only spawn the effect
		 * when the current position is some distance away from the previous spawn-point
		*/
		public override void OnTargetReached () {
			
			if (endOfPathEffect != null && Vector3.Distance (tr.position, lastTarget) > 1) {
				GameObject.Instantiate (endOfPathEffect,tr.position,tr.rotation);
				lastTarget = tr.position;
			}
			Toolbox toolbox = Toolbox.Instance;
			toolbox.EnemyBusy.Remove (gameObject.GetInstanceID ());
			Destroy (gameObject);
		}	

		public override Vector3 GetFeetPosition ()
		{
			return tr.position;	
		}
	
		protected new void Update () {
			healthBarPart.transform.position = new Vector3(transform.position.x, healthBar.transform.position.y, transform.position.z);
			if (state == State.walk) {
				//if (target ==  )target = 
				//Get velocity in world-space
				Vector3 velocity;
				if (canMove) {
				
					//Calculate desired velocity
					Vector3 dir = CalculateVelocity (GetFeetPosition ());

					//Rotate towards targetDirection (filled in by CalculateVelocity)
					RotateTowards (targetDirection);

					dir.y = 0;
					if (dir.sqrMagnitude > sleepVelocity * sleepVelocity) {
							//If the velocity is large enough, move
					} else {
							//Otherwise, just stand still (this ensures gravity is applied)
							dir = Vector3.zero;
					}

					if (navController != null) {
					} else if (controller != null)
							controller.SimpleMove (dir);
					else
							Debug.LogWarning ("No NavmeshController or CharacterController attached to GameObject");

					velocity = controller.velocity;
				} 
				else {
					velocity = Vector3.zero;
				}
			} 
			else if (state == State.fight) {
				attackTime += Time.deltaTime;
				if (attacker  == null) {
					state = State.walk;
					anim.SetBool("Fight",false);
				}
				else if (attackSpeed <= attackTime) {
					attacker.SendMessage("hit",damage);
					attackTime = 0;
/*
					if(attacker.tag = "building" ) {
						Building b = (Building) attacker.GetComponent("Building");
						if(b.isBuildingDestroyed()) {
							attacker = null;
							state = State.walk;
						}
					}
*/
				}
			}

			if (life <= 0) {
				Toolbox toolbox = Toolbox.Instance;
				toolbox.EnemyBusy.Remove (gameObject.GetInstanceID ());
				Destroy(gameObject);
			}
		}

		//a la ke choque con un soldier, un building o un castle lo atacara
		void OnTriggerEnter(Collider col) {
			if(col.tag=="Building" || col.tag == "Castle") {
				SetToFight (col.gameObject);
			}
		}

	}
}