using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdditionCountingLevel : PlayCountingLevel {

    protected override void LoadAllNumbers() {
        secondsToPlay = Mathf.Pow(1.2f, Mathf.Pow(1.02f, levelNum)) * levelNum * 15;

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
        if(x>=numberOfNumbers) {
            string sum = "";
            int numSum = 0;
            for(int i=0; i<numberOfNumbers; i++) {
                if(i < numberOfNumbers-1) {
                    int num = Mathf.FloorToInt(Random.Range(0, x / (float)numberOfNumbers));
                    numSum += num;
                    sum += num.ToString() + "+";
                } else {
                    sum += (x-numSum).ToString();
                }
            }
            return sum;
        } else {
            return x.ToString();
        }
    }
}
