using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelSelect : MonoBehaviour {

    Image[] levelImgs;
    Text[] levelsText;
    GameObject[] starsSet;
    GameObject[] starsNeededSet;

    public Text totalStarsText;

    [Space(5)]
    public Sprite emptyStar;
    public Sprite fullStar;

    [Space(5)]
    public Sprite easyLvlSprite;
    public Sprite mediumLvlSprite;
    public Sprite hardLvlSprite;

    int mediumLvlStart;
    int hardLvlStart;

    [Space(5)]
    public Sprite lockedLvlSprite;

    [Space(5)]
    public List<int> starsNeededForLvl;

    [Space(5)]
    public GameObject leftButton;
    public GameObject rightButton;

    [Space(5)]
    public GameObject upgradeButtonObj;
    public Sprite upgradeSprite;

    [Space(5)]
    public int rows;
    public int cols;
    public GameObject uiPrefab;

    [Space(5)]
    public GameObject loadingPanel;

    void Start() {
        LoadLevelSelectButtons();

        StartCoroutine("ShowTotalStars");

        mediumLvlStart = SaveSystem.data.maxLevel / 3;
        hardLvlStart = (SaveSystem.data.maxLevel * 2) / 3;
        starsNeededForLvl = new List<int>();
        starsNeededForLvl.AddRange(new int[]{0,0,0,0,0,8,9});
        while(starsNeededForLvl.Count<SaveSystem.data.maxLevel) {
            int addWhichNum = (starsNeededForLvl.Count - 5) % 8;
            if(addWhichNum<2) {
                starsNeededForLvl.Add(starsNeededForLvl[starsNeededForLvl.Count - 1] + 1);
            } else if(addWhichNum < 4) {
                starsNeededForLvl.Add(starsNeededForLvl[starsNeededForLvl.Count - 1] + 2);
            } else if(addWhichNum < 6) {
                starsNeededForLvl.Add(starsNeededForLvl[starsNeededForLvl.Count - 1] + 3);
            } else if(addWhichNum < 8) {
                starsNeededForLvl.Add(starsNeededForLvl[starsNeededForLvl.Count - 1] + 4);
            }
        }

        levelImgs = new Image[9];
        levelsText = new Text[9];
        starsSet = new GameObject[9];
        starsNeededSet = new GameObject[9];
        for(int i = 0; i < transform.childCount; i++) {
            levelImgs[i] = transform.GetChild(i).GetComponent<Image>();
            levelsText[i] = transform.GetChild(i).Find("Text").GetComponent<Text>();
            levelsText[i].text = (SaveSystem.data.curStartLvlChoice + i).ToString();
            starsSet[i] = transform.GetChild(i).Find("Stars").gameObject;
            starsNeededSet[i] = transform.GetChild(i).Find("StarsNeeded").gameObject;
            SetStarCount(starsSet[i].transform, SaveSystem.data.curStartLvlChoice + i - 1);
            
            if(GameManager.starAmountSum[(int)PlayCountingLevel.leveltype] >= starsNeededForLvl[SaveSystem.data.curStartLvlChoice + i - 1]) {
                levelsText[i].gameObject.SetActive(true);
                if(SaveSystem.data.curStartLvlChoice + i>=hardLvlStart) {
                    levelImgs[i].sprite = hardLvlSprite;
                } else if(SaveSystem.data.curStartLvlChoice + i >= mediumLvlStart) {
                    levelImgs[i].sprite = mediumLvlSprite;
                } else {
                    levelImgs[i].sprite = easyLvlSprite;
                }
                levelImgs[i].gameObject.GetComponent<Button>().interactable = true;
                starsSet[i].SetActive(true);
                starsNeededSet[i].SetActive(false);
            } else {
                levelsText[i].gameObject.SetActive(false);
                levelImgs[i].sprite = lockedLvlSprite;
                levelImgs[i].gameObject.GetComponent<Button>().interactable = false;
                starsSet[i].SetActive(false);
                starsNeededSet[i].SetActive(true);
                starsNeededSet[i].transform.Find("NumberStars").GetComponent<Text>().text =
                    (starsNeededForLvl[SaveSystem.data.curStartLvlChoice + i - 1]).ToString();
            }
        }
    }

    private void Update() {
        ReloadLevelSelectButtons();

        if(SaveSystem.data.curStartLvlChoice + 8 >= SaveSystem.data.maxLevel) {
            rightButton.SetActive(false);
        } else {
            rightButton.SetActive(true);
        }

        if(SaveSystem.data.curStartLvlChoice <= 1) {
            leftButton.SetActive(false);
        } else {
            leftButton.SetActive(true);
        }

        if(SaveSystem.data.unlockedAchievements[0]) {
            upgradeButtonObj.GetComponent<Button>().enabled = true;
            upgradeButtonObj.GetComponent<Image>().sprite = upgradeSprite;
        } else {
            upgradeButtonObj.GetComponent<Button>().enabled = false;
            upgradeButtonObj.GetComponent<Image>().sprite = lockedLvlSprite;
        }

        ShowAchievements.canShowAchievements = true;
    }

    void LoadLevelSelectButtons() {
        RectTransform parentRect = gameObject.GetComponent<RectTransform>();
        GridLayoutGroup gridLayout = gameObject.GetComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2((parentRect.rect.width - gridLayout.spacing.x * (cols - 1)) / cols, (parentRect.rect.height - gridLayout.spacing.y * (rows - 1)) / rows);
        for(int i = 0; i < rows; i++) {
            for(int j = 0; j < cols; j++) {
                GameObject inputField = Instantiate(uiPrefab);
                inputField.transform.SetParent(gameObject.transform, false);

                inputField.GetComponent<Button>().onClick.AddListener(delegate { ChooseLevel(inputField); });
                inputField.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { ChooseLevel(inputField); });
            }
        }
    }

    void ReloadLevelSelectButtons() {
        RectTransform parentRect = gameObject.GetComponent<RectTransform>();
        GridLayoutGroup gridLayout = gameObject.GetComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2((parentRect.rect.width - gridLayout.spacing.x * (cols - 1)) / cols, (parentRect.rect.height - gridLayout.spacing.y * (rows - 1)) / rows);
    }

    void ChooseLevel(GameObject g) {
        while(g.GetComponent<Text>() == null) {
            g = g.transform.GetChild(0).gameObject;
        }
        GameManager.previousSelectScene = null;
        PlayCountingLevel.levelNum = Convert.ToInt32(g.GetComponent<Text>().text);

        StartCoroutine("WaitToShowLevel");
    }

    IEnumerator WaitToShowLevel() {
        loadingPanel.SetActive(true);
        loadingPanel.GetComponent<Animation>().wrapMode = WrapMode.Loop;
        loadingPanel.GetComponent<Animation>().Play("LoadingGame");

        PlayCountingLevel.isInLevel = true;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("CountingLevel");
        asyncOperation.allowSceneActivation = false;
        
        float topBound = ((int)PlayCountingLevel.leveltype + 1) * PlayCountingLevel.levelNum / 5 + 1f;
        if(topBound > 2) {
            topBound = 2f;
        }

        yield return new WaitForSeconds(Random.Range(0.35f, topBound));

        asyncOperation.allowSceneActivation = true;
    }

    IEnumerator ShowTotalStars() {
        float tempStarCount = 0;
        float starDiv = GameManager.starAmountSum[(int)PlayCountingLevel.leveltype] / 45.0f;
        while(tempStarCount < GameManager.starAmountSum[(int)PlayCountingLevel.leveltype]) {
            totalStarsText.text = Mathf.CeilToInt(tempStarCount).ToString();
            tempStarCount += starDiv;
            yield return new WaitForSeconds(.01f);
        }
        totalStarsText.text = GameManager.starAmountSum[(int)PlayCountingLevel.leveltype].ToString();
    }

    public void IncreaseLevelChoices() {
        if(SaveSystem.data.curStartLvlChoice + 8 < SaveSystem.data.maxLevel) {
            SaveSystem.data.curStartLvlChoice += 9;
            for(int i = 0; i < transform.childCount; i++) {
                levelsText[i].text = (SaveSystem.data.curStartLvlChoice + i).ToString();
                SetStarCount(starsSet[i].transform, SaveSystem.data.curStartLvlChoice + i - 1);
                if(GameManager.starAmountSum[(int)PlayCountingLevel.leveltype] >= starsNeededForLvl[SaveSystem.data.curStartLvlChoice + i - 1]) {
                    levelsText[i].gameObject.SetActive(true);
                    if(SaveSystem.data.curStartLvlChoice + i >= hardLvlStart) {
                        levelImgs[i].sprite = hardLvlSprite;
                    } else if(SaveSystem.data.curStartLvlChoice + i >= mediumLvlStart) {
                        levelImgs[i].sprite = mediumLvlSprite;
                    } else {
                        levelImgs[i].sprite = easyLvlSprite;
                    }
                    levelImgs[i].gameObject.GetComponent<Button>().interactable = true;
                    starsSet[i].SetActive(true);
                    starsNeededSet[i].SetActive(false);
                } else {
                    levelsText[i].gameObject.SetActive(false);
                    levelImgs[i].sprite = lockedLvlSprite;
                    levelImgs[i].gameObject.GetComponent<Button>().interactable = false;
                    starsSet[i].SetActive(false);
                    starsNeededSet[i].SetActive(true);
                    starsNeededSet[i].transform.Find("NumberStars").GetComponent<Text>().text = (starsNeededForLvl[SaveSystem.data.curStartLvlChoice + i - 1]).ToString();
                }
            }
        }
    }

    public void DecreaseLevelChoices() {
        if(SaveSystem.data.curStartLvlChoice > 1) {
            SaveSystem.data.curStartLvlChoice -= 9;
            for(int i = 0; i < transform.childCount; i++) {
                levelsText[i].text = (SaveSystem.data.curStartLvlChoice + i).ToString();
                SetStarCount(starsSet[i].transform, SaveSystem.data.curStartLvlChoice + i - 1);
                if(GameManager.starAmountSum[(int)PlayCountingLevel.leveltype] >= starsNeededForLvl[SaveSystem.data.curStartLvlChoice + i - 1]) {
                    levelsText[i].gameObject.SetActive(true);
                    if(SaveSystem.data.curStartLvlChoice + i >= hardLvlStart) {
                        levelImgs[i].sprite = hardLvlSprite;
                    } else if(SaveSystem.data.curStartLvlChoice + i >= mediumLvlStart) {
                        levelImgs[i].sprite = mediumLvlSprite;
                    } else {
                        levelImgs[i].sprite = easyLvlSprite;
                    }
                    levelImgs[i].gameObject.GetComponent<Button>().interactable = true;
                    starsSet[i].SetActive(true);
                    starsNeededSet[i].SetActive(false);
                } else {
                    levelsText[i].gameObject.SetActive(false);
                    levelImgs[i].sprite = lockedLvlSprite;
                    levelImgs[i].gameObject.GetComponent<Button>().interactable = false;
                    starsSet[i].SetActive(false);
                    starsNeededSet[i].SetActive(true);
                    starsNeededSet[i].transform.Find("NumberStars").GetComponent<Text>().text = (starsNeededForLvl[SaveSystem.data.curStartLvlChoice + i - 1]).ToString();
                }
            }
        }
    }

    void SetStarCount(Transform starsObj, int ind) {
        if(SaveSystem.data.starAmount[(int)PlayCountingLevel.leveltype][ind] > 0) {
            starsObj.GetChild(0).GetComponent<Image>().sprite = fullStar;
        } else {
            starsObj.GetChild(0).GetComponent<Image>().sprite = emptyStar;
        }
        if(SaveSystem.data.starAmount[(int)PlayCountingLevel.leveltype][ind] > 1) {
            starsObj.GetChild(1).GetComponent<Image>().sprite = fullStar;
        } else {
            starsObj.GetChild(1).GetComponent<Image>().sprite = emptyStar;
        }
        if(SaveSystem.data.starAmount[(int)PlayCountingLevel.leveltype][ind] > 2) {
            starsObj.GetChild(2).GetComponent<Image>().sprite = fullStar;
        } else {
            starsObj.GetChild(2).GetComponent<Image>().sprite = emptyStar;
        }
    }
}
