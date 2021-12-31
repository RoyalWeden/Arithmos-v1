using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {

    public bool doFirstLoad;
    public bool startFullUnlocks;
    public static bool loadedFirstScene;
    public static bool loadedFirstSceneAfter;

    [Space(10)]
    public bool enableRegularAds;
    public bool enableRewardedAds;
    public static bool regularAdsEnabled;
    public static bool rewardedAdsEnabled;

    public static int gamesForAd;

    int countTouches;

    public static List<int> starAmountSum;
    public static int starAmountTotalSum;

    public static string[] unlockNames;
    public static int[] unlockStarsNeeded;
    public static int[] unlockCoinsNeeded;
    public static int[] unlockSectionStarsNeeded;
    public static int[] buySectionCoinsNeeded;

    public static string previousSelectScene;

    public static bool[] activateUnlocks;

    public static SFXManager sfxmanager;
    AudioClip buttonPressSFX;

    GameObject selectedObj;

    private void Awake() {
        #region First Load, Normal Loads, Set Values

        if(!loadedFirstScene) {

            if(doFirstLoad) {
                PlayerPrefs.DeleteAll();
            }

            unlockSectionStarsNeeded = new int[] { 0, 25, 40, 55, 70, 85, 100 };
            buySectionCoinsNeeded = new int[] { 0, 5, 10, 15, 25, 40, 60 };

            unlockNames = new string[] { "Unlocked Upgrades Menu", "Unlocked Hint Upgrade", "Unlocked Magnet Upgrade",
                               "Unlocked Freeze Upgrade", "Unlocked Fire Upgrade", "Unlocked Blackhole Upgrade",
                                "Unlocked Scissors Upgrade", "Unlocked Addition Section", "Unlocked Subtraction Section", "Unlocked Multiplication Section", "Unlocked Division Section", "Unlocked Powers Section", "Unlocked Equations Section" };
            activateUnlocks = new bool[] { true, true, true, true, true, true, true, false, false, false, false, false, false };
            unlockStarsNeeded = new int[] { 20, 35, 75, 110, 160, 225, 300, unlockSectionStarsNeeded[1], unlockSectionStarsNeeded[2], unlockSectionStarsNeeded[3], unlockSectionStarsNeeded[4], unlockSectionStarsNeeded[5], unlockSectionStarsNeeded[6] };
            unlockCoinsNeeded = new int[] { 0, 0, 0, 0, 0, 0, 0, buySectionCoinsNeeded[1], buySectionCoinsNeeded[2], buySectionCoinsNeeded[3], buySectionCoinsNeeded[4], buySectionCoinsNeeded[5], buySectionCoinsNeeded[6] };

            gamesForAd = Random.Range(2, 5);
            if(!SaveSystem.data.removedAds) {
                regularAdsEnabled = enableRegularAds;
            } else {
                regularAdsEnabled = false;
            }
            rewardedAdsEnabled = enableRewardedAds;

            previousSelectScene = null;

            SceneManager.LoadScene("MainMenu");
            loadedFirstScene = true;

            if(!PlayerPrefs.HasKey("firstload")) {
                PlayerPrefs.SetInt("firstload", 0);

                // default values
                SaveSystem.data.curStartLvlChoice = 1;
                SaveSystem.data.maxLevel = 9 * 5;

                starAmountSum = new List<int>();
                for(int i = 0; i < 7; i++) {
                    SaveSystem.data.starAmount.Add(new List<int>());
                    starAmountSum.Add(0);
                    for(int j = 0; j < SaveSystem.data.maxLevel; j++) {
                        if(!startFullUnlocks) {
                            SaveSystem.data.starAmount[i].Add(0);
                        } else {
                            SaveSystem.data.starAmount[i].Add(3);
                        }
                        starAmountSum[i] += SaveSystem.data.starAmount[i][j];
                    }
                    starAmountTotalSum += starAmountSum[i];
                }

                if(!startFullUnlocks) {
                    SaveSystem.data.coins = 0;
                } else {
                    SaveSystem.data.coins = 100000;
                }

                SaveSystem.data.numHints = 0;
                SaveSystem.data.numFreezes = 0;
                SaveSystem.data.numFires = 0;
                SaveSystem.data.numMagnets = 0;
                SaveSystem.data.numBlackholes = 0;
                SaveSystem.data.numScissors = 0;

                for(int i = 0; i < unlockNames.Length; i++) {
                    SaveSystem.data.unlockedAchievements.Add(false);
                }

                SaveSystem.data.musicVolume = .5f;
                SaveSystem.data.soundfxVolume = .5f;

                SaveSystem.data.gamesSinceAd = 0;

                SaveSystem.data.removedAds = false;

                SaveSystem.SavePlayer();
            } else {
                SaveSystem.LoadPlayer();

                starAmountSum = new List<int>();
                for(int i = 0; i < 7; i++) {
                    SaveSystem.data.starAmount.Add(new List<int>());
                    starAmountSum.Add(0);
                    for(int j = 0; j < SaveSystem.data.maxLevel; j++) {
                        starAmountSum[i] += SaveSystem.data.starAmount[i][j];
                    }
                    starAmountTotalSum += starAmountSum[i];
                }
            }

        }

        #endregion
    }

    void Start() {
        sfxmanager = GameObject.Find("SFXManager").GetComponent<SFXManager>();
        buttonPressSFX = (AudioClip)Resources.Load("Sounds/SoundFX/button_press");
        string sceneName = SceneManager.GetActiveScene().name;
        // if(loadedFirstScene) {
        //     if(sceneName!="CountingLevel" && loadedFirstSceneAfter) {
        //         loadedFirstScene = false;
        //         ChangeScene.ChangeLoadScene("MainMenu");
        //         // ChangeScene.switchScene = true;
        //     }
        // }
        // loadedFirstSceneAfter = true;
    }

    private void Update() {
        countTouches = Input.touchCount;
        if(Time.time%3==1 || countTouches != Input.touchCount || Input.GetMouseButtonUp(0)) {
            SaveSystem.SavePlayer();
        }

        starAmountTotalSum = 0;
        for(int i = 0; i < 7; i++) {
            starAmountSum[i] = 0;
            for(int j = 0; j < SaveSystem.data.maxLevel; j++) {
                starAmountSum[i] += SaveSystem.data.starAmount[i][j];
            }
            starAmountTotalSum += starAmountSum[i];
        }

        if(EventSystem.current.currentSelectedGameObject!=null && EventSystem.current.currentSelectedGameObject!=selectedObj) {
            selectedObj = EventSystem.current.currentSelectedGameObject;
            sfxmanager.PlaySFX(buttonPressSFX);
        }
        if(EventSystem.current.currentSelectedGameObject==null) {
            selectedObj = null;
        }
    }

    public void OpenLevelSelectOrPlay() {
        string sceneName = SceneManager.GetActiveScene().name;
        ShouldHideBanner(sceneName);
        SetPrevSelect(sceneName);
        FromPlayingGame();
        if(previousSelectScene!=null) {
            ChangeScene.ChangeLoadScene(sceneName);
        } else {
            ChangeScene.ChangeLoadScene("LevelSelect");
        }
    }

    public void OpenGameOptions() {
        string sceneName = SceneManager.GetActiveScene().name;
        ShouldHideBanner(sceneName);
        SetPrevSelect(sceneName);
        FromPlayingGame();
        ChangeScene.ChangeLoadScene("Options");
    }

    public void OpenUpgrades() {
        string sceneName = SceneManager.GetActiveScene().name;
        ShouldHideBanner(sceneName);
        SetPrevSelect(sceneName);
        FromPlayingGame();
        ChangeScene.ChangeLoadScene("Upgrades");
    }

    public void OpenShop() {
        string sceneName = SceneManager.GetActiveScene().name;
        ShouldHideBanner(sceneName);
        SetPrevSelect(sceneName);
        ChangeScene.ChangeLoadScene("Shop");
    }

    public void OpenInstructions() {
        string sceneName = SceneManager.GetActiveScene().name;
        ShouldHideBanner(sceneName);
        SetPrevSelect(sceneName);
        ChangeScene.ChangeLoadScene("Instructions");
    }

    public void OpenMainMenu() {
        string sceneName = SceneManager.GetActiveScene().name;
        ShouldHideBanner(sceneName);
        SetPrevSelect(sceneName);
        ChangeScene.ChangeLoadScene("MainMenu");
    }

    public void OpenSectionSelect() {
        string sceneName = SceneManager.GetActiveScene().name;
        ShouldHideBanner(sceneName);
        SetPrevSelect(sceneName);
        ChangeScene.ChangeLoadScene("SectionSelect");
    }

    public void OpenAchievements() {
        string sceneName = SceneManager.GetActiveScene().name;
        ShouldHideBanner(sceneName);
        SetPrevSelect(sceneName);
        ChangeScene.ChangeLoadScene("Achievements");
    }

    public void ReplayLevel() {
        FromPlayingGame();
        SceneManager.LoadScene("CountingLevel");
    }

    void FromPlayingGame() {
        string sceneName = SceneManager.GetActiveScene().name;
        if(sceneName == "CountingLevel") {
            SaveSystem.data.gamesSinceAd++;
            if(SaveSystem.data.gamesSinceAd >= gamesForAd) {
                SaveSystem.data.gamesSinceAd = 0;
                AdManager.ShowStandardAd();
                gamesForAd = Random.Range(2, 6);
            }
        }
    }

    void SetPrevSelect(string sceneName) {
        if(sceneName == "SectionSelect") {
            previousSelectScene = "SectionSelect";
        } else if(sceneName == "LevelSelect") {
            previousSelectScene = null;
        }
    }

    void ShouldHideBanner(string sceneName) {
        if(sceneName == "MainMenu" || sceneName == "Options" || sceneName == "Shop" || sceneName == "Upgrades" ||
            sceneName == "Achievements") {
            AdManager.HideBanner();
        }
    }

    void LoadMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
    void LoadSectionSelect() {
        SceneManager.LoadScene("SectionSelect");
    }
    void LoadLevelSelect() {
        SceneManager.LoadScene("LevelSelect");
    }
    void LoadAchievements() {
        SceneManager.LoadScene("Achievements");
    }
    void LoadOptions() {
        SceneManager.LoadScene("Options");
    }
    void LoadShop() {
        SceneManager.LoadScene("Shop");
    }
    void LoadUpgrades() {
        SceneManager.LoadScene("Upgrades");
    }
    void LoadInstructions() {
        SceneManager.LoadScene("Instructions");
    }
}
