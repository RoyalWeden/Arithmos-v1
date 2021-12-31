using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour {

    public static ChangeScene instance;

    private void Awake() {
        if(instance==null) {
            instance = this;
            DontDestroyOnLoad(instance);
        } else {
            Destroy(gameObject);
        }
    }

    public static void ChangeLoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
        if(sceneName == "MainMenu" || sceneName == "Options" || sceneName == "Shop" || sceneName == "Upgrades" || sceneName == "Achievements") {
            AdManager.ShowBanner();
        }
    }
}
