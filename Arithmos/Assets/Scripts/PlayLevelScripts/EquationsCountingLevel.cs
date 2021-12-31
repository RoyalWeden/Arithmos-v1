using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System;

public class EquationsCountingLevel : PlayCountingLevel {

    protected override void LoadAllNumbers() {
        secondsToPlay = Mathf.Pow(1.1f, Mathf.Pow(1.05f, levelNum)) * levelNum * 35;

        secondsForCombo = 4;

        totalNumbers = levelNum * 1;
        numbersOnPage = new List<int>();

        numbersPerPage = 5.0f;
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

    enum NumberType { add=0, subtract=1, multiply=2, divideBy=3, dividedBy=4 };
    struct Number {
        public bool leftOfEqual;
        public bool isByX;
        public int position;
        public int number;
        public int secondNum;
        public NumberType numbertype;
        public int tag;

        public List<string> sepNum;
    }

    protected override string GenerateNumberToPlace(int x) {
        float unknownVal = .5f;
        string leftSide = "";
        string rightSide = "";
        string[] superNums = { "⁰", "¹", "²", "³", "⁴", "⁵", "⁶", "⁷", "⁸", "⁹", "ˣ" };
        string[] subNums = { "₀", "₁", "₂", "₃", "₄", "₅", "₆", "₇", "₈", "₉", "ₓ" };
        string[] operators = { "+", "-", "×", "⁺", "₊", "⁻", "₋", "⁄" };

        int numberElements = Random.Range(1, 5);
        // TYPES OF ELEMENTS:  +()  |   -()  |  ()x  |  ˣ⁄₍₎  |  ⁽⁾⁄ₓ  

        // Make sure unknown value is an whole number
        while(unknownVal != Mathf.RoundToInt(unknownVal)) {

        
            // Create numbers
            List<Number> numbers = new List<Number>();
            List<Number> leftNumbers = new List<Number>();
            List<Number> rightNumbers = new List<Number>();
            for(int i=0; i<numberElements; i++) {
                Number num = new Number();
                num.leftOfEqual = Random.Range(0, 2) == 0;

                bool isOtherByX = false;
                foreach(Number n in numbers) {
                    if(n.isByX) {
                        isOtherByX = true;
                        break;
                    }
                }
                if(!isOtherByX && num.leftOfEqual) {
                    num.isByX = Random.Range(0, 2) == 0;
                }

                num.number = Random.Range(1, (x * 2) + 4);
                num.secondNum = Random.Range(1, (x * 2) + 4);

                num.numbertype = (NumberType)Random.Range(0, 5);
                if(num.isByX) {
                    if(num.numbertype == NumberType.subtract) {
                        num.number = Random.Range(0, x + 1);
                    } else if(num.numbertype == NumberType.divideBy) {
                        while(x % num.number != 0) {
                            num.number = Random.Range(1, (x * 2) + 4);
                        }
                    } else if(num.numbertype == NumberType.dividedBy) {
                        while(num.number % x != 0) {
                            num.number = Random.Range(1, (x * 2) + 4);
                        }
                    }
                } else {
                    if(num.numbertype == NumberType.divideBy) {
                        while(num.secondNum % num.number != 0) {
                            num.number = Random.Range(1, (x * 2) + 4);
                        }
                    } else if(num.numbertype == NumberType.dividedBy) {
                        while(num.number % num.secondNum != 0) {
                            num.secondNum = Random.Range(1, (x * 2) + 4);
                        }
                    }
                }

                num.tag = i;

                num.sepNum = new List<string>();

                numbers.Add(num);
            }

            // Put numbers in positions
            for(int i=0; i<numbers.Count; i++) {
                int topPosition = 0;
                if(numbers[i].leftOfEqual) {
                    topPosition = 0;
                } else {
                    topPosition = 1;
                }
                foreach(Number n in numbers) {
                    if(numbers[i].tag != n.tag) {
                        if(numbers[i].leftOfEqual == n.leftOfEqual) {
                            if(numbers[i].position >= 0) {
                                topPosition++;
                            }
                        }
                    }
                }
                Number num = numbers[i];
                num.position = topPosition;
                numbers[i] = num;
                if(num.leftOfEqual) {
                    leftNumbers.Add(numbers[i]);
                } else {
                    rightNumbers.Add(numbers[i]);
                }
            }

            leftSide = "";
            rightSide = "";
            // Get total values
            for(int i=0; i<numbers.Count; i++) {
                Number num = numbers[i];
                switch(num.numbertype) {
                    case NumberType.add:
                        if(num.isByX) {
                            if(num.position != 0) {
                                num.sepNum.Add(operators[0]);
                            }
                            num.sepNum.Add("(");
                            num.sepNum.Add("x");
                            num.sepNum.Add(operators[0]);
                            num.sepNum.Add(num.number.ToString());
                            num.sepNum.Add(")");
                        } else {
                            if(num.position != 0) {
                                num.sepNum.Add(operators[0]);
                                num.sepNum.Add(num.number.ToString());
                            } else {
                                num.sepNum.Add(num.number.ToString());
                                num.sepNum.Add(operators[0]);
                            }
                        }
                        break;
                    case NumberType.subtract:
                        if(num.isByX) {
                            if(num.position != 0) {
                                num.sepNum.Add(operators[0]);
                            }
                            num.sepNum.Add("(");
                            num.sepNum.Add("x");
                            num.sepNum.Add(operators[1]);
                            num.sepNum.Add(num.number.ToString());
                            num.sepNum.Add(")");
                        } else {
                            if(num.position != 0) {
                                num.sepNum.Add(operators[1]);
                                num.sepNum.Add(num.number.ToString());
                            } else {
                                num.sepNum.Add(num.number.ToString());
                                num.sepNum.Add(operators[1]);
                            }
                        }
                        break;
                    case NumberType.multiply:
                        if(num.isByX) {
                            if(num.position != 0) {
                                num.sepNum.Add(operators[0]);
                            }
                            num.sepNum.Add("(");
                            num.sepNum.Add("x");
                            num.sepNum.Add(operators[2]);
                            num.sepNum.Add(num.number.ToString());
                            num.sepNum.Add(")");
                        } else {
                            if(num.position != 0) {
                                num.sepNum.Add(operators[2]);
                                num.sepNum.Add(num.number.ToString());
                            } else {
                                num.sepNum.Add(num.number.ToString());
                                num.sepNum.Add(operators[2]);
                            }
                        }
                        break;
                    case NumberType.divideBy:
                        if(num.position != 0) {
                            num.sepNum.Add(operators[0]);
                        }
                        if(num.isByX) {
                            num.sepNum.Add("(");
                            num.sepNum.Add(superNums[10]);
                            num.sepNum.Add(operators[7]);
                            string stringNumber = num.number.ToString();
                            string tempNum = "";
                            for(int j = 0; j < stringNumber.Length; j++) {
                                tempNum += subNums[Convert.ToInt32(stringNumber[j].ToString())];
                            }
                            num.sepNum.Add(tempNum);
                            num.sepNum.Add(")");
                        } else {
                            string tempNum = "";
                            string stringSecondNumber = num.secondNum.ToString();
                            tempNum = "";
                            for(int j = 0; j < stringSecondNumber.Length; j++) {
                                tempNum += superNums[Convert.ToInt32(stringSecondNumber[j].ToString())];
                            }
                            num.sepNum.Add(tempNum);
                            num.sepNum.Add(operators[7]);
                            string stringNumber = num.number.ToString();
                            tempNum = "";
                            for(int j = 0; j < stringNumber.Length; j++) {
                                tempNum += subNums[Convert.ToInt32(stringNumber[j].ToString())];
                            }
                            num.sepNum.Add(tempNum);
                            if(num.position == 0) {
                                num.sepNum.Add(operators[0]);
                            }
                        }
                        break;
                    case NumberType.dividedBy:
                        if(num.position != 0) {
                            num.sepNum.Add(operators[0]);
                        }
                        if(num.isByX) {
                            num.sepNum.Add("(");
                            string stringNumber = num.number.ToString();
                            string tempNum = "";
                            for(int j = 0; j < stringNumber.Length; j++) {
                                tempNum += superNums[Convert.ToInt32(stringNumber[j].ToString())];
                            }
                            num.sepNum.Add(tempNum);
                            num.sepNum.Add(operators[7]);
                            num.sepNum.Add(subNums[10]);
                            num.sepNum.Add(")");
                        } else {
                            string stringNumber = num.number.ToString();
                            string tempNum = "";
                            for(int j = 0; j < stringNumber.Length; j++) {
                                tempNum += superNums[Convert.ToInt32(stringNumber[j].ToString())];
                            }
                            num.sepNum.Add(tempNum);
                            num.sepNum.Add(operators[7]);
                            string stringSecondNumber = num.secondNum.ToString();
                            tempNum = "";
                            for(int j = 0; j < stringSecondNumber.Length; j++) {
                                tempNum += subNums[Convert.ToInt32(stringSecondNumber[j].ToString())];
                            }
                            num.sepNum.Add(tempNum);
                            if(num.position == 0) {
                                num.sepNum.Add(operators[0]);
                            }
                        }
                        break;
                }
            
                numbers[i] = num;
                if(num.leftOfEqual) {
                    leftNumbers[num.position] = numbers[i];
                } else {
                    rightNumbers[num.position - 1] = numbers[i];
                }

                /*string leftS = "";
                string rightS = "";

                foreach(string s in num.sepNum) {
                    if(num.leftOfEqual) {
                        leftS += s;
                    } else {
                        rightS += s;
                    }
                }

                if(num.leftOfEqual) {
                    leftSide += leftS;
                } else {
                    rightSide += rightS;
                }*/
            }

            leftNumbers.Sort(SortByPosition);
            rightNumbers.Sort(SortByPosition);

            foreach(Number num in leftNumbers) {
                foreach(string s in num.sepNum) {
                    leftSide += s;
                }
            }
            foreach(Number num in rightNumbers) {
                foreach(string s in num.sepNum) {
                    rightSide += s;
                }
            }

            if(leftSide.Length > 0 && (leftSide[0].ToString() == operators[0] || leftSide[0].ToString() == operators[1] || leftSide[0].ToString() == operators[2])) {
                string tempLeftSide = leftSide;
                leftSide = "";
                for(int i=1; i<tempLeftSide.Length; i++) {
                    leftSide += tempLeftSide[i];
                }
            }

            bool isAnyByX = false;
            foreach(Number num in numbers) {
                if(num.isByX) {
                    isAnyByX = true;
                    break;
                }
            }
            if(!isAnyByX) {
                if(leftSide.Length > 0 && leftSide[leftSide.Length-1] != operators[0][0] && leftSide[leftSide.Length - 1] != operators[1][0] && leftSide[leftSide.Length - 1] != operators[2][0] && leftSide[leftSide.Length - 1] != operators[7][0]) {
                    leftSide += "+";
                }
                leftSide += "x";
            }

            List<string> leftSplit = new List<string>();
            List<char> leftPlusMinus = new List<char>();
            int index = 0;
            leftSplit.Add("");
            bool isInParan = false;
            string firstXNum = "";
            string secondXNum = "";
            bool isBeforeOpp = false;
            string oppType = "";
            foreach(char s in leftSide) {
                if(s == '(') {
                    isInParan = true;
                    isBeforeOpp = true;
                } else if(s == ')') {
                    isInParan = false;
                    switch(oppType) {
                        case "+":
                            leftSplit[index] += (int.Parse(firstXNum) + int.Parse(secondXNum)).ToString();
                            break;
                        case "-":
                            leftSplit[index] += (int.Parse(firstXNum) - int.Parse(secondXNum)).ToString();
                            break;
                        case "×":
                            leftSplit[index] += (int.Parse(firstXNum) * int.Parse(secondXNum)).ToString();
                            break;
                        case "⁄":
                            string actualNumber1 = "";
                            foreach(char c in firstXNum) {
                                actualNumber1 += Array.IndexOf(superNums, c.ToString()).ToString();
                            }
                            string actualNumber2 = "";
                            foreach(char c in secondXNum) {
                                actualNumber2 += Array.IndexOf(subNums, c.ToString()).ToString();
                            }
                            leftSplit[index] += (float.Parse(actualNumber1) / float.Parse(actualNumber2)).ToString();
                            break;
                    }
                } else if((s == operators[0][0] || s == operators[1][0]) && !isInParan) {
                    index++;
                    leftSplit.Add("");
                    leftPlusMinus.Add(s);
                } else {
                    if(isInParan) {
                        if(isBeforeOpp) {
                            if(s == operators[0][0] || s == operators[1][0] || s == operators[2][0] || s == operators[7][0]) {
                                isBeforeOpp = false;
                                oppType = s.ToString();
                            } else {
                                if(s == 'x') {
                                    firstXNum += x.ToString();
                                } else if(s == superNums[10][0]) {
                                    string xToString = x.ToString();
                                    foreach(char xPiece in xToString) {
                                        firstXNum += superNums[Convert.ToInt32(xPiece.ToString())];
                                    }
                                } else {
                                    firstXNum += s;
                                }
                            }
                        } else {
                            if(s == 'x') {
                                secondXNum += x.ToString();
                            } else if(s == subNums[10][0]) {
                                string xToString = x.ToString();
                                foreach(char xPiece in xToString) {
                                    secondXNum += subNums[Convert.ToInt32(xPiece.ToString())];
                                }
                            } else {
                                secondXNum += s;
                            }
                        }
                    } else {
                        if(s == 'x') {
                            leftSplit[index] += x.ToString();
                        } else {
                            leftSplit[index] += s;
                        }
                    }
                }
            }

            float[] insideLeftMath = new float[leftSplit.Count];
            for(int i=0; i<leftSplit.Count; i++) {
                if(leftSplit[i].Contains(operators[7])) {
                    string[] nums = new string[] { };
                    string[] newnums = new string[] { };
                    if(leftSplit[i].Contains(operators[2])) {
                        nums = leftSplit[i].Split(operators[2][0]);
                        if(nums[0].Contains(operators[7])) {
                            newnums = nums[0].Split(operators[7][0]);
                        } else {
                            newnums = nums[1].Split(operators[7][0]);
                        }
                    } else {
                        newnums = leftSplit[i].Split(operators[7][0]);
                    }

                    string actualNumber1 = "";
                    foreach(char c in newnums[0]) {
                        actualNumber1 += Array.IndexOf(superNums, c.ToString()).ToString();
                    }
                    string actualNumber2 = "";
                    foreach(char c in newnums[1]) {
                        actualNumber2 += Array.IndexOf(subNums, c.ToString()).ToString();
                    }
                    //Debug.Log(rightSide);
                    //Debug.Log(rightSplit[i]);
                    //Debug.Log(actualNumber1 + " " + actualNumber2);
                    if(leftSplit[i].Contains(operators[2])) {
                        if(nums[0].Contains(operators[7])) {
                            insideLeftMath[i] = float.Parse(nums[1]) * (float.Parse(actualNumber1) / float.Parse(actualNumber2));
                        } else {
                            insideLeftMath[i] = float.Parse(nums[0]) * float.Parse(actualNumber1) / float.Parse(actualNumber2);
                        }
                    } else {
                        insideLeftMath[i] = float.Parse(actualNumber1) / float.Parse(actualNumber2);
                    }
                } else if(leftSplit[i].Contains(operators[2])) {
                    string[] nums = leftSplit[i].Split(operators[2][0]);
                    int mult = 1;
                    foreach(string n in nums) {
                        mult *= Convert.ToInt32(n);
                    }
                    insideLeftMath[i] = mult;
                } else {
                    if(leftSplit[i] != "") {
                        //Debug.Log(leftSplit[i]);
                        insideLeftMath[i] = float.Parse(leftSplit[i]);
                    }
                }
            }

            float totalLeft = 0;
            for(int i=0; i<insideLeftMath.Length; i++) {
                if(i == 0 || leftPlusMinus[i - 1] == operators[0][0]) {
                    totalLeft += insideLeftMath[i];
                } else {
                    totalLeft -= insideLeftMath[i];
                }
            }

            List<string> rightSplit = new List<string>();
            List<char> rightPlusMinus = new List<char>();
            index = -1;
            foreach(char s in rightSide) {
                if(s == operators[0][0] || s == operators[1][0] || s == operators[2][0]) {
                    if(s == operators[2][0]) {
                        if(index==-1) {
                            index++;
                            rightSplit.Add("");
                            rightPlusMinus.Add(s);
                        } else {
                            rightSplit[index] += s;
                        }
                    } else {
                        index++;
                        rightSplit.Add("");
                        rightPlusMinus.Add(s);
                    }
                } else {
                    if(s != '_') {
                        rightSplit[index] += s;
                    }
                }
            }

            float[] insideRightMath = new float[rightSplit.Count];
            for(int i = 0; i < rightSplit.Count; i++) {
                if(rightSplit[i].Contains(operators[7])) {
                    string[] nums = new string[] { };
                    string[] newnums = new string[] { };
                    if(rightSplit[i].Contains(operators[2])) {
                        nums = rightSplit[i].Split(operators[2][0]);
                        if(nums[0].Contains(operators[7])) {
                            newnums = nums[0].Split(operators[7][0]);
                        } else {
                            newnums = nums[1].Split(operators[7][0]);
                        }
                    } else {
                        newnums = rightSplit[i].Split(operators[7][0]);
                    }
                
                    string actualNumber1 = "";
                    foreach(char c in newnums[0]) {
                        actualNumber1 += Array.IndexOf(superNums, c.ToString()).ToString();
                    }
                    string actualNumber2 = "";
                    foreach(char c in newnums[1]) {
                        actualNumber2 += Array.IndexOf(subNums, c.ToString()).ToString();
                    }
                    //Debug.Log(rightSplit[i]);
                    //Debug.Log(rightSide);
                    //Debug.Log(actualNumber1 + " " + actualNumber2);
                    if(rightSplit[i].Contains(operators[2])) {
                        if(nums[0].Contains(operators[7])) {
                            insideRightMath[i] = float.Parse(nums[1]) * (float.Parse(actualNumber1) / float.Parse(actualNumber2));
                        } else {
                            insideRightMath[i] = float.Parse(nums[0]) * float.Parse(actualNumber1) / float.Parse(actualNumber2);
                        }
                    } else {
                        insideRightMath[i] = float.Parse(actualNumber1) / float.Parse(actualNumber2);
                    }
                } else if(rightSplit[i].Contains(operators[2])) {
                    string[] nums = rightSplit[i].Split(operators[2][0]);
                    int mult = 1;
                    foreach(string n in nums) {
                        mult *= Convert.ToInt32(n);
                    }
                    insideRightMath[i] = mult;
                } else {
                    if(rightSplit[i] != "") {
                        insideRightMath[i] = float.Parse(rightSplit[i]);
                    }
                }
            }

            float totalRight = 0;
            if(rightPlusMinus.Count>0) {
                if(rightPlusMinus[0] == operators[1][0]) {
                    totalRight = -insideRightMath[0];
                } else {
                    totalRight = insideRightMath[0];
                }
                for(int i = 1; i < insideRightMath.Length; i++) {
                    if(rightPlusMinus[i] == operators[0][0]) {
                        totalRight += insideRightMath[i];
                    } else {
                        totalRight -= insideRightMath[i];
                    }
                }
            }

            if(rightPlusMinus.Count>0) {
                if(rightPlusMinus[0] == operators[2][0]) {
                    unknownVal = totalLeft / totalRight;
                } else {
                    unknownVal = totalLeft - totalRight;
                }
            } else {
                unknownVal = totalLeft;
            }
        }
        Debug.Log("Number " + x.ToString() + ": " + leftSide.ToString() + " = " + unknownVal.ToString() + rightSide.ToString());
        //Debug.Log(rightSide + " = " + totalRight);

        return leftSide.ToString() + " = " + unknownVal.ToString() + rightSide.ToString();
    }

    int SortByPosition(Number num1, Number num2) {
        return num1.position.CompareTo(num2.position);
    }
}
