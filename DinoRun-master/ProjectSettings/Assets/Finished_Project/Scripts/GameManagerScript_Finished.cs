using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerScript_Finished : MonoBehaviour
{
	public static GameManagerScript_Finished instance;
    public PlatformAndObstaclesManager_Finished platformScript;
	public GameObject groundGameObject;
    public GameObject player;

	private float score = 0;
	public Text scoreText;
    public GameObject gameStartText;
    public GameObject gameOverText;
    [HideInInspector]
	public bool gameOver = false;
	[HideInInspector]
    public bool gameRunning = false;

    void Start()
    {
    	if(GameManagerScript_Finished.instance==null){
    		GameManagerScript_Finished.instance = this;
            player.GetComponent<Animator>().enabled = false;
    	}
    }

    void Update()
    {	
        if(gameRunning==false){
            if(Input.anyKey){
                gameRunning = true;
                gameStartText.SetActive(false);
                player.GetComponent<Animator>().enabled = true;
                PlatformAndObstaclesManager_Finished.instance.SpawnNew();
            }
        }
        else{
            if(gameOver!=true){
            	score += Time.deltaTime*10;
            	scoreText.text = ((int)score).ToString();
            }
            else{
                if(Input.GetKeyDown(KeyCode.R)){
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
        } 
    }

    public void GameOver(){
    	gameOver = true;
        platformScript.enabled = false;
        player.GetComponent<Animator>().enabled = false;
        gameOverText.SetActive(true);
    }
}
