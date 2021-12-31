using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeOptions : MonoBehaviour {

    public Slider musicSlider;
    public Slider soundfxSlider;

    void Start() {
        musicSlider.value = SaveSystem.data.musicVolume;
        soundfxSlider.value = SaveSystem.data.soundfxVolume;
    }

    public void ChangeMusicVolume() {
        SaveSystem.data.musicVolume = musicSlider.value;
    }

    public void ChangeSoundFXVolume() {
        SaveSystem.data.soundfxVolume = soundfxSlider.value;
    }
}
