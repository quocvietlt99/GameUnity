using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinoScript_Finished : MonoBehaviour
{
	Rigidbody2D rb;
	public float jumpForceMultiplier = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
       if(Input.GetButtonDown("Jump") && Grounded()){
       		rb.AddForce(Vector2.up*jumpForceMultiplier, ForceMode2D.Impulse);
       } 
    }

    bool Grounded(){
    	RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 0.1f);
    	if(hit.collider!=null){
    		Debug.Log(hit.collider.gameObject.name);
    		return true;
    	}
    	else{
    		return false;
    	}

    }
}
