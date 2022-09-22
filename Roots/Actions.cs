using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions : MonoBehaviour
{   
    public RosettaStone rosetta;
    public PlayerStats playerStats;
    public BaseInformation playerBaseInfo;
    public UIManager uiManager;
    public string actionName;
    public GameObject triggeringObject;


    public void ExecuteActionOrder(GameObject menu){
        ExecuteAction();
        DismissContextMenu(menu);
    }
     public virtual void ExecuteAction(){}
    
    public void DismissContextMenu(GameObject menu){
        if(menu.activeSelf){
            menu.SetActive(false);
        }
    }

  
    public void SetUpVars(){
        rosetta = GameObject.Find("RosettaStone").GetComponent<RosettaStone>();
        playerStats = rosetta.player.GetComponent<PlayerStats>();
        playerBaseInfo = rosetta.player.GetComponent<BaseInformation>();
        uiManager = rosetta.uiManager.GetComponent<UIManager>();
    }

    public virtual void DoExtras(){}
}
