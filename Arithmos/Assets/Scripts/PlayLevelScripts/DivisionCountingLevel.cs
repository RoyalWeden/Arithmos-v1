using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DivisionCountingLevel : PlayCountingLevel {

    protected override void LoadAllNumbers() {
        secondsToPlay = Mathf.Pow(1.2f, Mathf.Pow(1.02f, levelNum)) * levelNum * 25;

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
        string numDiv = "";

        if(numberOfNumbers>1) {
            int product = x;
            List<int> divisors = new List<int>();
            for(int i = 1; i < numberOfNumbers; i++) {
                int num = Random.Range(1, 9);
                divisors.Add(num);
                product *= num;
            }
            numDiv += product.ToString() + "÷";

            for(int i=1; i<numberOfNumbers; i++) {
                int randChoice = Random.Range(0, divisors.Count);
                if(i+1==numberOfNumbers) {
                    numDiv += divisors[randChoice].ToString();
                } else {
                    numDiv += divisors[randChoice].ToString() + "÷";
                }
                divisors.RemoveAt(randChoice);
            }
        } else {
            numDiv = x.ToString();
        }
        

        return numDiv;
    }
}
