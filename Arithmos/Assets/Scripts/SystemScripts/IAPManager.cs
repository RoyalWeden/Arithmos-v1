using System;
using UnityEngine;
using UnityEngine.Purchasing;


public class IAPManager : MonoBehaviour, IStoreListener
{
    public static IAPManager instance;

    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;

    //Step 1 create your products

    string coin250 = "coin_250";
    string coin500 = "coin_500";
    string coin1000 = "coin_1000";
    string coin2500 = "coin_2500";
    string removeAds = "remove_ads";

    //************************** Adjust these methods **************************************
    public void InitializePurchasing()
    {
        if (IsInitialized()) { return; }
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        //Step 2 choose if your product is a consumable or non consumable
        builder.AddProduct(coin250, ProductType.Consumable);
        builder.AddProduct(coin500, ProductType.Consumable);
        builder.AddProduct(coin1000, ProductType.Consumable);
        builder.AddProduct(coin2500, ProductType.Consumable);
        builder.AddProduct(removeAds, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    //Step 3 Create methods
    public void BuyCoin250() {
        BuyProductID(coin250);
    }
    public void BuyCoin500() {
        BuyProductID(coin500);
    }
    public void BuyCoin1000() {
        BuyProductID(coin1000);
    }
    public void BuyCoin2500() {
        BuyProductID(coin2500);
    }
    public void BuyRemoveAds() {
        BuyProductID(removeAds);
    }



    //Step 4 modify purchasing
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (String.Equals(args.purchasedProduct.definition.id, removeAds, StringComparison.Ordinal)) {
            SaveSystem.data.removedAds = true;
            AdManager.HideBanner();
            GameManager.regularAdsEnabled = false;
            //Debug.Log("Removed ads successfully");
        } else if(String.Equals(args.purchasedProduct.definition.id, coin250, StringComparison.Ordinal)) {
            SaveSystem.data.coins += 250;
            //Debug.Log("Added 250 coins successfully");
        } else if(String.Equals(args.purchasedProduct.definition.id, coin500, StringComparison.Ordinal)) {
            SaveSystem.data.coins += 500;
            //Debug.Log("Added 500 coins successfully");
        } else if(String.Equals(args.purchasedProduct.definition.id, coin1000, StringComparison.Ordinal)) {
            SaveSystem.data.coins += 1000;
            //Debug.Log("Added 1000 coins successfully");
        } else if(String.Equals(args.purchasedProduct.definition.id, coin2500, StringComparison.Ordinal)) {
            SaveSystem.data.coins += 2500;
            //Debug.Log("Added 2500 coins successfully");
        } else {
            Debug.Log("Purchase Failed");
        }
        return PurchaseProcessingResult.Complete;
    }










    //**************************** Dont worry about these methods ***********************************
    private void Awake()
    {
        TestSingleton();
    }

    void Start()
    {
        if (m_StoreController == null) { InitializePurchasing(); }
    }

    private void TestSingleton()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("RestorePurchases started ...");

            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((result) => {
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        else
        {
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized: PASS");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}