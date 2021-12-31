using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubtractionCountingLevel : PlayCountingLevel {

    protected override void LoadAllNumbers() {
        secondsToPlay = Mathf.Pow(1.2f, Mathf.Pow(1.02f, levelNum)) * levelNum * 20;

        secondsForCombo = 1.5f;

        totalNumbers = levelNum * 1;
        numbersOnPage = new List<int>();

        numbersPerPage = 20.0f;
        totalPages = Mathf.CeilToInt(totalNumbers / numbersPerPage);

        int tempNumbers = 0;
        for(int i = 0; i < totalPages; i++) {
            if(tempNumbers + numbersPerPage > totalNumbers) {
                numbersOnPage.Add(totalNumbers - tempNumbers);
            } else {
                numbersOnPage.Add((int)numbersPerPage);
                tempNumbers += (int)numbersPerPage;
            }
        }

        if(totalPages > 1) {
            pagesPanel.SetActive(true);
        } else {
            pagesPanel.SetActive(false);
        }

        numTextList = new List<Text>();
        PlacePageNumbers();
    }

    protected override string GenerateNumberToPlace(int x) {
        int numberOfNumbers = Random.Range(1, 4);
        if(numberOfNumbers>1) {
            string numDiff = "";
            int diff = 0;
            for(int i=numberOfNumbers; i>1; i--) {
                int num = 0;
                if(i==numberOfNumbers) {
                    num = Random.Range(x, (i + 2) * x + 1);
                    diff = num;
                    numDiff = num.ToString() + "-";
                } else {
                    num = Random.Range(0, diff - x);
                    diff -= num;
                    numDiff += num.ToString() + "-";
                }
            }
            numDiff += (diff - x).ToString();
            return numDiff;
        } else {
            return x.ToString();
        }
        
    }
}
