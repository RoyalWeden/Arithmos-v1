using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NormalCountingLevel : PlayCountingLevel {

    protected override void LoadAllNumbers() {
        secondsToPlay = Mathf.Pow(1.1f, Mathf.Pow(1.02f, levelNum)) * levelNum * 10;

        secondsForCombo = 1;

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
        return x.ToString(); 
    }
}
