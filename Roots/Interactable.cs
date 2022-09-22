using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    // public ContextMenu contextMenu;
    public enum IType{Container, Production, Item, Surface}
    public IType type;
    public int uid;
    public string friendlyName;
    public string description;
    public List<Actions> actionList;
    public Item linkedItem;
    public GameObject interactionLocation;
}
