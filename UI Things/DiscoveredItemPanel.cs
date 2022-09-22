using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoveredItemPanel : MonoBehaviour
{
    public RosettaStone rs;
    public RestaurantInfo ri;
    public UIManager uim;

    public DiscoveredItemSlot itemSlot;
    public GameObject discoveredItemHolder;

    public bool populated;
    public void PopulateDiscoveredItems(){
        for(var i = 0; i < rs.foodVault.Count; i++){
            var newSlot = Instantiate(itemSlot);
            if(!ri.discoveredItems.Contains(rs.foodVault[i])){
                newSlot.item = rs.foodVault[i];
                newSlot.itemImage.sprite = rs.foodVault[i].itemImage;    
            } else {
                newSlot.itemImage.gameObject.SetActive(false);
            }
            newSlot.transform.SetParent(discoveredItemHolder.transform, worldPositionStays: false);
        }
    }
}
