using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : AnimalScript
{
    float rabbitHunger = 100;
    float rabbitEatingTime = 2;
    float rabbitSpeed = 7;
    float rabbitViewAngle = 110;
    float rabbitViewRadius = 15;
    // float idleTime = 1.5f;
    // float lastMove = 0;

    Rigidbody rabbitRB;
    void Start()
    {
        rabbitRB = GetComponent<Rigidbody>();

        SetRabbitGenes();
        BeBorn();
        hungerBar.SetMaxValue(rabbitHunger);
    }

    void SetRabbitGenes(){
        
        sex = Random.Range(0,1);
        hunger = rabbitHunger;
        eatingTime = rabbitEatingTime;

        speed = rabbitSpeed;
        
        viewAngle = rabbitViewAngle;
        viewRadius = rabbitViewRadius;

    }

    // Update is called once per frame
    void Update()
    {
        if(subject){
            print("------");
            print(myState);
            print(myAction);
        }

        switch(myState){
            case State.Hungry:{
                if(myAction != Action.Eating)
                { 
                    if(myAction != Action.SeekingFood){
                        Vector3 closestFood = LookForFood();
                        myAction = Action.SeekingFood;
                        if(closestFood != Vector3.zero){
                            MoveTowards(closestFood);
                        }else{
                            MoveTowards(RandomMove());
                        }
                    }
                }
                break;
            }
            case State.Idle:{
                if(myAction == Action.Nothing){
                    myAction = Action.MovingAround;
                    MoveTowards(RandomMove());
                }
                break;
            }
        }
    }
}
