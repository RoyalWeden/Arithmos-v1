using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGamePurchases : MonoBehaviour {

    public enum PurchaseType { removeAds=0, coin250=1, coin500=2, coin1000=3, coin2500=4 };

    public Button removeAdsButton;

    [Space(5)]
    public GameObject upgradeButtonObj;
    public Sprite upgradeSprite;
    public Sprite lockedSprite;

    private void Start() {
        if(SaveSystem.data.unlockedAchievements[0]) {
            upgradeButtonObj.GetComponent<Button>().enabled = true;
            upgradeButtonObj.GetComponent<Image>().sprite = upgradeSprite;
        } else {
            upgradeButtonObj.GetComponent<Button>().enabled = false;
            upgradeButtonObj.GetComponent<Image>().sprite = lockedSprite;
        }
    }

    private void Update() {
        if(SaveSystem.data.removedAds) {
            removeAdsButton.interactable = false;
        } else {
            removeAdsButton.interactable = true;
        }
    }

    public void PurchaseItem(int purchaseType) {
        switch(purchaseType) {
            case (int)PurchaseType.removeAds:
                IAPManager.instance.BuyRemoveAds();
                break;
            case (int)PurchaseType.coin250:
                IAPManager.instance.BuyCoin250();
                break;
            case (int)PurchaseType.coin500:
                IAPManager.instance.BuyCoin500();
                break;
            case (int)PurchaseType.coin1000:
                IAPManager.instance.BuyCoin1000();
                break;
            case (int)PurchaseType.coin2500:
                IAPManager.instance.BuyCoin2500();
                break;
        }
    }
}
