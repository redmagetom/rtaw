using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPlacement : MonoBehaviour
{
    public bool markedForDel;

    void OnTriggerStay(Collider col){
       if(markedForDel){
           this.gameObject.SetActive(false);       
           if(col.gameObject.layer == LayerMask.NameToLayer("Wall Placement Spot")){
               col.gameObject.GetComponent<WallPlacement>().markedForDel = true;
                col.gameObject.SetActive(false);
            }
           
        }
            
    }


  

}
