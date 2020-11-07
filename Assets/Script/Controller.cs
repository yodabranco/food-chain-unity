using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
 	public float speed = 7;
	Vector3 velocity;
	Rigidbody playerRigidbody;	

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent <Rigidbody>(); // Tenho que Aprender
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));// O vetor input, recebe os valores -1, 0, 1, retornados pelos eixos horizontais e verticais definidos no objeto
        Vector3 direction = input.normalized;// o Vetor input é dividido pelo seu modulo, para se ter o vetor direçao sobre o unit circle
        velocity = direction * speed; // O vetor direção é multiplicado pela speed, gerando o vetor velocity
    }

    void FixedUpdate()
    {
    	playerRigidbody.position += velocity * Time.fixedDeltaTime; // a posição do corpo recebe o vetor deslocamento, dado pelo vetor velocidade multiplicado pelo clock de update
    }

    // void OnTriggerEnter(Collider triggerCollider) // ao ser detectada a colisão com o objeto trigger, a variavel trigger collider recebe as informações do objeto
    // {   
    // 	if(triggerCollider.tag == "Grass")
    // 	{
    //         GrassScript grass = triggerCollider.gameObject.GetComponent<GrassScript>();
    //         print(grass.OnEat());
    //         Destroy(triggerCollider.gameObject);
    // 	}
    // }
}
