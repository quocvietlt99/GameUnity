using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectAndSpawnNew : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D other){
		Debug.Log(other.gameObject.name);
		if(other.gameObject.tag=="Obstacle" && other.isTrigger==false){
			Destroy(other.gameObject, 3f);
			PlatformAndObstaclesManager_Finished.instance.SpawnNew();
		}
	}
}
