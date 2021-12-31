using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {

    AudioSource audioS;

    AudioMixer mixer;

    private void Start() {
        gameObject.AddComponent<AudioSource>();
        audioS = GetComponent<AudioSource>();
        mixer = Resources.Load("Sounds/MainMixer") as AudioMixer;
    }

    public void PlaySoundEffect(AudioClip clip) {
        audioS.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        audioS.volume = SaveSystem.data.soundfxVolume;
        audioS.clip = clip;
        audioS.Play();
    }

    public void PlayMusic(AudioClip clip) {
        audioS.outputAudioMixerGroup = mixer.FindMatchingGroups("Music")[0];
        audioS.volume = SaveSystem.data.musicVolume;
        audioS.clip = clip;
        if(!audioS.isPlaying) {
            audioS.Play();
        }
    }
}
