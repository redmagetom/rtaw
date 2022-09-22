using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public string playerName;


    // upgrade inventory size by doubling from 4 to start
    public int inventorySize;
    public List<Item> playerInventory;


    [Header("Skills")]
    public int knifeSkills;
}
