using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public RosettaStone rs;
    int UILayer = 5;
    public Camera mainCam;
    public UIManager uim;
    private NavMeshAgent navAgent;

    private PlayerStats playerStats;

    
    void Start()
    {
        navAgent = gameObject.GetComponent<NavMeshAgent>();   
        playerStats = gameObject.GetComponent<PlayerStats>();
    }

   
    void Update(){


        // // Right click
        if(Input.GetMouseButtonDown(1)){
            // dont move if panel open
            foreach(var panel in uim.allPanels){
                if(panel.activeSelf){return;}
            }

            RaycastHit hit;
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)){
                // move if hit floor
                if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Floors")){
                    navAgent.destination = hit.point;
                }
            }
        }
    }



}
