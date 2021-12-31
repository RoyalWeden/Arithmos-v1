using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour {

    public static SFXManager instance;

    void Start() {
        if(instance == null) {
            gameObject.AddComponent<SoundManager>();
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip sfxClip) {
        GetComponent<SoundManager>().PlaySoundEffect(sfxClip);
    }
}
