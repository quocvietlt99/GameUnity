using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject_Finished : MonoBehaviour
{
    public Vector3 dir;
    public float speed = 0.5f;

    void Start(){
    	dir.Normalize();
    }

    void Update()
    {
    	if(GameManagerScript_Finished.instance.gameOver==false){
    		transform.Translate(dir*Time.deltaTime*speed);    
    	}
    }
}
