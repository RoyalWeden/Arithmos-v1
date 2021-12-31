using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System;
using JetBrains.Annotations;

public class PowersCountingLevel : PlayCountingLevel {

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

    struct PowNum {
        public int power;
        public int num;

        public PowNum(int power, int num) {
            this.power = power;
            this.num = num;
        }
    }

    protected override string GenerateNumberToPlace(int x) {
        string[] powers = { "⁰", "¹", "²", "³", "⁴", "⁵", "⁶", "⁷", "⁸", "⁹" };
        string[] operators = { "⁺", "⁻", "ˣ" };

        int numberOfNumbers = Random.Range(1, 5);
        string numPow = "";

        bool isDouble = Random.Range(0, 2) == 0;

        List<PowNum> pows = new List<PowNum>();

        switch(numberOfNumbers) {
            case 2:
                if(x==1) {
                    int whichPow = Random.Range(0, 10);
                    if(whichPow==0) {
                        numPow = Random.Range(1, 51).ToString() + powers[whichPow];
                    } else {
                        if(!isDouble) {
                            numPow = "1" + powers[whichPow];
                        } else {
                            int secondPow = Random.Range(0, 10);
                            numPow = "1" + powers[whichPow] + powers[secondPow];
                        }
                    }
                } else {
                    for(int i = 1; i <= x; i++) {
                        float num = Mathf.Pow(x, 1f / i);
                        if(num == Mathf.RoundToInt(num)) {
                            pows.Add(new PowNum(i, Mathf.RoundToInt(num)));
                        }
                    }
                    int randChoice = Random.Range(0, pows.Count);
                    numPow = pows[randChoice].num.ToString();
                    int powerNum = pows[randChoice].power;
                    while(powerNum/10f>0) {
                        int tempPowNum = powerNum;
                        while(tempPowNum>9) {
                            tempPowNum /= 10;
                        }
                        numPow += powers[tempPowNum];
                        powerNum /= 10;
                    }
                }
                return numPow;
            case 3:
                bool isAdd = Random.Range(0, 2) == 0;
                if(x==1) {
                    int firstFNum = Random.Range(0, 10);
                    int secondFNum = Random.Range(0, 10);

                    int firstSNum;
                    int secondSNum;
                    if(isAdd) {
                        firstSNum = Random.Range(0, 10);
                        secondSNum = Random.Range(0, 10);
                        if(firstFNum == 0 && firstSNum == 0) {
                            numPow = Random.Range(1, 51).ToString() + powers[firstFNum] + operators[0] + powers[firstSNum];
                        } else {
                            if(!isDouble) {
                                numPow = "1" + powers[firstFNum] + operators[0] + powers[firstSNum];
                            } else {
                                if(firstFNum == 0 && firstSNum != 0) {
                                    numPow = "1" + powers[firstFNum] + operators[0] + powers[firstSNum] + powers[secondSNum];
                                } else if (firstFNum != 0 && firstSNum == 0) {
                                    numPow = "1" + powers[firstFNum] + powers[secondFNum] + operators[0] + powers[firstSNum];
                                } else {
                                    numPow = "1" + powers[firstFNum] + powers[secondFNum] + operators[0] + powers[firstSNum] + powers[secondSNum];
                                }
                            }
                        }
                    } else {
                        if(firstFNum == 0) {
                            firstSNum = 0;
                            numPow = Random.Range(1, 51).ToString() + powers[firstFNum] + operators[1] + powers[firstSNum];
                        } else {
                            if(!isDouble) {
                                firstSNum = Random.Range(0, firstFNum);
                                numPow = Random.Range(1, 51).ToString() + powers[firstFNum] + operators[1] + powers[firstSNum];
                            } else {
                                int firstNumber = Convert.ToInt32(firstFNum.ToString() + secondFNum.ToString());
                                int secondNumber = Random.Range(0, firstNumber);
                                string secondNumberString = secondNumber.ToString();
                                if(secondNumber < 10) {
                                    firstSNum = Convert.ToInt32(secondNumberString);
                                    numPow = "1" + powers[firstFNum] + powers[secondFNum] + operators[1] + powers[firstSNum];
                                } else {
                                    firstSNum = Convert.ToInt32(secondNumberString[0]);
                                    secondSNum = Convert.ToInt32(secondNumberString[1]);
                                    numPow = "1" + powers[firstFNum] + powers[secondFNum] + operators[1] + powers[firstSNum] + powers[secondSNum];
                                }
                            }
                        }
                    }
                } else {
                    for(int i = 1; i <= x; i++) {
                        float num = Mathf.Pow(x, 1f / i);
                        if(num == Mathf.RoundToInt(num)) {
                            pows.Add(new PowNum(i, Mathf.RoundToInt(num)));
                        }
                    }
                    int randChoice = Random.Range(0, pows.Count);
                    int totalPower = pows[randChoice].power;
                    numPow = pows[randChoice].num.ToString();

                    int firstPowNum;
                    int secondPowNum;
                    if(isAdd) {
                        firstPowNum = Random.Range(0, totalPower + 1);
                        secondPowNum = totalPower - firstPowNum;
                    } else {
                        firstPowNum = Random.Range(totalPower, totalPower + 21);
                        secondPowNum = firstPowNum - totalPower;
                    }

                    string firstPowString = firstPowNum.ToString();
                    for(int i=0; i<firstPowString.Length; i++) {
                        numPow += powers[Convert.ToInt32(char.GetNumericValue(firstPowString[i]))];
                    }

                    if(isAdd) {
                        numPow += operators[0];
                    } else {
                        numPow += operators[1];
                    }

                    string secondPowString = secondPowNum.ToString();
                    for(int i = 0; i < secondPowString.Length; i++) {
                        numPow += powers[Convert.ToInt32(char.GetNumericValue(secondPowString[i]))];
                    }
                }
                return numPow;
            case 4:
                if(x==1) {
                    int firstFNum = Random.Range(0, 10);
                    int secondFNum = Random.Range(0, 10);

                    int firstSNum = Random.Range(0, 10);
                    int secondSNum = Random.Range(0, 10);
                    if(firstFNum == 0 && firstSNum == 0) {
                        numPow = Random.Range(1, 51).ToString() + powers[firstFNum] + operators[2] + powers[firstSNum];
                    } else {
                        if(!isDouble) {
                            numPow = "1" + powers[firstFNum] + operators[2] + powers[firstSNum];
                        } else {
                            if(firstFNum == 0 && firstSNum != 0) {
                                numPow = "1" + powers[firstFNum] + operators[2] + powers[firstSNum] + powers[secondSNum];
                            } else if(firstFNum != 0 && firstSNum == 0) {
                                numPow = "1" + powers[firstFNum] + powers[secondFNum] + operators[2] + powers[firstSNum];
                            } else {
                                numPow = "1" + powers[firstFNum] + powers[secondFNum] + operators[2] + powers[firstSNum] + powers[secondSNum];
                            }
                        }
                    }
                } else {
                    for(int i = 1; i <= x; i++) {
                        float num = Mathf.Pow(x, 1f / i);
                        if(num == Mathf.RoundToInt(num)) {
                            pows.Add(new PowNum(i, Mathf.RoundToInt(num)));
                        }
                    }
                    int randChoice = Random.Range(0, pows.Count);
                    int totalPower = pows[randChoice].power;
                    numPow = pows[randChoice].num.ToString();

                    List<Vector2Int> products = new List<Vector2Int>();
                    for(int i = 1; i <= Mathf.CeilToInt(Mathf.Sqrt(totalPower)); i++) {
                        if(totalPower % i == 0) {
                            products.Add(new Vector2Int(i, totalPower / i));
                        }
                    }

                    randChoice = Random.Range(0, products.Count);
                    int firstPow = products[randChoice].x;
                    int secondPow = products[randChoice].y;

                    string firstPowString = firstPow.ToString();
                    for(int i = 0; i < firstPowString.Length; i++) {
                        numPow += powers[Convert.ToInt32(char.GetNumericValue(firstPowString[i]))];
                    }

                    numPow += operators[2];

                    string secondPowString = secondPow.ToString();
                    for(int i = 0; i < secondPowString.Length; i++) {
                        numPow += powers[Convert.ToInt32(char.GetNumericValue(secondPowString[i]))];
                    }
                }

                return numPow;
            default:
                return x.ToString();
        }
    }
}
