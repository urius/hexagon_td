using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener
{
    public static IAPManager Instance { get; private set; }

    public event Action<Product> ProcessPurchaseTriggered = delegate { };
    public event Action<Product, PurchaseFailureReason> PurchaseFailed = delegate { };

    public const string Gold100 = "gold_100";
    public const string Gold500 = "gold_500";

    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider;

    private bool IsInitialized => m_StoreController != null && m_StoreExtensionProvider != null;

    public void Start()
    {
        Instance = this;
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        if (IsInitialized) return;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(Gold100, ProductType.Consumable);
        builder.AddProduct(Gold500, ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public async Task<bool> BuyProductAsync(string productId)
    {
        var purchaseTsc = new TaskCompletionSource<bool>();
        void OnPurchaseFailed(Product p, PurchaseFailureReason r)
        {
            if (p.definition.id == productId)
            {
                purchaseTsc.TrySetResult(false);
            }
        }
        void OnPurchaseSuccess(Product p)
        {
            if (p.definition.id == productId)
            {
                purchaseTsc.TrySetResult(true);
            }
        }
        PurchaseFailed += OnPurchaseFailed;
        ProcessPurchaseTriggered += OnPurchaseSuccess;
        BuyProductID(productId);

        var result = await purchaseTsc.Task;

        PurchaseFailed -= OnPurchaseFailed;
        ProcessPurchaseTriggered -= OnPurchaseSuccess;

        return result;
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized");
        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public void OnPurchaseFailed(Product p, PurchaseFailureReason r)
    {
        PurchaseFailed(p, r);
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", p.definition.storeSpecificId, r));
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        ProcessPurchaseTriggered(args.purchasedProduct);
        return PurchaseProcessingResult.Complete;
    }

    private void BuyProductID(string productId)
    {
        if (IsInitialized)
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }
}
