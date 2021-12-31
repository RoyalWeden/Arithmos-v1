using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplicationCountingLevel : PlayCountingLevel {

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
        string numMult = "";
        int mult = 1;

        for(int i = 0; i < numberOfNumbers; i++) {
            List<Vector2Int> products = new List<Vector2Int>();
            if(i + 1 != numberOfNumbers) {

                int div = x / mult;
                for(int j = 1; j <= Mathf.CeilToInt(Mathf.Sqrt(div)); j++) {
                    if(div % j == 0) {
                        products.Add(new Vector2Int(j, div / j));
                    }
                }

                int randProducts = Random.Range(0, products.Count);
                if(Random.Range(0, 2) == 0) {
                    numMult += products[randProducts].x.ToString() + "×";
                    mult *= products[randProducts].x;
                } else {
                    numMult += products[randProducts].y.ToString() + "×";
                    mult *= products[randProducts].y;
                }

            } else {
                numMult += (x / mult).ToString();
                mult *= (x / mult);
            }
        }

        return numMult;
    }
}
