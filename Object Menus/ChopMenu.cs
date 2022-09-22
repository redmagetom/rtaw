using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ChopMenu : MonoBehaviour
{
    public RosettaStone rs;
    public RestaurantInfo ri;
    public UIManager uim;

    public int selectedIdx;
    public GameObject chopSelection;
    public GameObject chopGamePanel;
    public Dropdown chopType;

    public List<string> presentTense;
    public List<string> pastTense;
    public UIItemSlot selectedItem;

    void Start(){
        PopulateLists();
    }




    private void PopulateLists(){
        presentTense.Add("Chop");
        presentTense.Add("Mince");
        presentTense.Add("Slice");


        pastTense.Add("Chopped");
        pastTense.Add("Minced");
        pastTense.Add("Sliced");

        chopType.AddOptions(presentTense);
    }


    public void StartMinigame(){
        chopSelection.SetActive(false);
        chopGamePanel.SetActive(true);
    }

    public void GetRandomQuality(){
        print($"{selectedItem.item.itemName}_{pastTense[chopType.value]}");
        var baseItem = rs.fullItemVault.Find(i => i.itemName == $"{selectedItem.item.itemName}_{pastTense[chopType.value]}");
        print(baseItem);
        ProcessedFood newItem = baseItem as ProcessedFood;
        newItem.processQuality = (ProcessedFood.ProcessQuality)Random.Range((int)ProcessedFood.ProcessQuality.Akward, (int)ProcessedFood.ProcessQuality.Professional +1);
        newItem.itemName = newItem.processQuality.ToString() + " " + pastTense[chopType.value] + " " + selectedItem.item.itemName;
        newItem.name = newItem.itemName;
        newItem.value = ((int)newItem.processQuality * selectedItem.item.value);
        newItem.itemDescription = "Some " +selectedItem.item.itemName + "'s that have been " + pastTense[chopType.value];
        newItem.itemType = Item.ItemType.Food;
        newItem.freshTime = 3200;
        
        newItem.coreItem = selectedItem.item as Food;

        newItem.processList = new List<string>();
        newItem.processList.Add(presentTense[chopType.value]);

        newItem.baseitem = selectedItem.item;
        newItem.unprocessed = false;

        
        newItem.uid = baseItem.uid;
        newItem.itemImage = baseItem.itemImage;
        newItem.itemModel = baseItem.itemModel;
        ri.inventory[selectedIdx] = newItem;

        chopGamePanel.SetActive(false);
        selectedItem.item = null;
        selectedItem.itemImage.sprite = null;

        
        uim.ChopMenuToggle();
        rs.uiManager.UpdateAllUI();

    }
    
}
