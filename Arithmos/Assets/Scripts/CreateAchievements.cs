using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateAchievements : MonoBehaviour {

    public Text totalStarsText;

    List<Text> textNames;
    List<Text> textStarCount;

    
    int rows;
    int cols;
    [Space(5)]
    public GameObject uiPrefab;

    void Start() {
        rows = GameManager.unlockNames.Length;
        cols = 1;

        GetComponent<RectTransform>().offsetMin = new Vector2(GetComponent<RectTransform>().offsetMin.x, -97.07605f * rows);

        LoadLevelSelectButtons();

        StartCoroutine("ShowTotalStars");


        textNames = new List<Text>();
        textStarCount = new List<Text>();
        foreach(Transform t in transform) {
            textNames.Add(t.Find("Name").GetComponent<Text>());
            textStarCount.Add(t.Find("StarCount").GetComponent<Text>());
        }

        for(int i=0; i < GameManager.unlockNames.Length; i++) {
            SetAchievementText(textNames[i], textStarCount[i], GameManager.unlockNames[i], GameManager.unlockStarsNeeded[i], SaveSystem.data.unlockedAchievements[i]);
        }
    }

    private void Update() {
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
            }
        }
    }

    void ReloadLevelSelectButtons() {
        RectTransform parentRect = gameObject.GetComponent<RectTransform>();
        GridLayoutGroup gridLayout = gameObject.GetComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2((parentRect.rect.width - gridLayout.spacing.x * (cols - 1)) / cols, (parentRect.rect.height - gridLayout.spacing.y * (rows - 1)) / rows);

        for(int i = 0; i < GameManager.unlockNames.Length; i++) {
            SetAchievementText(textNames[i], textStarCount[i], GameManager.unlockNames[i], GameManager.unlockStarsNeeded[i], SaveSystem.data.unlockedAchievements[i]);
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

    void SetAchievementText(Text name, Text count, string setname, int setcount, bool isUnlocked) {
        if(!isUnlocked) {
            name.text = "???";
            count.color = Color.red;
        } else {
            name.text = setname;
            count.color = Color.white;
        }
        count.text = setcount.ToString();
    }
}
