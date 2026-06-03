using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBlock_Finished : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other){
    	if(other.gameObject.tag=="Player"){
    		 GameManagerScript_Finished.instance.GameOver();
    	}
    }
}
