using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using UnityEditor;
using Object = UnityEngine.Object;
using System.Linq;

public class PlayCountingLevel : MonoBehaviour {

    Slider starSlider;
    int starCount;

    Canvas canvas;

    Image threeTwoStarsBar;
    Image twoOneStarsBar;
    Color32 twoBarColor;
    Color32 oneBarColor;

    Sprite emptyStar;
    Sprite fullStar;

    Image starBar;
    Sprite[] starColoredBars;

    public static bool isInLevel;

    public static int levelNum;
    Text levelText;

    public enum LevelType { regular = 0, addition = 1, subtraction = 2, multiplication = 3, division = 4, power = 5, equation = 6 };
    public static LevelType leveltype;

    protected float secondsToPlay;
    float startTime;

    float secondsStartLastNum;
    float secondsSinceLastNum;
    int numClickCombo;

    public float secondsForCombo;
    Text comboIndicator;
    Text comboIndicatorCount;
    string[] comboText;

    Text numberTxtPrefab;
    GameObject numbersPanel;
    Vector2 topLeftNumPanel;
    Vector2 bottomRightNumPanel;

    protected int totalNumbers;
    protected List<Text> numTextList;
    int numbersFound;

    bool isGameOver;
    bool isPaused;
    bool showGameOverOnce;

    Button pauseButton;

    float startPauseTime;
    float currentPauseTime;
    float totalPauseTime;

    GameObject pausePanel;
    Button leftPauseButton;
    Button rightPauseButton;
    GameObject[] pauseButtonObjs;

    GameObject hintButtonObj;
    GameObject freezeButtonObj;
    GameObject fireButtonObj;
    GameObject magnetButtonObj;
    GameObject blackholeButtonObj;
    GameObject scissorsButtonObj;

    Text hintCountText;
    Text freezeCountText;
    Text fireCountText;
    Text magnetCountText;
    Text blackholeCountText;
    Text scissorsCountText;

    PauseShowing pauseShow;

    enum PauseShowing {
        pausebuttons = 0,
        upgradebuttons = 1
    }

    GameObject lvlCompletePanel;

    GameObject starsObj;
    Image[] stars;
    GameObject[] fallInStars;

    protected float numbersPerPage;
    protected int totalPages;
    protected List<int> numbersOnPage;
    int currentPage;
    int leftBoundNumRange;
    int rightBoundNumRange;

    protected GameObject pagesPanel;
    Text pageText;

    int coinsEarned;
    GameObject coinsEarnedObj;
    Text numberOfCoinsText;

    Button doubleCoinsButton;

    AnimationClip showHintAnim;
    bool isHintPlaying;

    GameObject FrozenObj;
    bool isTimeFrozen;
    bool isFreezeIning;
    bool isFreezeOuting;
    float startFreezeTime;
    float currentFreezeTime;
    float totalFreezeTime;
    float totalAllowedFreezeTime;

    GameObject fireObj;
    Sprite[] fireSprites;
    int curFireSprite;
    GameObject fireTextToBurn;
    float startFireTime;
    bool isBurning;
    bool fireRunning;
    bool fireCorotineRunning;
    int totalNumbersToBurn;

    GameObject magnetObj;
    bool isMagnetActive;
    bool isMagnetCorotineOn;
    bool hasMagnetMovedForw;
    bool canMagnetMoveBack;
    List<Text> magnetTexts;

    GameObject blackholeObj;
    bool isBlackholeActive;
    bool hasBlackholeAnimStarted;
    float suckSpeed;
    float startBlackholeTime;
    float totalBlackholeTime;
    bool hasUsedBlackholeOnce;

    GameObject scissorsObj;
    bool isScissorsActive;
    bool hasScissorsFlownIn;
    bool isScissorsCutting;
    float startScissorsTime;
    float totalScissorsTime;
    int pagesToCut;

    GameObject upgradeButtonObj;
    Sprite upgradeSprite;
    Sprite lockedSprite;

    enum UpgradeType { hint, magnet, freeze, fire, blackhole, scissors };

    enum WeatheringType { tornado=0, gravity=1, whirlpol=2, revision=3, merge=4 };
    WeatheringType weatheringtype;
    float timeSinceWeatheringStart;
    float[] weatheringTypeChances;
    bool doWeathingEffect;

    GameObject tornadoObj;
    bool isTornadoActive;
    bool hasTornadoMoved;
    bool isToronadoCoroutine;

    bool activateGravity;
    GameObject gravityBounds;
    PhysicsMaterial2D gravityPhysicsMat;

    float reduceTimeWrong;
    bool canGetWrong;
    bool gotNumRight;

    void Start() {
        canvas = transform.parent.GetComponent<Canvas>();
        numbersPanel = transform.Find("NumbersPanel").gameObject;

        pauseButton = transform.Find("PauseButton").GetComponent<Button>();
        pauseButton.onClick.AddListener(PauseGame);

        emptyStar = GetSprite("GUI Sprites/GameGUIEasyFREEApart", "GameGUIEasyFREEApart_159");
        fullStar = GetSprite("GUI Sprites/GameGUIEasyFREEApart", "GameGUIEasyFREEApart_84");

        starColoredBars = new Sprite[] { GetSprite("GUI Sprites/GameGUIEasyFREEApart", "GameGUIEasyFREEApart_31"),
                                         GetSprite("GUI Sprites/GameGUIEasyFREEApart", "GameGUIEasyFREEApart_30"),
                                         GetSprite("GUI Sprites/GameGUIEasyFREEApart", "GameGUIEasyFREEApart_32")};

        numberTxtPrefab = Resources.Load<Text>("Prefabs/Number");

        starSlider = transform.Find("StarsIndicator").GetComponent<Slider>();
        threeTwoStarsBar = starSlider.transform.Find("ThreeTwoStars").GetComponent<Image>();
        twoOneStarsBar = starSlider.transform.Find("TwoOneStars").GetComponent<Image>();
        starBar = starSlider.transform.Find("StarBar").GetComponent<Image>();

        levelText = transform.Find("LevelPanel").Find("Text").GetComponent<Text>();

        comboIndicator = starSlider.transform.Find("ComboIndicator").GetComponent<Text>();
        comboIndicatorCount = comboIndicator.transform.Find("ComboIndicatorCount").GetComponent<Text>();

        pagesPanel = transform.Find("TotalPagesPanel").gameObject;
        pageText = pagesPanel.transform.Find("Text").GetComponent<Text>();

        pausePanel = canvas.transform.Find("PausedPanel").gameObject;
        leftPauseButton = pausePanel.transform.Find("Panel").Find("RotatePauseLeft").GetComponent<Button>();
        leftPauseButton.onClick.AddListener(delegate { ChangePauseShowing(-1); });
        rightPauseButton = pausePanel.transform.Find("Panel").Find("RotatePauseRight").GetComponent<Button>();
        rightPauseButton.onClick.AddListener(delegate { ChangePauseShowing(1); });
        pauseButtonObjs = new GameObject[]{ pausePanel.transform.Find("Panel").Find("PauseButtonGrid").gameObject,
                                            pausePanel.transform.Find("Panel").Find("UpgradeButtonGrid").gameObject};

        pauseButtonObjs[0].transform.Find("UnPauseButton").GetComponent<Button>().onClick.AddListener(unPauseGame);

        hintButtonObj = pauseButtonObjs[1].transform.Find("UseHint").gameObject;
        hintButtonObj.GetComponent<Button>().onClick.AddListener(SetShowHint);
        hintCountText = hintButtonObj.transform.Find("HintCount").GetComponent<Text>();
        showHintAnim = Resources.Load<AnimationClip>("Animations/Upgrades/ShowHint");

        freezeButtonObj = pauseButtonObjs[1].transform.Find("UseFreeze").gameObject;
        freezeButtonObj.GetComponent<Button>().onClick.AddListener(SetFreezeTime);
        freezeCountText = freezeButtonObj.transform.Find("FreezeCount").GetComponent<Text>();

        fireButtonObj = pauseButtonObjs[1].transform.Find("UseFire").gameObject;
        fireButtonObj.GetComponent<Button>().onClick.AddListener(SetStartFire);
        fireCountText = fireButtonObj.transform.Find("FireCount").GetComponent<Text>();
        fireSprites = Resources.LoadAll<Sprite>("Other Sprites/Flame");

        magnetButtonObj = pauseButtonObjs[1].transform.Find("UseMagnet").gameObject;
        magnetButtonObj.GetComponent<Button>().onClick.AddListener(SetShowMagnet);
        magnetCountText = magnetButtonObj.transform.Find("MagnetCount").GetComponent<Text>();

        blackholeButtonObj = pauseButtonObjs[1].transform.Find("UseBlackhole").gameObject;
        blackholeButtonObj.GetComponent<Button>().onClick.AddListener(SetPlaceBlackhole);
        blackholeCountText = blackholeButtonObj.transform.Find("BlackholeCount").GetComponent<Text>();

        scissorsButtonObj = pauseButtonObjs[1].transform.Find("UseScissors").gameObject;
        scissorsButtonObj.GetComponent<Button>().onClick.AddListener(SetShowScissors);
        scissorsCountText = scissorsButtonObj.transform.Find("ScissorsCount").GetComponent<Text>();

        tornadoObj = numbersPanel.transform.Find("Tornado").gameObject;

        gravityBounds = numbersPanel.transform.Find("GravityBounds").gameObject;
        gravityPhysicsMat = (PhysicsMaterial2D)Resources.Load("GravityWeatheringPhysics");

        lvlCompletePanel = canvas.transform.Find("LevelCompletePanel").gameObject;

        starsObj = lvlCompletePanel.transform.Find("Panel").Find("Stars").gameObject;
        stars = new Image[] { starsObj.transform.Find("star").GetComponent<Image>(),
                              starsObj.transform.Find("star (1)").GetComponent<Image>(),
                              starsObj.transform.Find("star (2)").GetComponent<Image>()};
        fallInStars = new GameObject[] { starsObj.transform.Find("starFull").gameObject,
                                         starsObj.transform.Find("starFull (1)").gameObject,
                                         starsObj.transform.Find("starFull (2)").gameObject};

        coinsEarnedObj = lvlCompletePanel.transform.Find("Panel").Find("CoinsEarnedObj").gameObject;
        numberOfCoinsText = coinsEarnedObj.transform.Find("numberOfCoins").GetComponent<Text>();

        doubleCoinsButton = lvlCompletePanel.transform.Find("Panel").Find("2xCoinsButton").GetComponent<Button>();
        doubleCoinsButton.onClick.AddListener(WatchAdDoubleCoins);

        upgradeButtonObj = lvlCompletePanel.transform.Find("Panel").Find("ButtonGrid").Find("UpgradesButton").gameObject;
        upgradeSprite = GetSprite("GUI Sprites/GameGUIEasyFREEApart", "GameGUIEasyFREEApart_230");
        lockedSprite = GetSprite("GUI Sprites/GameGUIEasyFREEApart", "GameGUIEasyFREEApart_122");

        FrozenObj = starSlider.transform.Find("FrozenObject").gameObject;
        fireObj = numbersPanel.transform.Find("Fire").gameObject;
        magnetObj = numbersPanel.transform.Find("Magnet").gameObject;
        blackholeObj = numbersPanel.transform.Find("Blackhole").gameObject;
        scissorsObj = canvas.transform.Find("Scissors").gameObject;

        levelText.text = "Level " + levelNum.ToString();

        pauseShow = 0;
        ChangePauseShowing(0);

        twoBarColor = new Color32(75, 102, 11, 255);
        oneBarColor = new Color32(229, 140, 6, 255);

        topLeftNumPanel.x = (-numbersPanel.GetComponent<BoxCollider2D>().size.x / 2) + 15;
        topLeftNumPanel.y = (numbersPanel.GetComponent<BoxCollider2D>().size.y / 2) - 15;

        bottomRightNumPanel.x = (numbersPanel.GetComponent<BoxCollider2D>().size.x / 2) - 15;
        bottomRightNumPanel.y = (-numbersPanel.GetComponent<BoxCollider2D>().size.y / 2) + 15;

        comboText = new string[]{"Smart", "Genius", "Nice", "Great", "Excellent", "Insane", "On Fire", "Cool", "Fantastic",
                                 "Surperb", "Outstanding", "Great Job", "Amazing", "Perfect", "Spectacular"};

        starCount = 3;

        reduceTimeWrong = 1;
        canGetWrong = true;

        LoadAllNumbers();
        ShowAchievements.canShowAchievements = false;

        startTime = Time.time;
        secondsStartLastNum = Time.time;

        timeSinceWeatheringStart = Time.time; // chance happens every 20 seconds
        weatheringTypeChances = new float[] { .5f, .4f, .25f, .1f, .025f, .01f };
        for(int i = 0; i < weatheringTypeChances.Length; i++) {
            if(levelNum > SaveSystem.data.maxLevel / 3) {
                weatheringTypeChances[i] *= levelNum;
            } else {
                weatheringTypeChances[i] = 0;
            }
        }
    }

    void Update() {
        if(starCount < 3) {
            threeTwoStarsBar.enabled = false;
            twoOneStarsBar.color = oneBarColor;
        } else {
            threeTwoStarsBar.enabled = true;
            threeTwoStarsBar.color = twoBarColor;
            twoOneStarsBar.color = twoBarColor;
        }

        if(starCount < 2) {
            twoOneStarsBar.enabled = false;
        } else {
            twoOneStarsBar.enabled = true;
        }

        if(starCount > 0) {
            starBar.sprite = starColoredBars[starCount - 1];
        }

        if(SaveSystem.data.unlockedAchievements[0]) {
            upgradeButtonObj.GetComponent<Button>().enabled = true;
            upgradeButtonObj.GetComponent<Image>().sprite = upgradeSprite;
        } else {
            upgradeButtonObj.GetComponent<Button>().enabled = false;
            upgradeButtonObj.GetComponent<Image>().sprite = lockedSprite;
        }

        // fix start time
        if(Time.time - (startTime + totalPauseTime + totalFreezeTime) < 0) {
            startTime = Time.time - totalPauseTime - totalFreezeTime;
        }

        // touched number
        if(Input.touchCount == 1 && !isGameOver && !isPaused && !isBlackholeActive && !isScissorsActive) {
            Touch touch = Input.GetTouch(0);
            if(numTextList.Count > 0) {
                Text t = numTextList[0];
                if(t.GetComponent<BoxCollider2D>().bounds.Contains(touch.position)) {
                    if(isHintPlaying) {
                        isHintPlaying = false;
                    }
                    StartCoroutine("PressedCorrectText", t);

                    gotNumRight = true;
                    StartCoroutine("PausePossibleWrong");
                    reduceTimeWrong = 1;

                    CheckCombo();
                } else {
                    foreach(Text wrongText in numTextList) {
                        if(wrongText.GetComponent<BoxCollider2D>().bounds.Contains(touch.position) && !gotNumRight && canGetWrong) {
                            wrongText.GetComponent<Animation>().Play("WrongNumber");
                            startTime -= reduceTimeWrong;
                            reduceTimeWrong *= 1.5f;
                            canGetWrong = false;
                            numClickCombo = 0;
                            break;
                        }
                    }
                }
            }
        }

        if(Input.touchCount==0) {
            bool isWrongNumberAnimPlaying = false;
            foreach(Text wrongText in numTextList) {
                if(wrongText.GetComponent<Animation>().isPlaying) {
                    isWrongNumberAnimPlaying = true;
                    break;
                }
            }
            if(!isWrongNumberAnimPlaying && !gotNumRight) {
                canGetWrong = true;
            } else {
                canGetWrong = false;
            }
        }

        // level is/isn't paused
        if(isPaused) {
            if(startPauseTime == 0) {
                startPauseTime = Time.time;

                foreach(Text t in numTextList) {
                    t.GetComponent<Animation>().enabled = false;
                }
            }
            currentPauseTime = Time.time - startPauseTime;

            if(pauseShow == PauseShowing.upgradebuttons) {
                showUpgradeButton(SaveSystem.data.numHints, hintButtonObj, hintCountText, UpgradeType.hint);
                showUpgradeButton(SaveSystem.data.numFreezes, freezeButtonObj, freezeCountText, UpgradeType.freeze);
                showUpgradeButton(SaveSystem.data.numFires, fireButtonObj, fireCountText, UpgradeType.fire);
                showUpgradeButton(SaveSystem.data.numMagnets, magnetButtonObj, magnetCountText, UpgradeType.magnet);
                showUpgradeButton(SaveSystem.data.numBlackholes, blackholeButtonObj, blackholeCountText, UpgradeType.blackhole);
                showUpgradeButton(SaveSystem.data.numScissors, scissorsButtonObj, scissorsCountText, UpgradeType.scissors);
            }

            if(isBlackholeActive) {
                blackholeObj.GetComponent<Animation>().enabled = false;
            }

            if(isTornadoActive) {
                tornadoObj.GetComponent<Animation>().enabled = false;
            }
        } else {
            ShowNumberHint();

            if(isBurning) {
                startFireTime += currentPauseTime;
            }
            if(isTimeFrozen) {
                startFreezeTime += currentPauseTime;
            }
            if(isBlackholeActive) {
                startBlackholeTime += currentPauseTime;
                blackholeObj.GetComponent<Animation>().enabled = true;
            }
            if(isScissorsActive) {
                scissorsObj.transform.SetAsLastSibling();
                startScissorsTime += currentPauseTime;
            }
            timeSinceWeatheringStart += currentPauseTime;

            if(isTornadoActive) {
                if(!isToronadoCoroutine) {
                    tornadoObj.GetComponent<Animation>().enabled = true;
                    StartCoroutine("DoTornado");
                }
            }
            
            if(activateGravity) {
                activateGravity = false;
                gravityBounds.SetActive(true);
                foreach(Text t in numTextList) {
                    t.GetComponent<BoxCollider2D>().isTrigger = false;
                    if(t.GetComponent<Rigidbody2D>() == null) {
                        t.gameObject.AddComponent<Rigidbody2D>();
                        t.GetComponent<Rigidbody2D>().gravityScale = Random.Range(2f, 5f);
                        t.GetComponent<Rigidbody2D>().mass = Random.Range(3f, 20f);
                        t.GetComponent<Rigidbody2D>().sharedMaterial = gravityPhysicsMat;
                    }
                }
            }

            if(startPauseTime > 0) {
                totalPauseTime += currentPauseTime;
                startPauseTime = 0;
                currentPauseTime = 0;

                foreach(Text t in numTextList) {
                    t.GetComponent<Animation>().enabled = true;
                }
            }

            if(Time.time - timeSinceWeatheringStart >= 20) {
                timeSinceWeatheringStart = Time.time;
                float randProb = Random.Range(0, 100);
                float prevSum = 0;
                for(int i=0; i<weatheringTypeChances.Length; i++) {
                    if(randProb >= prevSum && randProb < weatheringTypeChances[i]) {
                        weatheringtype = (WeatheringType)i;
                        doWeathingEffect = true;
                        break;
                    } else {
                        prevSum += weatheringTypeChances[i];
                    }
                }
                timeSinceWeatheringStart = Time.time;
            }

            if(doWeathingEffect) {
                switch(weatheringtype) {
                    case WeatheringType.tornado:
                        isTornadoActive = true;
                        break;
                    case WeatheringType.gravity:
                        activateGravity = true;
                        break;
                    case WeatheringType.whirlpol:
                        break;
                    case WeatheringType.revision:
                        break;
                    case WeatheringType.merge:
                        break;
                }
                doWeathingEffect = false;
            }

            // time is/isn't frozen
            if(isTimeFrozen) {
                if(startFreezeTime == 0) {
                    startFreezeTime = Time.time;
                }
                if(isFreezeIning) {
                    if(FrozenObj.GetComponent<Animation>().isPlaying) {
                        FrozenObj.GetComponent<Animation>().Stop();
                    }
                    FrozenObj.GetComponent<Animation>().Play("IceFadeIn");
                    isFreezeIning = false;
                }
                currentFreezeTime = Time.time - startFreezeTime;

                if(currentFreezeTime >= totalAllowedFreezeTime) {
                    isTimeFrozen = false;
                } else if(currentFreezeTime >= totalAllowedFreezeTime - 3 && !isFreezeOuting) {
                    isFreezeOuting = true;
                    FrozenObj.GetComponent<Animation>().Play("IceFadeOut");
                }
            } else {
                if(startFreezeTime > 0) {
                    isFreezeOuting = false;
                    totalFreezeTime += currentFreezeTime;
                    startFreezeTime = 0;
                }
            }

            // burn numbers
            if(isBurning && numTextList.Count > 0) {
                setFires();
            }

            if(isMagnetActive && !isMagnetCorotineOn) {
                StartCoroutine("MoveMagnet");
            }

            if(isBlackholeActive) {
                PlayBlackhole();
            }

            if(isScissorsActive) {
                StartCoroutine("CutWithScissors");
            }

            if(numbersFound == totalNumbers || (TimeLeftPercent() == 0 && !isTimeFrozen)) {
                isGameOver = true;
            }
        }

        // level is complete
        if(isGameOver && !showGameOverOnce) {
            StartCoroutine("GameIsOver");
        }

        // show number of pages
        if(numTextList.Count == 0 && !isGameOver) {
            PlacePageNumbers();
        }

        // draw star bar
        if(!isGameOver && !isPaused && !isTimeFrozen) {
            starSlider.value = Mathf.Lerp(.351f, 1, TimeLeftPercent());
        }

        // calculate number of stars
        if(starSlider.value > .351f) {
            starCount = Mathf.CeilToInt(Mathf.Lerp(0, 3, (starSlider.value - .351f) / .649f));
        } else {
            starCount = 0;
            starSlider.value = .351f;
        }
    }

    /* PRIVATE METHODS */
    float TimeLeftPercent() {
        float totalWeatheringTime = 0;
        float totalUpgradeTime = totalPauseTime + totalFreezeTime + totalBlackholeTime + totalScissorsTime;
        return 1 - Mathf.Clamp01((Time.time - (startTime + totalUpgradeTime + totalWeatheringTime)) / secondsToPlay);
    }

    IEnumerator PausePossibleWrong() {
        yield return new WaitForSeconds(.4f);
        gotNumRight = false;
    }
    
    IEnumerator PressedCorrectText(Text t) {
        numTextList.Remove(t);
        numbersFound++;
        t.GetComponent<Animation>().Play("CorrectNumber");
        yield return new WaitForSeconds(t.GetComponent<Animation>().GetClip("CorrectNumber").length);
        Destroy(t);
    }

    Sprite GetSprite(string path, string name) {
        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        foreach(Sprite sprite in sprites) {
            if(sprite.name == name) {
                return sprite;
            }
        }
        return null;
    }

    #region Create Numbers
    protected virtual void LoadAllNumbers() { }

    protected virtual string GenerateNumberToPlace(int x) { return ""; }

    protected void PlacePageNumbers() {
        gravityBounds.SetActive(false);

        numbersPanel.transform.Find("TestPosObj").gameObject.SetActive(true);
        currentPage++;
        pageText.text = currentPage.ToString() + "/" + totalPages.ToString();
        if(currentPage == 1) {
            leftBoundNumRange = 0;
        } else {
            leftBoundNumRange = (currentPage - 1) * (int)numbersPerPage;
        }
        if(totalNumbers > currentPage * numbersPerPage) {
            rightBoundNumRange = currentPage * (int)numbersPerPage;
        } else {
            rightBoundNumRange = totalNumbers;
        }

        for(int i = leftBoundNumRange + 1; i <= rightBoundNumRange; i++) {
            Text t = Instantiate(numberTxtPrefab, numbersPanel.transform);

            t.text = GenerateNumberToPlace(i);
            t.transform.SetSiblingIndex(3);

            PlaceNumber(t);

            numTextList.Add(t);
        }
        
        foreach(Text t in numTextList) {
            int breakCount = 0;
            Vector2 textRectSize = t.GetComponent<RectTransform>().rect.size;
            while(!IsNotTouchingOther(t.transform.localPosition, textRectSize)) {
                t.transform.localPosition = new Vector2(Random.Range(topLeftNumPanel.x, bottomRightNumPanel.x), Random.Range(bottomRightNumPanel.y, topLeftNumPanel.y));
                Canvas.ForceUpdateCanvases();
                t.GetComponent<ContentSizeFitter>().SetLayoutVertical();
                breakCount++;
                if(breakCount==5000) {
                    Debug.Log(t.text + " :   ran for 5000 times.");
                    break;
                }
            }
        }
        Debug.Log("------------------FINAL------------------");
        foreach(Text t in numTextList) {
            Debug.Log(t.text + " :   " + (!IsNotTouchingOther(t.transform.localPosition, t.GetComponent<RectTransform>().rect.size)));
        }
        numbersPanel.transform.Find("TestPosObj").gameObject.SetActive(false);
    }

    void PlaceNumber(Text t) {
        t.fontSize = 115;
        Canvas.ForceUpdateCanvases();
        t.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        Vector2 textSize = t.GetComponent<RectTransform>().rect.size;
        t.transform.localPosition = new Vector2(Random.Range(topLeftNumPanel.x+textSize.x/2, bottomRightNumPanel.x-textSize.x/2), Random.Range(bottomRightNumPanel.y+textSize.y/2, topLeftNumPanel.y-textSize.y/2));
        // List<Vector2> possiblePositions = new List<Vector2>();
        // List<int> possibleSizes = new List<int>();

        int testPosCount = 0;
        // Debug.Log(t.transform.localPosition);
        while(testPosCount<100) {
            Vector2 textPos = t.transform.localPosition;
            if(IsNotTouchingOther(textPos, textSize)) {
                break;
            } else {
                t.transform.localPosition = new Vector2(Random.Range(topLeftNumPanel.x+textSize.x/2, bottomRightNumPanel.x-textSize.x/2), Random.Range(bottomRightNumPanel.y+textSize.y/2, topLeftNumPanel.y-textSize.y/2));
                Canvas.ForceUpdateCanvases();
                t.GetComponent<ContentSizeFitter>().SetLayoutVertical();
            }
            testPosCount++;
        }
        t.GetComponent<BoxCollider2D>().size = new Vector2(t.GetComponent<RectTransform>().rect.width, t.GetComponent<RectTransform>().rect.height);

        // while(possibleSizes.Count<100) {
        //     GameObject g = Instantiate(numbersPanel.transform.Find("TestPosObj").gameObject, numbersPanel.transform);
        //     BoxCollider2D gBox = g.GetComponent<BoxCollider2D>();
        //     possiblePositions.Add(new Vector2(Random.Range(topLeftNumPanel.x, bottomRightNumPanel.x), Random.Range(bottomRightNumPanel.y, topLeftNumPanel.y)));
        //     possibleSizes.Add(50);
        //     g.transform.localPosition = possiblePositions[possibleSizes.Count - 1];
        //     gBox.size = new Vector2(possibleSizes[possibleSizes.Count - 1], possibleSizes[possibleSizes.Count - 1]);

        //     while(IsNotTouchingOther(g, gBox)) {
        //         possibleSizes[possibleSizes.Count - 1] += 2;
        //         gBox.size = new Vector2(possibleSizes[possibleSizes.Count - 1], possibleSizes[possibleSizes.Count - 1]);
        //     }
        //     Destroy(g);
        // }
        // int largestInd = possibleSizes.IndexOf(possibleSizes.Max());
        // t.transform.localPosition = possiblePositions[largestInd];

        // float smallestFont = 100f;
        // t.fontSize = (int)smallestFont;
        // Canvas.ForceUpdateCanvases();

        // t.GetComponent<BoxCollider2D>().size = new Vector2(t.GetComponent<RectTransform>().rect.width, t.GetComponent<RectTransform>().rect.height);

        // int largestFont = 175;
        // if(leveltype == LevelType.equation) {
        //     largestFont = Mathf.FloorToInt(largestFont * .75f);
        // }

        // t.fontSize = Random.Range((int)smallestFont, largestFont);
        // if(t.fontSize > 300) {
        //     t.fontSize = 300;
        // } else if(t.fontSize < smallestFont) {
        //     t.fontSize = (int)smallestFont;
        // }
        // Canvas.ForceUpdateCanvases();
        // t.GetComponent<BoxCollider2D>().size = new Vector2(t.GetComponent<RectTransform>().rect.width, t.GetComponent<RectTransform>().rect.height);

        // if(!IsNotTouchingOther(t.gameObject, t.GetComponent<BoxCollider2D>())) {
        //     // t.fontSize -= 5;
        //     Canvas.ForceUpdateCanvases();
        //     t.GetComponent<BoxCollider2D>().size = new Vector2(t.GetComponent<RectTransform>().rect.width, t.GetComponent<RectTransform>().rect.height);
        //     if(leveltype == LevelType.equation) {
        //         Vector2 boxSize = t.GetComponent<BoxCollider2D>().size;
        //         t.transform.localPosition = new Vector2(Random.Range(topLeftNumPanel.x + boxSize.x / 2, bottomRightNumPanel.x - boxSize.x / 2), Random.Range(bottomRightNumPanel.y + boxSize.y / 2, topLeftNumPanel.y - boxSize.y / 2));
        //     }
        // }
    }
    // bool IsNotTouchingOther(GameObject g, BoxCollider2D gBox) {
    bool IsNotTouchingOther(Vector2 textPos, Vector2 textSize) {
        // Vector2 textPos = g.transform.localPosition;
        // Vector2 textSize = gBox.size;

        Vector2 posTopLeftText = new Vector2(textPos.x - textSize.x / 2, textPos.y + textSize.y / 2);
        Vector2 posTopRightText = new Vector2(textPos.x + textSize.x / 2, textPos.y + textSize.y / 2);
        Vector2 posBottomLeftText = new Vector2(textPos.x - textSize.x / 2, textPos.y - textSize.y / 2);
        Vector2 posBottomRightText = new Vector2(textPos.x + textSize.x / 2, textPos.y - textSize.y / 2);

        bool topLeftInsidePanel = posTopLeftText.x > topLeftNumPanel.x && posTopLeftText.x < bottomRightNumPanel.x && posTopLeftText.y < topLeftNumPanel.y && posTopLeftText.y > bottomRightNumPanel.y;
        bool topRightInsidePanel = posTopRightText.x > topLeftNumPanel.x && posTopRightText.x < bottomRightNumPanel.x && posTopRightText.y < topLeftNumPanel.y && posTopRightText.y > bottomRightNumPanel.y;
        bool bottomLeftInsidePanel = posBottomLeftText.x > topLeftNumPanel.x && posBottomLeftText.x < bottomRightNumPanel.x && posBottomLeftText.y < topLeftNumPanel.y && posBottomLeftText.y > bottomRightNumPanel.y;
        bool bottomRightInsidePanel = posBottomRightText.x > topLeftNumPanel.x && posBottomRightText.x < bottomRightNumPanel.x && posBottomRightText.y < topLeftNumPanel.y && posBottomRightText.y > bottomRightNumPanel.y;

        bool isInsideWall = topLeftInsidePanel && topRightInsidePanel && bottomLeftInsidePanel && bottomRightInsidePanel;

        bool topLeftInsideText = false;
        bool topRightInsideText = false;
        bool bottomLeftInsideText = false;
        bool bottomRightInsideText = false;
        bool boxIntersectText = false;

        bool isInsideText = false;

        if(isInsideWall) {
            foreach(Text x in numTextList) {
                Vector2 tempTextPos = x.transform.localPosition;
                if(textPos==tempTextPos) {
                    continue;
                }
                // Vector2 tempTextPos = x.gameObject.GetComponent<RectTransform>().rect.center;
                Vector2 tempTextSize = x.gameObject.GetComponent<RectTransform>().rect.size;
                // Vector2 tempTextSize = x.gameObject.GetComponent<BoxCollider2D>().size;

                Vector2 tempPosTopLeftText = new Vector2(tempTextPos.x - tempTextSize.x / 2, tempTextPos.y + tempTextSize.y / 2);
                Vector2 tempPosTopRightText = new Vector2(tempTextPos.x + tempTextSize.x / 2, tempTextPos.y + tempTextSize.y / 2);
                Vector2 tempPosBottomLeftText = new Vector2(tempTextPos.x - tempTextSize.x / 2, tempTextPos.y - tempTextSize.y / 2);
                Vector2 tempPosBottomRightText = new Vector2(tempTextPos.x + tempTextSize.x / 2, tempTextPos.y - tempTextSize.y / 2);
                
                topLeftInsideText = posTopLeftText.x > tempPosTopLeftText.x && posTopLeftText.x < tempPosBottomRightText.x && posTopLeftText.y < tempPosTopLeftText.y && posTopLeftText.y > tempPosBottomRightText.y;
                topRightInsideText = posTopRightText.x > tempPosTopLeftText.x && posTopRightText.x < tempPosBottomRightText.x && posTopRightText.y < tempPosTopLeftText.y && posTopRightText.y > tempPosBottomRightText.y;
                bottomLeftInsideText = posBottomRightText.x > tempPosTopLeftText.x && posBottomRightText.x < tempPosBottomRightText.x && posBottomRightText.y < tempPosTopLeftText.y && posBottomRightText.y > tempPosBottomRightText.y;
                bottomRightInsideText = posBottomRightText.x > tempPosTopLeftText.x && posBottomRightText.x < tempPosBottomRightText.x && posBottomRightText.y < tempPosTopLeftText.y && posBottomRightText.y > tempPosBottomRightText.y;

                // topLeftInsideText = (posTopLeftText.x >= tempPosTopLeftText.x && posTopLeftText.x <= tempPosTopRightText.x) && (posTopLeftText.y <= tempPosTopLeftText.y && posTopLeftText.y >= tempPosBottomLeftText.y);
                // topRightInsideText = (posTopRightText.x <= tempPosTopRightText.x && posTopRightText.x >= tempPosTopLeftText.x) && (posTopRightText.y <= tempPosTopRightText.y && posTopRightText.y >= tempPosBottomRightText.y);
                // bottomLeftInsideText = (posBottomLeftText.x >= tempPosBottomLeftText.x && posBottomLeftText.x <= tempPosBottomRightText.x) && (posBottomLeftText.y >= tempPosBottomLeftText.y && posBottomLeftText.y <= tempPosTopLeftText.y);
                // bottomRightInsideText = (posBottomRightText.x <= tempPosBottomRightText.x && posBottomRightText.x >= tempPosBottomLeftText.x) && (posBottomRightText.y >= tempPosBottomRightText.y && posBottomRightText.y <= tempPosTopRightText.y);

                //boxIntersectText = x.gameObject.GetComponent<BoxCollider2D>().bounds.Intersects(gBox.bounds);

                isInsideText = topLeftInsideText || topRightInsideText || bottomLeftInsideText || bottomRightInsideText || boxIntersectText;
                if(isInsideText) {
                    break;
                }
            }
        }

        return isInsideWall && !isInsideText;
    }
    #endregion


    /* SET WHEN UPGRADE BUTTONS CAN SHOW/BE USE */
    void showUpgradeButton(int numOf, GameObject buttonObj, Text countText, UpgradeType upgradeType) {
        bool canUseUpgrade = true;
        switch(upgradeType) {
            case UpgradeType.hint:
                if(isMagnetActive || isBurning || isHintPlaying || isBlackholeActive) {
                    canUseUpgrade = false;
                }
                break;
            case UpgradeType.freeze:
                break;
            case UpgradeType.fire:
                if(isMagnetActive || isBlackholeActive) {
                    canUseUpgrade = false;
                }
                break;
            case UpgradeType.magnet:
                if(isMagnetActive || isBurning || isHintPlaying || isBlackholeActive) {
                    canUseUpgrade = false;
                }
                break;
            case UpgradeType.blackhole:
                if(isMagnetActive || isBlackholeActive || isBurning || isHintPlaying || isTimeFrozen || hasUsedBlackholeOnce) {
                    canUseUpgrade = false;
                }
                break;
            case UpgradeType.scissors:
                if(currentPage + pagesToCut == totalPages) {
                    canUseUpgrade = false;
                }
                break;
        }
        if(numOf > 0) {
            buttonObj.SetActive(true);
            if(canUseUpgrade) {
                buttonObj.GetComponent<Button>().interactable = true;
            } else {
                buttonObj.GetComponent<Button>().interactable = false;
            }
            countText.text = "x" + numOf.ToString();
        } else {
            buttonObj.SetActive(false);
        }
    }

    /* Gameover */
    IEnumerator GameIsOver() {
        showGameOverOnce = true;

        if(numbersFound!=totalNumbers) {
            float aVal = 1;
            while(numTextList[0].transform.localPosition.x != 0 || numTextList[0].transform.localPosition.y != 0 || aVal>0) {
                numTextList[0].transform.localPosition = Vector2.MoveTowards(numTextList[0].transform.localPosition, Vector2.zero, 5f);
                if(numTextList[0].fontSize < 300) {
                    numTextList[0].fontSize+=2;
                    Canvas.ForceUpdateCanvases();
                }
                if(numTextList.Count>1) {
                    if(aVal>0) {
                        aVal -= 1f / 65f;
                    }
                    foreach(Text t in numTextList) {
                        if(t != numTextList[0]) {
                            t.color = new Color(t.color.r, t.color.g, t.color.b, aVal);
                        }
                    }
                }
                yield return new WaitForSeconds(.01f);
            }
            yield return new WaitForSeconds(.75f);
        }

        lvlCompletePanel.SetActive(true);

        isInLevel = false;

        bool alreadyGot3Stars = SaveSystem.data.starAmount[(int)leveltype][levelNum - 1] == 3;

        if(numbersFound != totalNumbers) {
            starCount = 0;
        }

        if(starCount > SaveSystem.data.starAmount[(int)leveltype][levelNum - 1]) {
            SaveSystem.data.starAmount[(int)leveltype][levelNum - 1] = starCount;
        }

        if(!alreadyGot3Stars) {
            coinsEarned = Random.Range(starCount * levelNum / 2, starCount * levelNum);
        } else {
            coinsEarned = Random.Range(starCount * levelNum / 4, starCount * levelNum / 2);
        }
        if(coinsEarned < 1 && starCount > 0) {
            coinsEarned = 1;
        }

        coinsEarned *= (int)leveltype + 1;

        if(starCount > 0) {
            StartCoroutine("ShowLevelComplete");
            ShowAchievements.canShowAchievements = true;
        }

        SaveSystem.data.coins += coinsEarned;
    }

    IEnumerator ShowLevelComplete() {
        lvlCompletePanel.GetComponent<Animation>().Play("ShowLevelCompletePanel");
        yield return new WaitForSeconds(lvlCompletePanel.GetComponent<Animation>().GetClip("ShowLevelCompletePanel").length);
        StartCoroutine("PlayStarAnimations");
    }

    /* Level Complete Star Animation */
    IEnumerator PlayStarAnimations() {
        yield return new WaitForSeconds(.75f);

        if(starCount > 0) {
            fallInStars[0].SetActive(true);
            fallInStars[0].GetComponent<Animation>().Play();
            yield return new WaitForSeconds(fallInStars[0].GetComponent<Animation>().clip.length);
            fallInStars[0].SetActive(false);
            stars[0].sprite = fullStar;
        } else {
            stars[0].sprite = emptyStar;
            coinsEarned = 0;
        }

        if(starCount > 1) {
            fallInStars[1].SetActive(true);
            fallInStars[1].GetComponent<Animation>().Play();
            yield return new WaitForSeconds(fallInStars[1].GetComponent<Animation>().clip.length);
            fallInStars[1].SetActive(false);
            stars[1].sprite = fullStar;
        } else {
            stars[1].sprite = emptyStar;
        }

        if(starCount > 2) {
            fallInStars[2].SetActive(true);
            fallInStars[2].GetComponent<Animation>().Play();
            yield return new WaitForSeconds(fallInStars[2].GetComponent<Animation>().clip.length);
            fallInStars[2].SetActive(false);
            stars[2].sprite = fullStar;
        } else {
            stars[2].sprite = emptyStar;
        }

        starsObj.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(starsObj.GetComponent<Animation>().clip.length);

        coinsEarnedObj.SetActive(true);
        coinsEarnedObj.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(coinsEarnedObj.GetComponent<Animation>().clip.length);

        float tempCoins = 0;
        float coinsDiv = coinsEarned / 50.0f;
        while(tempCoins < coinsEarned) {
            numberOfCoinsText.text = Mathf.CeilToInt(tempCoins).ToString();
            tempCoins += coinsDiv;
            yield return new WaitForSeconds(.01f);
        }
        numberOfCoinsText.text = coinsEarned.ToString();

        if(GameManager.rewardedAdsEnabled) {
            doubleCoinsButton.gameObject.SetActive(true);
            doubleCoinsButton.gameObject.GetComponent<Animation>().Play("FadeIn2xCoins");
        }
    }

    void ShowNumberHint() {
        if(isHintPlaying && !isPaused) {
            numTextList[0].gameObject.GetComponent<Animation>().Play("ShowHint");
        }
    }

    void CheckCombo() {
        secondsSinceLastNum = Time.time - secondsStartLastNum;
        if(secondsSinceLastNum <= secondsForCombo) {
            numClickCombo++;
            float tempRandX = Random.Range(52, 122);
            float tempRandY = Random.Range(10, 32);
            comboIndicator.GetComponent<RectTransform>().localPosition = new Vector3(tempRandX, tempRandY, 0);
            float tempRandRotZ = Random.Range(-20, 20);
            comboIndicator.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, tempRandRotZ);
            if(comboIndicator.GetComponent<Animation>().isPlaying) {
                comboIndicator.GetComponent<Animation>().Stop();
            }
            if(comboIndicatorCount.GetComponent<Animation>().isPlaying) {
                comboIndicatorCount.GetComponent<Animation>().Stop();
            }
            comboIndicator.GetComponent<Animation>().Play("ComboFadeIn");
            comboIndicator.GetComponent<Animation>().PlayQueued("ComboFadeOut");
            comboIndicatorCount.GetComponent<Animation>().Play("ComboFadeIn");
            comboIndicatorCount.GetComponent<Animation>().PlayQueued("ComboFadeOut");
            comboIndicator.text = comboText[Random.Range(0, comboText.Length)];
            comboIndicatorCount.text = "x" + numClickCombo.ToString();
            startTime += numClickCombo * .2f + .2f;
        } else {
            numClickCombo = 0;
        }
        secondsStartLastNum = Time.time;
    }

    #region Weathering Effects
    IEnumerator DoTornado() {
        isToronadoCoroutine = true;
        if(!hasTornadoMoved) {
            tornadoObj.SetActive(true);
            tornadoObj.GetComponent<Animation>().Play("TornadoMove");
            hasTornadoMoved = true;
        }
        List<GameObject> objectsToMove = new List<GameObject>();
        foreach(Text t in numTextList) {
            if(t.GetComponent<BoxCollider2D>().bounds.Intersects(tornadoObj.GetComponent<BoxCollider2D>().bounds)) {
                objectsToMove.Add(t.gameObject);
            }
        }
        for(int i=0; i<objectsToMove.Count/2; i++) {
            Vector2 tempTransform = objectsToMove[i].transform.localPosition;
            int fontS1 = objectsToMove[i].GetComponent<Text>().fontSize;
            int randomTrans = Random.Range(0, objectsToMove.Count);
            Vector2 tempTransform2 = objectsToMove[randomTrans].transform.localPosition;
            int fontS2 = objectsToMove[randomTrans].GetComponent<Text>().fontSize;
            objectsToMove[randomTrans].transform.localPosition = tempTransform;
            objectsToMove[i].transform.localPosition = tempTransform2;
            objectsToMove[i].GetComponent<Text>().fontSize = fontS2;
            objectsToMove.RemoveAt(i);
            if(i!=randomTrans) {
                if(randomTrans>i) {
                    objectsToMove[randomTrans-1].GetComponent<Text>().fontSize = fontS1;
                    objectsToMove.RemoveAt(randomTrans-1);
                } else {
                    objectsToMove[randomTrans].GetComponent<Text>().fontSize = fontS1;
                    objectsToMove.RemoveAt(randomTrans);
                }
            }
        }
        yield return new WaitForSeconds(.05f);
        if(!tornadoObj.GetComponent<Animation>().isPlaying) {
            isTornadoActive = false;
            hasTornadoMoved = false;
            tornadoObj.SetActive(false);
        }
        isToronadoCoroutine = false;
    }

    #endregion

    /* PUBLIC METHODS */
    #region Pause
    public void PauseGame() {
        pausePanel.SetActive(true);
        pausePanel.GetComponent<Animation>().Play("ShowPausePanel");
        isPaused = true;
    }

    public void unPauseGame() {
        StartCoroutine("PlayUnPauseAnimation");
    }
    IEnumerator PlayUnPauseAnimation() {
        pausePanel.GetComponent<Animation>().Play("HidePausePanel");
        yield return new WaitForSeconds(pausePanel.GetComponent<Animation>().GetClip("HidePausePanel").length);
        pausePanel.SetActive(false);
        isPaused = false;
    }

    public void ChangePauseShowing(int dir) {
        pauseShow += dir;
        int curPauseShow = (int)pauseShow;
        pauseButtonObjs[curPauseShow].SetActive(true);
        for(int i = 0; i < pauseButtonObjs.Length; i++) {
            if(curPauseShow != i) {
                pauseButtonObjs[i].SetActive(false);
            }
        }

        if(pauseShow == 0) {
            leftPauseButton.gameObject.SetActive(false);
            rightPauseButton.gameObject.SetActive(true);
        } else if(curPauseShow == pauseButtonObjs.Length - 1) {
            rightPauseButton.gameObject.SetActive(false);
            leftPauseButton.gameObject.SetActive(true);
        }
    }
    #endregion

    #region Upgrades
    public void SetShowHint() {
        if(SaveSystem.data.numHints > 0 && numTextList[0].gameObject.GetComponent<Animation>() == null) {
            numTextList[0].gameObject.AddComponent<Animation>();
            numTextList[0].gameObject.GetComponent<Animation>().AddClip(showHintAnim, "ShowHint");
            SaveSystem.data.numHints--;
            isHintPlaying = true;
            hintCountText.text = "x" + SaveSystem.data.numHints.ToString();
        }
    }

    public void SetFreezeTime() {
        if(SaveSystem.data.numFreezes > 0) {
            isTimeFrozen = true;
            totalAllowedFreezeTime += 10;
            isFreezeIning = true;
            SaveSystem.data.numFreezes--;
            freezeCountText.text = "x" + SaveSystem.data.numFreezes.ToString();
        }
    }


    /* FIRE UPGRADE */
    public void SetStartFire() {
        if(SaveSystem.data.numFires > 0) {
            isBurning = true;
            totalNumbersToBurn += 5;
            SaveSystem.data.numFires--;
            fireCountText.text = "x" + SaveSystem.data.numFires.ToString();
            fireObj.SetActive(true);
        }
    }
    void setFires() {
        if(!fireRunning) {
            fireRunning = true;
            totalNumbersToBurn--;
            startFireTime = Time.time;
            fireTextToBurn = numTextList[0].gameObject;

            if(isBurning) {
                Vector2 textPos = fireTextToBurn.transform.localPosition;
                float fireScale = fireTextToBurn.GetComponent<RectTransform>().rect.width / 100f;
                fireObj.transform.localPosition = new Vector2(textPos.x, textPos.y);
                fireObj.transform.localScale = new Vector3(fireScale, fireScale, 0);
            }
        }

        if(isBurning) {
            if(!fireCorotineRunning) {
                StartCoroutine("SwitchFireSprite");
            }

            if(Time.time - startFireTime >= .9f) {
                numTextList.Remove(fireTextToBurn.GetComponent<Text>());
                Destroy(fireTextToBurn);
                numbersFound++;
                CheckCombo();
                fireRunning = false;
                startFireTime = Time.time;
            }
            if(totalNumbersToBurn == -1 || isGameOver) {
                fireObj.SetActive(false);
                isBurning = false;
            }
        }
    }
    IEnumerator SwitchFireSprite() {
        fireCorotineRunning = true;
        yield return new WaitForSeconds(.0166667f);
        foreach(Transform child in fireObj.transform) {
            child.GetComponent<Image>().sprite = fireSprites[curFireSprite];
        }

        curFireSprite++;
        if(curFireSprite == 8) {
            curFireSprite = 0;
        }
        fireCorotineRunning = false;
    }

    /* MAGNET UPGRADE */
    public void SetShowMagnet() {
        if(SaveSystem.data.numMagnets > 0) {
            isMagnetActive = true;
            SaveSystem.data.numMagnets--;
            magnetCountText.text = "x" + SaveSystem.data.numMagnets.ToString();
            magnetObj.SetActive(true);
        }
    }
    IEnumerator MoveMagnet() {
        isMagnetCorotineOn = true;
        Animation magnetAnim = magnetObj.GetComponent<Animation>();
        if(!hasMagnetMovedForw) {
            magnetTexts = new List<Text> { numTextList[0] };
            if(numTextList.Count > 1) {
                magnetTexts.Add(numTextList[1]);
            }
            if(numTextList.Count > 2) {
                magnetTexts.Add(numTextList[2]);
            }
            magnetAnim.Play("MagnetSlideIn");
            yield return new WaitForSeconds(magnetAnim.GetClip("MagnetSlideIn").length);
            hasMagnetMovedForw = true;
        }

        int numCloseEnough = 0;
        if(!canMagnetMoveBack && hasMagnetMovedForw) {
            if(!magnetAnim.isPlaying) {
                magnetAnim.Play("MagnetShake");
            }
            for(int i = 0; i < magnetTexts.Count; i++) {
                if(magnetTexts[i] == null) {
                    magnetTexts.RemoveAt(i);
                }
            }
            foreach(Text t in magnetTexts) {
                if(t.GetComponent<BoxCollider2D>().bounds.Intersects(magnetObj.GetComponent<BoxCollider2D>().bounds)) {
                    numCloseEnough++;
                } else {
                    float speed = .8f * Vector2.Distance(t.transform.localPosition, magnetObj.transform.localPosition) + 100;
                    t.transform.localPosition = Vector2.MoveTowards(t.transform.localPosition, magnetObj.transform.localPosition,
                    Time.deltaTime * speed);
                }
            }
            if(numCloseEnough == magnetTexts.Count) {
                canMagnetMoveBack = true;
            }
        }

        if(canMagnetMoveBack) {
            magnetObj.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
            magnetAnim.Play("MagnetSlideOut");
            yield return new WaitForSeconds(magnetAnim.GetClip("MagnetSlideOut").length);
            isMagnetActive = false;
            hasMagnetMovedForw = false;
            canMagnetMoveBack = false;
            magnetObj.SetActive(false);
        }
        isMagnetCorotineOn = false;
    }

    /* BLACKHOLE UPGRADE */
    public void SetPlaceBlackhole() {
        if(SaveSystem.data.numBlackholes > 0) {
            isBlackholeActive = true;
            SaveSystem.data.numBlackholes--;
            blackholeCountText.text = "x" + SaveSystem.data.numBlackholes.ToString();
            blackholeObj.SetActive(true);
            hasUsedBlackholeOnce = true;
        }
    }
    void PlayBlackhole() {
        if(!hasBlackholeAnimStarted) {
            blackholeObj.GetComponent<Animation>().Play("CreateBlackhole");
            hasBlackholeAnimStarted = true;
            startBlackholeTime = Time.time;
        }

        totalBlackholeTime = Time.time - startBlackholeTime;
        foreach(Text t in numTextList) {
            suckSpeed = blackholeObj.GetComponent<CircleCollider2D>().radius * (35f / Vector2.Distance(t.transform.localPosition,
                blackholeObj.transform.localPosition));
            t.transform.localPosition = Vector2.MoveTowards(t.transform.localPosition, blackholeObj.transform.localPosition,
                Time.deltaTime * suckSpeed);
            if(blackholeObj.GetComponent<CircleCollider2D>().bounds.Contains(t.transform.position)) {
                numTextList.Remove(t);
                Destroy(t);
                numbersFound++;
                CheckCombo();
                if(numTextList.Count == 0) {
                    isBlackholeActive = false;
                    blackholeObj.SetActive(false);
                }
                break;
            }
        }
    }

    /* SCISSORS UPGRADE */
    public void SetShowScissors() {
        if(SaveSystem.data.numScissors > 0) {
            isScissorsActive = true;
            SaveSystem.data.numScissors--;
            pagesToCut++;
            scissorsCountText.text = "x" + SaveSystem.data.numScissors.ToString();
            scissorsObj.SetActive(true);
        }
    }
    IEnumerator CutWithScissors() {
        if(!hasScissorsFlownIn) {
            startScissorsTime = Time.time;
            scissorsObj.GetComponent<Animation>().Play("ScissorsFlyIn");
            yield return new WaitForSeconds(scissorsObj.GetComponent<Animation>().GetClip("ScissorsFlyIn").length);
            hasScissorsFlownIn = true;
        }
        totalScissorsTime = Time.time - startScissorsTime;

        if(hasScissorsFlownIn) {
            if(currentPage == totalPages) {
                pagesToCut = 0;
            }
            if(pagesToCut > 0 && !isScissorsCutting) {
                isScissorsCutting = true;
                scissorsObj.GetComponent<Animation>().Play("ScissorsCutPage");
                yield return new WaitForSeconds(scissorsObj.GetComponent<Animation>().GetClip("ScissorsCutPage").length / 2.0f);
                pagesToCut--;
                totalPages--;
                totalNumbers -= numbersOnPage[numbersOnPage.Count - 1];
                numbersOnPage.RemoveAt(numbersOnPage.Count - 1);
                pageText.text = currentPage.ToString() + "/" + totalPages.ToString();
                yield return new WaitForSeconds(scissorsObj.GetComponent<Animation>().GetClip("ScissorsCutPage").length / 2.0f);
                isScissorsCutting = false;
            }
            if(pagesToCut == 0 && !scissorsObj.GetComponent<Animation>().isPlaying) {
                isScissorsActive = false;
                hasScissorsFlownIn = false;
                scissorsObj.GetComponent<Animation>().Play("ScissorsFlyOut");
                yield return new WaitForSeconds(scissorsObj.GetComponent<Animation>().GetClip("ScissorsFlyOut").length);
                scissorsObj.SetActive(false);
            }
        }
    }
    #endregion

    #region Double Coins
    /* DOUBLE COINS FROM AD */
    public void WatchAdDoubleCoins() {
        AdManager.ShowRewardAd(AdDoubleCoinsSuccess, AdDoubleCoinsSkip, AdDoubleCoinsFail);
    }
    void AdDoubleCoinsSuccess() {
        doubleCoinsButton.interactable = false;
        SaveSystem.data.coins += coinsEarned;
        StartCoroutine("DoubleCoins");
    }
    IEnumerator DoubleCoins() {
        float tempCoins = coinsEarned;
        float coinsDiv = coinsEarned / 50.0f;
        while(tempCoins < coinsEarned * 2) {
            numberOfCoinsText.text = Mathf.CeilToInt(tempCoins).ToString();
            tempCoins += coinsDiv;
            yield return new WaitForSeconds(.01f);
        }
        numberOfCoinsText.text = (coinsEarned * 2).ToString();
    }
    void AdDoubleCoinsSkip() {
        Debug.Log("Double Coins Ad Skipped");
    }
    void AdDoubleCoinsFail() {
        Debug.Log("Double Coins Ad Failed");
    }
    #endregion
}
