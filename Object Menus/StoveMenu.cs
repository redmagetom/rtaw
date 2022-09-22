using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoveMenu : MonoBehaviour
{
    public RosettaStone rs;
    public RestaurantInfo ri;
    public UIManager uim;

    public int selectedIdx;


    public GameObject cookSelection;
    public GameObject cookGamePanel;
    public Dropdown cookType;

    public List<string> presentTense;
    public List<string> pastTense;
    public UIItemSlot selectedItem;

    void Start(){
        PopulateLists();
    }




    private void PopulateLists(){
        presentTense.Add("Fry");
        presentTense.Add("Sear");
        presentTense.Add("Sautee");


        pastTense.Add("Fried");
        pastTense.Add("Seared");
        pastTense.Add("Sauteed");

        cookType.AddOptions(presentTense);
    }


    public void StartMinigame(){
        cookSelection.SetActive(false);
        cookGamePanel.SetActive(true);
    }

    public void GetRandomQuality(){
        ProcessedFood newItem = new ProcessedFood();
        newItem.processQuality = (ProcessedFood.ProcessQuality)Random.Range((int)ProcessedFood.ProcessQuality.Akward, (int)ProcessedFood.ProcessQuality.Professional +1);
        newItem.itemName = newItem.processQuality.ToString() + " " + pastTense[cookType.value] + " " + selectedItem.item.itemName;
        newItem.name = newItem.itemName;
        newItem.value = ((int)newItem.processQuality * selectedItem.item.value);
        newItem.itemDescription = "Some " + selectedItem.item.itemName + "'s that have been " + pastTense[cookType.value];
        newItem.itemType = Item.ItemType.Food;
        newItem.freshTime = 300;

        Food food = selectedItem.item as Food;
        ProcessedFood proced = selectedItem.item as ProcessedFood;

        if(food.unprocessed){
            newItem.itemDescription = newItem.itemDescription.Replace($"{selectedItem.item.itemName}", $"whole {selectedItem.item.itemName}"); 
            var baseItem = rs.fullItemVault.Find(i => i.itemName == $"{food.baseitem.itemName}_StoveCooked");
            newItem.uid = baseItem.uid;
            newItem.itemImage = baseItem.itemImage;
            newItem.itemModel = baseItem.itemModel;
        } else {

            var baseItem = rs.fullItemVault.Find(i => i.itemName == $"{food.baseitem.itemName}_{food.itemName.Split(' ')[1]}_StoveCooked");

            newItem.uid = baseItem.uid;
            newItem.itemImage = baseItem.itemImage;
            newItem.itemModel = baseItem.itemModel;
        }

        if(proced){
            newItem.coreItem = proced.coreItem;
            newItem.processList = proced.processList;
        } else {
            newItem.coreItem = food;
            newItem.processList = new List<string>();
        }

        newItem.fullyProcessed = true;
        newItem.unprocessed = false;
        
        newItem.processList.Add(presentTense[cookType.value]);
        ri.inventory[selectedIdx] = newItem;

        cookGamePanel.SetActive(false);
        selectedItem.item = null;
        selectedItem.itemImage.sprite = null;

        uim.StoveMenuToggle();
        rs.uiManager.UpdateAllUI();

    }

}
