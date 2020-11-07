using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassScript : MonoBehaviour
{
    public float foodValue = +44;
    public bool beingEaten = false;
    
    public void OnEat(){
        Destroy(gameObject);
    }
}
 