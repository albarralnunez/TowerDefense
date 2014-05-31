using UnityEngine;
using System.Collections;
using Pathfinding.RVO;
using System;
using System.Collections.Generic;

namespace Pathfinding {
	[RequireComponent(typeof(Seeker))]
	public class SoldierAI : AIPath {
		enum State {fight,wait,walk};
			
		/** Animation component.
		 * Should hold animations "awake" and "forward"
		 */
		//public Animation animRun;
		//public Animation animAttack;
		private Animator anim;
		
		public float distanceAlert;
		/** Minimum velocity for moving */
		public float sleepVelocity = 0.4F;

		/** Speed relative to velocity with which to play animations */
		public float animationSpeed = 0.2F;

		/** Effect which will be instantiated when end of path is reached.
		 * \see OnTargetReached */
		public GameObject endOfPathEffect;

		private bool hunting = false;
		private State state = State.wait;
		private GameObject targetObj;
		private Transform defPosition;
		
		public int life;
		public int damage;
		public int attackSpeed;		
		private int curLife;

		public GameObject selectionHighlight;
		public GameObject defenseHighlight;
		public int alertHiglight;

		private float attackTime;

		private GameObject attacker;		

		public int getTotalHP() {return life;}

		public int getLife() { return curLife;}

		private List<GameObject> enemys;		

		private Transform posAnim;

		public void setHighlight(bool active) {
			selectionHighlight.SetActive(active);
			defenseHighlight.SetActive(active);
		}

		public void add(GameObject a) {
			enemys.Add(a);
		}

		public void erase(GameObject a) {
			enemys.Remove(a);
		}

		public new void Start () {
			curLife = life;
			posAnim = transform.FindChild("ChainMail_Knight");
			enemys = new List<GameObject>();
			selectionHighlight = 
				(GameObject)Instantiate (Resources.Load ("Highlight"), gameObject.transform.position - new Vector3(0.0F,0.8F,0.0F), Quaternion.identity);
			selectionHighlight.transform.parent = transform;
			selectionHighlight.transform.localScale = new Vector3(alertHiglight, 1, alertHiglight);
			selectionHighlight.SetActive(false);

			defenseHighlight = 
				(GameObject)Instantiate (Resources.Load ("HighlightDef"), transform.parent.Find ("defPos").transform.position - new Vector3(0.0F,0.8F,0.0F),Quaternion.identity);
			defenseHighlight.transform.parent = transform.parent.Find ("defPos");
			defenseHighlight.transform.localScale = new Vector3(3, 1, 3);
			defenseHighlight.SetActive(false);


			anim = gameObject.GetComponentInChildren<Animator>();			

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
		}	

		public override Vector3 GetFeetPosition ()
		{
			return tr.position;
		}

		private GameObject GetNearestTaggedObject(){
			// and finally the actual process for finding the nearest object:
			
			float nearestDistanceSqr = Mathf.Infinity;
			GameObject[] taggedGameObjects = GameObject.FindGameObjectsWithTag("Enemy"); 
			
			GameObject nearestObj = null;
			
			// loop through each tagged object, remembering nearest one found
			foreach (GameObject obj in taggedGameObjects) {
				Vector3 objectPos = obj.transform.position;
					float distanceSqr = (float)((objectPos - transform.position).sqrMagnitude);
				Toolbox toolbox = Toolbox.Instance;
				bool exists =  toolbox.EnemyBusy.Contains(obj.GetInstanceID());
				if (distanceSqr < nearestDistanceSqr && distanceSqr <= distanceAlert && !exists) {
					nearestObj = obj;
					nearestDistanceSqr = distanceSqr;
					toolbox.EnemyBusy.Add(obj.GetInstanceID());
					hunting = true;
				}
			}
			return nearestObj;
		}

		public void hit(int damage) {
			curLife -= damage;
		}


		public void  OnTrigerEnter(Collider other) {
			if(other.tag == "Enemy")
				state = State.fight;
				attacker = other.gameObject;
				//other.gameObject.SendMessage("SetToFight",gameObject);
		}

		public void SetToFight (GameObject a) {
			a.SendMessage("SetToFight", gameObject);
			state = State.fight;
			attacker = a;
		}
			
		protected new void Update () {

			if (state == State.wait) {
				if (!hunting) {
					targetObj = GetNearestTaggedObject();
					if (defPosition != gameObject.transform && targetObj == null)
							defPosition = transform.parent.Find ("defPos").transform;
					//GameObject.Find("Soldier/defPos").GetComponent<Transform>().transform;
					target = defPosition;
				} else {
					if (targetObj != null) {
						target = targetObj.transform;
					} else {
						hunting = false;
						target = defPosition;
						anim.SetFloat("Axis_Vertical",0);
					}
				}
				
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
				} else {
					velocity = Vector3.zero;
				}


				//Animation

				//Calculate the velocity relative to this transform's orientation
				Vector3 relVelocity = tr.InverseTransformDirection (velocity);
				relVelocity.y = 0;

				if (velocity.sqrMagnitude <= sleepVelocity * sleepVelocity) {
					//Fade out walking animation
					anim.SetFloat("Axis_Vertical",0);
				} 
				else {
					float speed = relVelocity.z;
					anim.SetFloat("Axis_Vertical",speed);// * animationSpeed);
					posAnim.position = transform.position;
				}
			}
			else if (state == State.fight) {
				attackTime += Time.deltaTime;
				anim.SetFloat("Axis_Vertical",0);
				if (target == null) {
					state = State.wait;
					anim.SetBool("RightMouseClick",false);
				}
				else if (attackSpeed <= attackTime) {
					attacker.SendMessage("hit",damage);
					attackTime = 0;
					anim.SetBool("RightMouseClick",true);
				}
			}
			if (life <= 0) {
				Toolbox toolbox = Toolbox.Instance;
				toolbox.EnemyBusy.Remove (targetObj.GetInstanceID ());
				enemys.Remove(targetObj);
				Destroy(transform.parent.gameObject);
			}
		}
		
	}
}