using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowAchievements : MonoBehaviour {

    public Text achievementName;
    public GameObject achievementPanel;
    public static ShowAchievements instance;
    bool isAchievementsPlaying;
    public static bool canShowAchievements;

    private void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Update() {
        if(canShowAchievements && !isAchievementsPlaying && !PlayCountingLevel.isInLevel) {
            StartCoroutine("PlayAchievements");
        }
        if(PlayCountingLevel.isInLevel && isAchievementsPlaying) {
            StopCoroutine("PlayAchievements");
            achievementPanel.SetActive(false);
            isAchievementsPlaying = false;
        }
    }

    public IEnumerator PlayAchievements() {
        isAchievementsPlaying = true;
        achievementPanel.SetActive(true);

        for(int i=0; i<GameManager.unlockNames.Length; i++) {
            if(CheckAchievement(SaveSystem.data.unlockedAchievements[i], GameManager.unlockStarsNeeded[i], GameManager.unlockCoinsNeeded[i], GameManager.activateUnlocks[i])) {
                SaveSystem.data.coins -= GameManager.unlockCoinsNeeded[i];
                achievementName.text = GameManager.unlockNames[i];
                achievementPanel.GetComponent<Animation>().Play("ShowAchievement");
                SaveSystem.data.unlockedAchievements[i] = true;
                yield return new WaitForSeconds(achievementPanel.GetComponent<Animation>().GetClip("ShowAchievement").length + .1f);
            }
        }

        isAchievementsPlaying = false;
        achievementPanel.SetActive(false);
    }

    bool CheckAchievement(bool hasBeenUnlocked, int starsNeeded, int coinsNeeded, bool activation) {
        if(!hasBeenUnlocked) {
            if(activation) {
                return GameManager.starAmountTotalSum >= starsNeeded && SaveSystem.data.coins >= coinsNeeded;
            } else {
                return false;
            }
        }
        return false;
    }
}
