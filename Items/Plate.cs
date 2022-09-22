using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{

    public List<GameObject> platingAreas;
    public GameObject orderSlip;

    public List<Food>platedItems;
    public int selectedPlatePosition;

    public void ReadyPlating(){
   
        for(var i = 0; i < platingAreas.Count; i++){
            if(platedItems[i] == null){
                platingAreas[i].SetActive(true);
            }
        }
    }

    public void HidePlatingOptions(){
      
        foreach(GameObject area in platingAreas){
            area.SetActive(false);
        } 
    }

    public void AddItemToPlate(){

    }
}
