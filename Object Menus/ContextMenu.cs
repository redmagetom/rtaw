using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ContextMenu : MonoBehaviour
{
    public UIManager uiManager;
    public GameObject invItemPrefab;
    public Interactable obj;
    public GameObject titlePanel;
    public TextMeshProUGUI titleText;

    public GameObject contextItemsHolder;
    public GameObject contextButton;
    public GameObject contextButtonText;


    public void LaunchMenu(){
        SetUpMenu();
    }

    private void SetUpMenu(){
        foreach(Transform child in contextItemsHolder.transform){
            Destroy(child.gameObject);
        }
        obj = obj.GetComponent<Interactable>();
        titleText.text = obj.friendlyName;
        for(int i = 0; i < obj.actionList.Count; i++){
            GameObject newButton = Instantiate(contextButton, contextItemsHolder.transform.position, Quaternion.identity);
            newButton.transform.SetParent(contextItemsHolder.transform, worldPositionStays: false);
            var actionToAdd = obj.actionList[i];
            actionToAdd.triggeringObject = obj.gameObject;
            newButton.GetComponent<Button>().onClick.AddListener(delegate{actionToAdd.ExecuteActionOrder(gameObject);});
            GameObject buttonText = Instantiate(contextButtonText, contextButton.transform.position, Quaternion.identity);
            buttonText.transform.SetParent(newButton.transform, worldPositionStays: false);
            buttonText.GetComponent<Text>().text = actionToAdd.actionName;
            actionToAdd.SetUpVars();
        }
    }
}
