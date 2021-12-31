using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public AudioClip songClip;

    public static MusicManager instance;

    void Awake() {
        if(instance == null) {
            gameObject.AddComponent<SoundManager>();
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Update() {
        GetComponent<SoundManager>().PlayMusic(songClip);
    }
}
