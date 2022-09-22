using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bayat.SaveSystem;


public class DataManager : MonoBehaviour
{
    public ES3ReferenceMgr refmgr;
    public RestaurantInfo ri;
    public UIManager uim;




    public async void Save(){
        // --- untested....
   
        // ES3.Save("ri", ri);
        // ES3.Save("restaurant", ri.gameObject.GetComponent<RosettaStone>().restaurant);
        await SaveSystemAPI.SaveAsync("ri.json", ri);
        // await SaveSystemAPI.SaveAsync("restaurant", ri.gameObject.GetComponent<RosettaStone>().restaurant);

   
    }

    private async void LoadSaved(){
        ri = await SaveSystemAPI.LoadAsync<RestaurantInfo>("ri.json");
 
        // ri.gameObject.GetComponent<RosettaStone>().restaurant = await SaveSystemAPI.LoadAsync<GameObject>("restaurant");
    }
    public void Load(){
        LoadSaved();
        // StartCoroutine(I_Load());

    }

    private IEnumerator I_Load(){
        
        while(!ri.gameObject.GetComponent<RosettaStone>().fullyLoaded){
            Debug.Log("Waiting for load");
            yield return new WaitForSeconds(0.25f);
        }
        // foreach(var thing in ri.allFloors){
        //     Destroy(thing.gameObject);
        // }
        yield return new WaitForSeconds(0.02f);
        ES3.LoadInto("ri", ri);
        // ES3.Load("restaurant", ri.gameObject.GetComponent<RosettaStone>().restaurant);
       
        

        foreach(var recipe in ri.recipes){
            recipe.name = recipe.recipeName;
        }

  
        // // update dish names
        foreach(var dish in ri.currentMenuItems){
            if(dish != null){
                dish.name = dish.dishName;
            }     
        }

        foreach(var item in ri.inventory){
            // Debug.Log(item);
            if(!item){
                continue;
            }
            item.name = item.itemName;
            item.itemImage = gameObject.GetComponent<RosettaStone>().fullItemVault.Find(it => it.uid == item.uid).itemImage;
        }

    }


}




