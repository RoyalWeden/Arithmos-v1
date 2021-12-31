using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.PackageManager;     // comment this when building to WebGL
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsListener {

#if UNITY_IOS
    static readonly string storeID = "3783128";
#elif UNITY_ANDROID
    static readonly string storeID = "3783129";
#endif

    static readonly string videoID = "video";
    static readonly string rewardedID = "rewardedVideo";
    static readonly string bannerID = "bannerAd";

    Action adSuccess;
    Action adSkipped;
    Action adFailed;

#if UNITY_EDITOR
    static bool testMode = true;
#else
    static bool testMode = false;
#endif

    public static AdManager instance;

    private void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(instance);
            Advertisement.AddListener(this);
            Advertisement.Initialize(storeID, testMode);     // comment this when building to WebGL
        } else {
            Destroy(gameObject);
        }
    }

    public static void ShowStandardAd() {
        if(GameManager.regularAdsEnabled) {
            if(Advertisement.IsReady(videoID)) {
                Advertisement.Show(videoID);
            }
        }
    }

    public static void ShowBanner() {
        if(GameManager.regularAdsEnabled) {
            instance.StartCoroutine(ShowBannerWhenReady());
        }
    }

    public static void HideBanner() {
        if(GameManager.regularAdsEnabled) {
            Advertisement.Banner.Hide();
        }
    }

    public static void ShowRewardAd(Action success, Action skipped, Action failed) {
        if(GameManager.rewardedAdsEnabled) {
            instance.adSuccess = success;
            instance.adSkipped = skipped;
            instance.adFailed = failed;
            if(Advertisement.IsReady(rewardedID)) {
                Advertisement.Show(rewardedID);
            }
        }
    }

    static IEnumerator ShowBannerWhenReady() {
        while(!Advertisement.IsReady(bannerID)) {
            yield return new WaitForSeconds(.5f);
        }
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(bannerID);
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult) {
        if(placementId==rewardedID) {
            switch(showResult) {
                case ShowResult.Finished:
                    adSuccess();
                    break;
                case ShowResult.Skipped:
                    adSkipped();
                    break;
                case ShowResult.Failed:
                    adFailed();
                    break;
            }
        }
    }

    public void OnUnityAdsDidError(string message) { }
    public void OnUnityAdsDidStart(string placementId) { }
    public void OnUnityAdsReady(string placementId) { }
}
