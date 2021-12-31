using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PurchaseUpgrades : MonoBehaviour {

    public Text totalCoins;

    [Space(5)]
    public int rows;
    public int cols;
    public GameObject uiPrefab;

    string[] upgradeNames;
    int[] upgradePrices;
    UnityAction[] upgradeActions;
    List<ArrayList> upgradeArrays;
    List<GameObject> upgradeObjs;

    void Start() {
        upgradeArrays = new List<ArrayList>();
        upgradeObjs = new List<GameObject>();
        upgradeNames = new string[] { "Hint", "Magnet", "Freeze", "Fire", "Blackhole", "Scissors" };
        upgradePrices = new int[] { 15, 50, 85, 110, 415, 900 };
        upgradeActions = new UnityAction[] { PurchaseHint, PurchaseMagnet, PurchaseFreeze, PurchaseFire, PurchaseBlackhole, PurchaseScissors };
        LoadLevelSelectButtons();

        StartCoroutine("ShowTotalCoinsDownUp");
    }

    void Update() {
        ReloadLevelSelectButtons();
    }

    void LoadLevelSelectButtons() {
        RectTransform parentRect = gameObject.GetComponent<RectTransform>();
        GridLayoutGroup gridLayout = gameObject.GetComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2((parentRect.rect.width - gridLayout.spacing.x * (cols - 1)) / cols, (parentRect.rect.height - gridLayout.spacing.y * (rows - 1)) / rows);
        for(int i = 0; i < rows; i++) {
            for(int j = 0; j < cols; j++) {
                GameObject inputField = Instantiate(uiPrefab);
                inputField.transform.SetParent(gameObject.transform, false);
                upgradeObjs.Add(inputField);
                upgradeArrays.Add(new ArrayList());
                SetUpgradeArray(upgradeArrays[upgradeObjs.Count - 1], upgradeObjs[upgradeObjs.Count - 1]);
                ((Button)upgradeArrays[upgradeObjs.Count - 1][0]).onClick.AddListener(upgradeActions[upgradeObjs.Count - 1]);
                ((Text)upgradeArrays[upgradeObjs.Count - 1][3]).text = upgradeNames[upgradeObjs.Count - 1];
            }
        }
    }

    void ReloadLevelSelectButtons() {
        RectTransform parentRect = gameObject.GetComponent<RectTransform>();
        GridLayoutGroup gridLayout = gameObject.GetComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2((parentRect.rect.width - gridLayout.spacing.x * (cols - 1)) / cols, (parentRect.rect.height - gridLayout.spacing.y * (rows - 1)) / rows);
        for(int i = 0; i < rows; i++) {
            for(int j = 0; j < cols; j++) {
                SetPurchaseButton(upgradePrices[2*i+j], (Text)upgradeArrays[2*i+j][1], (Button)upgradeArrays[2*i+j][0]);
            }
        }

        for(int i=0; i<upgradeObjs.Count; i++) {
            SetUpgradeUnlocked(SaveSystem.data.unlockedAchievements[i+1], upgradeObjs[i]);
        }

        ((Text)upgradeArrays[0][2]).text = "x" + SaveSystem.data.numHints.ToString();
        ((Text)upgradeArrays[1][2]).text = "x" + SaveSystem.data.numMagnets.ToString();
        ((Text)upgradeArrays[2][2]).text = "x" + SaveSystem.data.numFreezes.ToString();
        ((Text)upgradeArrays[3][2]).text = "x" + SaveSystem.data.numFires.ToString();
        ((Text)upgradeArrays[4][2]).text = "x" + SaveSystem.data.numBlackholes.ToString();
        ((Text)upgradeArrays[5][2]).text = "x" + SaveSystem.data.numScissors.ToString();
    }

    void SetUpgradeUnlocked(bool unlocked, GameObject upgradeObj) {
        if(unlocked) {
            upgradeObj.SetActive(true);
        } else {
            upgradeObj.SetActive(false);
        }
    }

    void SetUpgradeArray(ArrayList upgradeArray, GameObject upgradeObj) {
        upgradeArray.Add(upgradeObj.transform.Find("Button").GetComponent<Button>());
        upgradeArray.Add(upgradeObj.transform.Find("Cost").GetComponent<Text>());
        upgradeArray.Add(upgradeObj.transform.Find("PurchaseCount").GetComponent<Text>());
        upgradeArray.Add(upgradeObj.transform.Find("Button").Find("Name").GetComponent<Text>());
    }

    void SetPurchaseButton(int baseCost, Text costText, Button upgradeButton) {
        costText.text = baseCost.ToString();
        if(SaveSystem.data.coins >= baseCost) {
            upgradeButton.interactable = true;
            costText.color = Color.white;
        } else {
            upgradeButton.interactable = false;
            costText.color = Color.red;
        }
    }

    IEnumerator ShowTotalCoinsDownUp() {
        float tempCoinCount = 0;
        float coinDiv = SaveSystem.data.coins / 100.0f;
        while(tempCoinCount < SaveSystem.data.coins) {
            totalCoins.text = Mathf.CeilToInt(tempCoinCount).ToString();
            tempCoinCount += coinDiv;
            yield return new WaitForSeconds(.01f);
        }
        totalCoins.text = SaveSystem.data.coins.ToString();
    }

    IEnumerator ShowTotalCoinsUpDown(int cost) {
        float tempCoinCount = SaveSystem.data.coins + cost;
        float coinDiv = cost / 50.0f;
        while(tempCoinCount > SaveSystem.data.coins) {
            totalCoins.text = Mathf.FloorToInt(tempCoinCount).ToString();
            tempCoinCount -= coinDiv;
            yield return new WaitForSeconds(.01f);
        }
        totalCoins.text = SaveSystem.data.coins.ToString();
    }

    public void PurchaseHint() {
        SaveSystem.data.coins -= upgradePrices[0];
        SaveSystem.data.numHints++;
        StopAllCoroutines();
        StartCoroutine(ShowTotalCoinsUpDown(upgradePrices[0]));
    }
    public void PurchaseMagnet() {
        SaveSystem.data.coins -= upgradePrices[1];
        SaveSystem.data.numMagnets++;
        StopAllCoroutines();
        StartCoroutine(ShowTotalCoinsUpDown(upgradePrices[1]));
    }
    public void PurchaseFreeze() {
        SaveSystem.data.coins -= upgradePrices[2];
        SaveSystem.data.numFreezes++;
        StopAllCoroutines();
        StartCoroutine(ShowTotalCoinsUpDown(upgradePrices[2]));
    }
    public void PurchaseFire() {
        SaveSystem.data.coins -= upgradePrices[3];
        SaveSystem.data.numFires++;
        StopAllCoroutines();
        StartCoroutine(ShowTotalCoinsUpDown(upgradePrices[3]));
    }
    public void PurchaseBlackhole() {
        SaveSystem.data.coins -= upgradePrices[4];
        SaveSystem.data.numBlackholes++;
        StopAllCoroutines();
        StartCoroutine(ShowTotalCoinsUpDown(upgradePrices[4]));
    }
    public void PurchaseScissors() {
        SaveSystem.data.coins -= upgradePrices[5];
        SaveSystem.data.numScissors++;
        StopAllCoroutines();
        StartCoroutine(ShowTotalCoinsUpDown(upgradePrices[5]));
    }
}
