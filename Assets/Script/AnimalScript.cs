using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalScript : MonoBehaviour
{
    public bool subject = false;

    public LayerMask animalMask;
    // ----------------------------------------
    // --------------- HUNGER -----------------
    // ---------------------------------------- 

    [Range(0,100)]
    public float hunger;
    float maxHunger;
    public float eatingTime;
    public LayerMask eatingMask;
    public HealthBar hungerBar;

    // ----------------------------------------
    
    
    // ----------------------------------------
    // -------------- MOVEMENT ----------------
    // ---------------------------------------- 

    public float speed;
    IEnumerator currentMoveCoroutine; // Cria um objeto IEnumerator para poder referenciar uma coroutine
    // ----------------------------------------


    // ----------------------------------------
    // --------------- MATING -----------------
    // ---------------------------------------- 
    public int sex; //0 female
                    //1 male
    public float matingMeter;
    float maxMatingMeter;
    public float gestationDuration;
    public float matingCallRange;
    public HealthBar matingBar;

    // ----------------------------------------


    // ----------------------------------------
    // -------------- BEHAVIOUR ---------------
    // ---------------------------------------- 
    public enum State{
        Idle,
        Hungry,
        Thirsty,
        Afraid,
        Horny
    }
    public enum Action{
        Nothing,
        MovingAround,
        SeekingFood,
        SeekingMate,
        Running,
        Chasing,
        Eating,
        Mating
    }
    public State myState;
    public Action myAction;
    // ----------------------------------------


    // ----------------------------------------
    // --------------- SIGHT -----------------
    // ---------------------------------------- 

    public float viewRadius;
    [Range(0,360)]
	public float viewAngle;
	public LayerMask obstacleMask;
	[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();
    // ----------------------------------------


    /*
        LIFE
    */

    public void BeBorn(){
        maxHunger = hunger;
        StartCoroutine ("VitalSigns",.5);
        StartCoroutine ("FindTargetsWithDelay", .1);
    }
    IEnumerator VitalSigns(float delay){
        while(true){
            hunger -= delay * 5;
            matingMeter += delay * 3;
            matingBar.SetValue(matingMeter);
            hungerBar.SetValue(hunger);
            if(matingMeter > (maxHunger - hunger))
            { 
                myState = State.Horny;
            }
            if(hunger  < 70){
                myState = State.Hungry;
            }else{
                myState = State.Idle;
            }
            if(hunger <= 0){
                Die();
            }
			yield return new WaitForSeconds (delay);
        }
      
    }
    void Die(){
        Destroy(gameObject);
    }

    /*
        SIGHT
    */
    public Vector3 LookForFood(){
        if(visibleTargets.Count > 0){
            if(visibleTargets[0] != null){
                return visibleTargets[0].position;
            }
        }
        return Vector3.zero;
    }
	IEnumerator FindTargetsWithDelay(float delay) {
		while (true) {
			yield return new WaitForSeconds (delay);
			FindVisibleTargets ();
		}
	}

	void FindVisibleTargets() {
		visibleTargets.Clear ();
		Collider[] targetsInViewRadius = Physics.OverlapSphere (transform.position, viewRadius, eatingMask);
        // print(targetsInViewRadius);
		for (int i = 0; i < targetsInViewRadius.Length; i++) {
			Transform target = targetsInViewRadius [i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			if (Vector3.Angle (transform.forward, dirToTarget) < viewAngle / 2) {
				float dstToTarget = Vector3.Distance (transform.position, target.position);

				if (!Physics.Raycast (transform.position, dirToTarget, dstToTarget, obstacleMask)) {
                    if(!target.gameObject.GetComponent<GrassScript>().beingEaten){
                        visibleTargets.Add (target);
                    }
				}
			}
		}
	}

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
		if (!angleIsGlobal) {
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),0,Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}

    

    /*
        MOVEMENT
    */
	public Vector3 RandomMove(){
        float moveRadius = viewRadius/2;
		float x = Random.Range(-moveRadius,moveRadius);
		float y = Random.Range(-Mathf.Sin(Mathf.Acos(x/moveRadius)),Mathf.Sin(Mathf.Acos(x/moveRadius))) * moveRadius;
		Vector3 wayPoint = Vector3.right * x + Vector3.forward * y;

		return wayPoint;
	}

    public void MoveTowards(Vector3 target){
        if(currentMoveCoroutine != null){
            StopCoroutine(currentMoveCoroutine);
        }
        currentMoveCoroutine = MoveTowardsTarget(target);
        StartCoroutine(currentMoveCoroutine);
    }

    IEnumerator MoveTowardsTarget(Vector3 target){
        transform.LookAt(target);
    	// float targetAngle = Mathf.Atan2(target.x, target.z) * Mathf.Rad2Deg;//pega o angulo sobre o plano xz em que esse vetor se encontra
        // print(targetAngle);
        // float turnSpeed = 20f;

        while(true){
            // if(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f){
            //     float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);// armazena um valor de rotação em direção ao angulo selecionado em uma determinada velocidade, no intervalo de tempo de 1 frame
            //     transform.eulerAngles = Vector3.up * angle;// rotaciona o guarda esse tanto
            // }
    		transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);// o guarda se movimenta até a posição do proximo waypoint
            // print(myState);
            if(transform.position == target){
                myAction = Action.Nothing;
                yield break;                
            }
            yield return null;
        }
        
    }

    public void StopMoving(){
        if(currentMoveCoroutine != null){
            StopCoroutine(currentMoveCoroutine);
        }
    }


    /*
        FEEDING
    */
  

    IEnumerator Eating(GameObject food){
        if(food.gameObject.GetComponent<GrassScript>().beingEaten){
            yield break;
        }
        food.gameObject.GetComponent<GrassScript>().beingEaten = true;
        float thisFoodValue = food.GetComponent<GrassScript>().foodValue;
        float initialHunger  = hunger;
        float t = 0.0f;
        float rate = 1/eatingTime;
        StopMoving();
        while(t<1 && hunger <= maxHunger){
            myAction = Action.Eating;
            hunger = Mathf.Lerp(initialHunger,initialHunger + thisFoodValue,t);
            t +=  rate * Time.deltaTime;
            // print(t); 
            yield return null;
        }

        if(hunger>maxHunger){
            hunger = maxHunger;
        }

    
        food.GetComponent<GrassScript>().OnEat();
        
        myAction = Action.Nothing;
        yield break;
    }

    /*
     MATING 
     
     */

    public void MakeCall(){
		Collider[] call = Physics.OverlapSphere (transform.position, matingCallRange, animalMask);
        // print(targetsInViewRadius);
	
    }

      private void OnTriggerEnter(Collider collider) {
        if(collider.tag == "Grass"){
            StartCoroutine(Eating(collider.gameObject));
        }
    }
}
