using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Grocer : MonoBehaviour
{
    public GameObject grocerItemPrefab;
    public GameObject grocerCheckoutPrefab;
    public RosettaStone rs;
    public RestaurantInfo ri;
    public string grocerName;
    public int availableMoney;
    public int merchandiseQuantity;
    public List<Item> merchandise;
   
    public List<Item> checkoutItems;
  

    [Header("UI Things")]
    public Text invCount;
    public Text checkoutTotal;
    public Image grocerImage;
    public GameObject groceryItemHolder;
    public GameObject checkoutItemHolder;

    public void SelectRandomMerch(){
        for(var i = 0; i < merchandiseQuantity; i++){
            int rollValue = Random.Range(0, rs.foodVault.Count);
            var item = (rs.foodVault[rollValue]);
            merchandise.Add(item);
            AddMerchItem(item);
        }
    }

    public void AddMerchItem(Item item){
        var uiItem = Instantiate(grocerItemPrefab);
        uiItem.transform.SetParent(groceryItemHolder.transform, worldPositionStays: false);
        var uiFields = uiItem.GetComponent<GrocerInventoryItem>();
        uiFields.item = item;
        uiFields.itemImage.sprite = item.itemImage;
        uiFields.itemName.text = item.itemName;
        uiFields.itemDescription.text = item.itemDescription;
        uiFields.itemCost.text = item.value.ToString();
    }


    public void FinalizeCheckout(){
        int itemsRemaining = checkoutItems.Count;
        for(var i = 0; i < ri.maxInventory; i++){
            if(itemsRemaining == 0){break;}
            if(!ri.inventory[i]){
                ri.inventory[i] = checkoutItems[0];
                checkoutItems.RemoveAt(0);
                itemsRemaining -= 1;
            }
        //     for(var i = 0; i < ri.maxInventory; i++){
        //         if(!ri.inventory[i]){
        //             ri.inventory[i] = item;
        //         }
        //     }
        }

        foreach(Transform child in checkoutItemHolder.transform){
            Destroy(child.gameObject);
        }

        ri.currentMoney -= int.Parse(checkoutTotal.text);
        // rs.uiManager.totalMoneyDisplay.text = ri.currentMoney.ToString();

        checkoutItems.Clear();
        checkoutTotal.text = "0";

        rs.uiManager.UpdateAllUI();
    }

}
