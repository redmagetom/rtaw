using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RosettaStone : MonoBehaviour
{
   public bool fullyLoaded;
   public GameObject mainCamHolder;
   public Camera mainCam;
   public GameObject player;
   public UIManager uiManager;
   public RestaurantInfo ri;
   public GameplayManager gpm;
   public List<Item> fullItemVault;
   public List<Item> foodVault;
   public List<WallType> wallTypeVault;


   public GameObject restaurant;
   void Start(){
      LoadVaults();
   }


   private void LoadVaults(){
      LoadItemVault();
   }

   private void LoadItemVault(){
      print("Loading Item Vault");
      var itemFolder = Resources.LoadAll("Items");
      foreach(var thing in itemFolder){
         try {
            Item i = thing as Item;
            if(i.itemType == Item.ItemType.Food){
               if((i as Food).unprocessed){
                  foodVault.Add(i);
                  i.itemName = i.name;
               }
            }
            fullItemVault.Add(i);
         } catch (System.Exception e){
            Debug.Log($"Error with: {thing.name}");
         }

      }


      print("Loading Wall Types");
      var wallTypeFolder = Resources.LoadAll("Build Mode Objects/Wall Types");
      foreach(var thing in wallTypeFolder){
         WallType wt = thing as WallType;
         wt.name = wt.styleName;
         wallTypeVault.Add(wt);
      }

      fullyLoaded = true;
   }
}
