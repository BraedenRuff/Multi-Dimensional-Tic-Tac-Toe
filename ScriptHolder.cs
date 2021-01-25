using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptHolder : MonoBehaviour
{
    /// <summary>Static reference to the instance of our DataManager</summary>
    public static ScriptHolder instance;


    public AudioClip backgroundMusic;
    public Sprite backgroundPicture;

    /// <summary>Awake is called when the script instance is being loaded.</summary>
    void Awake()
    {
        // If the instance reference has not been set, yet, 
        if (instance == null)
        {
            
            // Set this instance as the instance reference.
            instance = this;
        }
        
        else if (instance != this)
        {
            // If the instance reference has already been set, and this is not the
            // the instance reference, destroy this game object.
            SetBackGroundMusic(instance.backgroundMusic);
            SetBackGroundPicture(instance.backgroundPicture);
            Destroy(instance.gameObject);
            instance = this;
        }
        

        // Do not destroy this object, when we load a new scene.
        DontDestroyOnLoad(gameObject);
    }

    public void SetBackGroundMusic(AudioClip clip)
    {
        backgroundMusic = clip;
    }
    public void SetBackGroundPicture(Sprite picture)
    {
        backgroundPicture = picture;
    }
}