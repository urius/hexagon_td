﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class IAPManager : MonoBehaviour, IStoreListener
{
    public static IAPManager Instance { get; private set; }

    public event Action<Product> ProcessPurchaseTriggered = delegate { };
    public event Action<Product, PurchaseFailureReason> PurchaseFailed = delegate { };

    private readonly TaskCompletionSource<bool> _isInitializedTsc = new TaskCompletionSource<bool>();
    public Task<bool> InitializedTask => _isInitializedTsc.Task;

    public readonly List<string> Products = new List<string>();

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

    public Product GetProductData(string productId)
    {
        return m_StoreController.products.WithID(productId);
    }

    public async void InitializePurchasing()
    {
        if (IsInitialized) return;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        var storeProductsResponse = await NetworkManager.GetStoreProductsAsync();
        if (storeProductsResponse.IsSuccess && !storeProductsResponse.Result.IsError)
        {
            var products = storeProductsResponse.Result.payload.products;
            Products.AddRange(products);
        }
        else
        {
            Products.AddRange(new[] { "gold_1000", "gold_5000", "gold_60000" });
        }

        var length = Math.Min(Products.Count, 3);
        for (var i = 0; i < length; i++)
        {
            builder.AddProduct(Products[i], ProductType.Consumable);
        }

        UnityPurchasing.Initialize(this, builder);
    }

    public async Task<PurchaseResult> BuyProductAsync(string productId)
    {
        var purchaseTsc = new TaskCompletionSource<PurchaseResult>();
        void OnPurchaseFailed(Product p, PurchaseFailureReason r)
        {
            if (p.definition.id == productId)
            {
                var purchaseResult = new PurchaseResult()
                {
                    IsSuccess = false,
                    FailureReason = r,
                    ProductData = p,
                };
                purchaseTsc.TrySetResult(purchaseResult);
            }
        }
        void OnPurchaseSuccess(Product p)
        {
            if (p.definition.id == productId)
            {
                var purchaseResult = new PurchaseResult()
                {
                    IsSuccess = true,
                    ProductData = p,
                    FailureReason = PurchaseFailureReason.Unknown,
                };
                purchaseTsc.TrySetResult(purchaseResult);
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

        _isInitializedTsc.TrySetResult(true);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);

        _isInitializedTsc.TrySetResult(false);
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

public struct PurchaseResult
{
    public bool IsSuccess;
    public Product ProductData;
    public PurchaseFailureReason FailureReason;

    public string Receipt => ProductData != null ? ProductData.receipt : "null ProductData receipt";
}
