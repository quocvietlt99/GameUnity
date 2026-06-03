using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAndObstaclesManager_Finished : MonoBehaviour
{
    public static PlatformAndObstaclesManager_Finished instance;
	public GameObject ground;
	public float groundSingleBlockLength;
	public float speed;

    public GameObject[] obstacles;

    void Start()
    {
        if(PlatformAndObstaclesManager_Finished.instance==null){
            PlatformAndObstaclesManager_Finished.instance = this;
        }
    }

    void Update()
    {
        if(GameManagerScript_Finished.instance.gameRunning){
        	// For moving ground
            if(ground.transform.position.x >= -groundSingleBlockLength){
            	ground.transform.Translate(Vector3.left*Time.deltaTime*speed);
            }
            else{
            	ground.transform.position = new Vector3(0f,0f,0f);
            }
        }
    }

    public void SpawnNew(){
        Instantiate(obstacles[Random.Range(0,obstacles.Length)], new Vector3(Random.Range(2f,3.5f), -0.147f, 0f), Quaternion.identity);
    }
}
