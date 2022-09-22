using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ES3Serializable]
[CreateAssetMenu(fileName = "New Base Item", menuName ="Items/Base Item")]
public class Item : ScriptableObject
{
    public enum ItemType{CuttingTool, Food}
    public ItemType itemType;
    public int uid;
    public string itemName;
    public string itemDescription;
    public Sprite itemImage;
    public GameObject itemModel;
    public int value;


}
