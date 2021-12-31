using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class PlayerData {
    public int curStartLvlChoice;
    public int maxLevel;

    public List<List<int>> starAmount;

    public int coins;

    public int numHints;
    public int numFreezes;
    public int numFires;
    public int numMagnets;
    public int numBlackholes;
    public int numScissors;

    public List<bool> unlockedAchievements;

    public float musicVolume;
    public float soundfxVolume;

    public int gamesSinceAd;

    public bool removedAds;

    public PlayerData(int curStartLvlChoice, int maxLevel,
                      List<List<int>> starAmount,
                      int coins, int numHints, int numFreezes, int numFires, int numMagnets, int numBlackholes, int numScissors,
                      List<bool> unlockedAchievements,
                      float musicVolume, float soundfxVolume, 
                      int gamesSinceAd,
                      bool removedAds) {
        this.curStartLvlChoice = curStartLvlChoice;
        this.maxLevel = maxLevel;

        this.starAmount = starAmount;

        this.coins = coins;
        this.numHints = numHints;
        this.numFreezes = numFreezes;
        this.numFires = numFires;
        this.numMagnets = numMagnets;
        this.numBlackholes = numBlackholes;
        this.numScissors = numScissors;

        this.unlockedAchievements = unlockedAchievements;

        this.musicVolume = musicVolume;
        this.soundfxVolume = soundfxVolume;

        this.gamesSinceAd = gamesSinceAd;

        this.removedAds = removedAds;
    }
}
