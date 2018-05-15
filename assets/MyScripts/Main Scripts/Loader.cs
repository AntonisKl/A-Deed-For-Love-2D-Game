using UnityEngine;
using System.Collections;


public class Loader : MonoBehaviour
{
    public static GameObject gameManager;          //GameManager prefab to instantiate.
    public static GameObject soundManager; 

    void Awake()
    {   
        if (SoundManager.instance == null)
        {
            //Instantiate soundManager prefab
            soundManager = (GameObject)Instantiate(Resources.Load("Misc/SoundManager"));
        }
        //Check if a GameManager has already been assigned to static variable GameManager.instance or if it's still null
        if (GameManager.instance == null)
        {
            //Instantiate gameManager prefab
            gameManager = (GameObject)Instantiate(Resources.Load("Misc/GameManager"));
        }
    }
}