using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SectionSelect : MonoBehaviour {

    Image[] sectionImgs;
    Text[] sectionsText;
    GameObject[] starsInSection;
    GameObject[] starsNeededSet;
    GameObject[] coinsNeededSet;

    GameObject[] purchaseSectionButtons;

    string[] sectionLabels;

    public Text totalStarsText;

    [Space(5)]
    public Sprite emptyStar;
    public Sprite fullStar;

    [Space(5)]
    public Sprite easySectionSprite;
    public Sprite mediumSectionSprite;
    public Sprite hardSectionSprite;

    [Space(5)]
    public Sprite lockedSectionSprite;

    [Space(5)]
    public GameObject upgradeButtonObj;
    public Sprite upgradeSprite;
    public Sprite lockedUpgradeSprite;

    UnityAction[] buttonActions;

    void Start() {
        StartCoroutine("ShowTotalStars");

        List<bool> sectionsUnlocked = new List<bool> { true };
        for(int i = 7; i <= 12; i++) {
            sectionsUnlocked.Add(SaveSystem.data.unlockedAchievements[i]);
        }
        sectionLabels = new string[] { "Counting", "Addition", "Subtraction", "Multiplication", "Division", "Powers", "Linear Expressions" };

        buttonActions = new UnityAction[] {GoToCountingLevelSelect, GoToAddingLevelSelect, GoToSubtractingLevelSelect, GoToMultiplyingLevelSelect, GoToDividingLevelSelect, GoToPowersLevelSelect, GoToEquationsLevelSelect};

        sectionImgs = new Image[sectionLabels.Length];
        sectionsText = new Text[sectionLabels.Length];
        starsInSection = new GameObject[sectionLabels.Length];
        starsNeededSet = new GameObject[sectionLabels.Length];
        coinsNeededSet = new GameObject[sectionLabels.Length];
        purchaseSectionButtons = new GameObject[sectionLabels.Length];
        for(int i = 0; i < sectionLabels.Length; i++) {
            sectionImgs[i] = transform.GetChild(i).GetComponent<Image>();
            sectionsText[i] = transform.GetChild(i).Find("Text").GetComponent<Text>();
            starsInSection[i] = transform.GetChild(i).Find("StarsInSection").gameObject;
            starsNeededSet[i] = transform.GetChild(i).Find("StarsNeeded").gameObject;
            coinsNeededSet[i] = transform.GetChild(i).Find("CoinsNeeded").gameObject;
            purchaseSectionButtons[i] = transform.GetChild(i).Find("PurchaseSection").gameObject;

            transform.GetChild(i).GetComponent<Button>().onClick.AddListener(buttonActions[i]);
            sectionsText[i].transform.GetComponent<Button>().onClick.AddListener(buttonActions[i]);

            int section = i-1;
            purchaseSectionButtons[i].GetComponent<Button>().onClick.AddListener(delegate { BuySection(section); });
            purchaseSectionButtons[i].transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { BuySection(section); });

            if(sectionsUnlocked[i]) {
                sectionsText[i].text = sectionLabels[i];
                if(i >= Mathf.CeilToInt(transform.childCount * 2 / 3)) {
                    sectionImgs[i].sprite = hardSectionSprite;
                } else if(i >= Mathf.CeilToInt(transform.childCount * 1 / 3)) {
                    sectionImgs[i].sprite = mediumSectionSprite;
                } else {
                    sectionImgs[i].sprite = easySectionSprite;
                }
                starsInSection[i].transform.Find("NumberStars").GetComponent<Text>().text = GameManager.starAmountSum[i].ToString();
                if(GameManager.starAmountSum[i]>0) {
                    starsInSection[i].transform.Find("star").GetComponent<Image>().sprite = fullStar;
                } else {
                    starsInSection[i].transform.Find("star").GetComponent<Image>().sprite = emptyStar;
                }
                sectionImgs[i].gameObject.GetComponent<Button>().interactable = true;
                sectionsText[i].gameObject.GetComponent<Button>().interactable = true;
                starsInSection[i].SetActive(true);
                starsNeededSet[i].SetActive(false);
                coinsNeededSet[i].SetActive(false);
                purchaseSectionButtons[i].SetActive(false);
            } else {
                sectionsText[i].text = "Locked";
                sectionImgs[i].sprite = lockedSectionSprite;
                sectionImgs[i].gameObject.GetComponent<Button>().interactable = false;
                sectionsText[i].gameObject.GetComponent<Button>().interactable = false;
                starsInSection[i].SetActive(false);
                starsNeededSet[i].SetActive(true);
                coinsNeededSet[i].SetActive(true);

                Text numberStars = starsNeededSet[i].transform.Find("NumberStars").GetComponent<Text>();
                numberStars.text = GameManager.unlockSectionStarsNeeded[i].ToString();
                bool hasStarsNeeded = false;
                if(GameManager.starAmountTotalSum >= GameManager.unlockSectionStarsNeeded[i]) {
                    numberStars.color = Color.white;
                    hasStarsNeeded = true;
                } else {
                    numberStars.color = Color.red;
                    hasStarsNeeded = false;
                }

                Text numberCoins = coinsNeededSet[i].transform.Find("NumberCoins").GetComponent<Text>();
                numberCoins.text = GameManager.buySectionCoinsNeeded[i].ToString();
                bool hasCoinsNeeded = false;
                if(SaveSystem.data.coins >= GameManager.buySectionCoinsNeeded[i]) {
                    numberCoins.color = Color.white;
                    hasCoinsNeeded = true;
                } else {
                    numberCoins.color = Color.red;
                    hasCoinsNeeded = false;
                }

                if(hasStarsNeeded && hasCoinsNeeded) {
                    purchaseSectionButtons[i].SetActive(true);
                } else {
                    purchaseSectionButtons[i].SetActive(false);
                }
            }
        }
    }

    private void Update() {
        if(SaveSystem.data.unlockedAchievements[0]) {
            upgradeButtonObj.GetComponent<Button>().enabled = true;
            upgradeButtonObj.GetComponent<Image>().sprite = upgradeSprite;
        } else {
            upgradeButtonObj.GetComponent<Button>().enabled = false;
            upgradeButtonObj.GetComponent<Image>().sprite = lockedUpgradeSprite;
        }
        ReloadSectionsUnlocked();
        ShowAchievements.canShowAchievements = true;
    }

    void ReloadSectionsUnlocked() {
        List<bool> sectionsUnlocked = new List<bool> { true };
        for(int i = 7; i <= 12; i++) {
            sectionsUnlocked.Add(SaveSystem.data.unlockedAchievements[i]);
        }

        for(int i = 0; i < sectionLabels.Length; i++) {
            if(sectionsUnlocked[i]) {
                sectionsText[i].text = sectionLabels[i];
                if(i >= Mathf.CeilToInt(transform.childCount * 2 / 3)) {
                    sectionImgs[i].sprite = hardSectionSprite;
                } else if(i >= Mathf.CeilToInt(transform.childCount * 1 / 3)) {
                    sectionImgs[i].sprite = mediumSectionSprite;
                } else {
                    sectionImgs[i].sprite = easySectionSprite;
                }
                starsInSection[i].transform.Find("NumberStars").GetComponent<Text>().text = GameManager.starAmountSum[i].ToString();
                if(GameManager.starAmountSum[i] > 0) {
                    starsInSection[i].transform.Find("star").GetComponent<Image>().sprite = fullStar;
                } else {
                    starsInSection[i].transform.Find("star").GetComponent<Image>().sprite = emptyStar;
                }
                sectionImgs[i].gameObject.GetComponent<Button>().interactable = true;
                sectionsText[i].gameObject.GetComponent<Button>().interactable = true;
                starsInSection[i].SetActive(true);
                starsNeededSet[i].SetActive(false);
                coinsNeededSet[i].SetActive(false);
                purchaseSectionButtons[i].SetActive(false);
            } else {
                sectionsText[i].text = "Locked";
                sectionImgs[i].sprite = lockedSectionSprite;
                sectionImgs[i].gameObject.GetComponent<Button>().interactable = false;
                sectionsText[i].gameObject.GetComponent<Button>().interactable = false;
                starsInSection[i].SetActive(false);
                starsNeededSet[i].SetActive(true);
                coinsNeededSet[i].SetActive(true);

                Text numberStars = starsNeededSet[i].transform.Find("NumberStars").GetComponent<Text>();
                numberStars.text = GameManager.unlockSectionStarsNeeded[i].ToString();
                bool hasStarsNeeded = false;
                if(GameManager.starAmountTotalSum >= GameManager.unlockSectionStarsNeeded[i]) {
                    numberStars.color = Color.white;
                    hasStarsNeeded = true;
                } else {
                    numberStars.color = Color.red;
                    hasStarsNeeded = false;
                }

                Text numberCoins = coinsNeededSet[i].transform.Find("NumberCoins").GetComponent<Text>();
                numberCoins.text = GameManager.buySectionCoinsNeeded[i].ToString();
                bool hasCoinsNeeded = false;
                if(SaveSystem.data.coins >= GameManager.buySectionCoinsNeeded[i]) {
                    numberCoins.color = Color.white;
                    hasCoinsNeeded = true;
                } else {
                    numberCoins.color = Color.red;
                    hasCoinsNeeded = false;
                }

                if(hasStarsNeeded && hasCoinsNeeded) {
                    purchaseSectionButtons[i].SetActive(true);
                } else {
                    purchaseSectionButtons[i].SetActive(false);
                }
            }
        }
    }

    IEnumerator ShowTotalStars() {
        float tempStarCount = 0;
        float starDiv = GameManager.starAmountTotalSum / 45.0f;
        while(tempStarCount < GameManager.starAmountTotalSum) {
            totalStarsText.text = Mathf.CeilToInt(tempStarCount).ToString();
            tempStarCount += starDiv;
            yield return new WaitForSeconds(.01f);
        }
        totalStarsText.text = GameManager.starAmountTotalSum.ToString();
    }

    void BuySection(int section) {
        GameManager.activateUnlocks[section + 7] = true;
        purchaseSectionButtons[section].GetComponent<Button>().enabled = false;
    }

    public void GoToCountingLevelSelect() {
        PlayCountingLevel.leveltype = PlayCountingLevel.LevelType.regular;
        ChangeScene.ChangeLoadScene("LevelSelect");
    }
    public void GoToAddingLevelSelect() {
        PlayCountingLevel.leveltype = PlayCountingLevel.LevelType.addition;
        ChangeScene.ChangeLoadScene("LevelSelect");
    }
    public void GoToSubtractingLevelSelect() {
        PlayCountingLevel.leveltype = PlayCountingLevel.LevelType.subtraction;
        ChangeScene.ChangeLoadScene("LevelSelect");
    }
    public void GoToMultiplyingLevelSelect() {
        PlayCountingLevel.leveltype = PlayCountingLevel.LevelType.multiplication;
        ChangeScene.ChangeLoadScene("LevelSelect");
    }
    public void GoToDividingLevelSelect() {
        PlayCountingLevel.leveltype = PlayCountingLevel.LevelType.division;
        ChangeScene.ChangeLoadScene("LevelSelect");
    }
    public void GoToPowersLevelSelect() {
        PlayCountingLevel.leveltype = PlayCountingLevel.LevelType.power;
        ChangeScene.ChangeLoadScene("LevelSelect");
    }
    public void GoToEquationsLevelSelect() {
        PlayCountingLevel.leveltype = PlayCountingLevel.LevelType.equation;
        ChangeScene.ChangeLoadScene("LevelSelect");
    }
}
