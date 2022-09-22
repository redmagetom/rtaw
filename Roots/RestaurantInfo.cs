using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestaurantInfo : MonoBehaviour
{
    public string restarauntName;
    [Header("Stats")]
    public int exhaustionPoints;
    public int currentMoney;
    public int maxInventory;
    public List<Item> inventory;
    public List<Recipe> recipes;
    public List<Item> discoveredItems;
    public int maxMenuItems;
    public List<Dish> currentMenuItems;

    [Header("Physical Restaurant Items")]
    public RestaurantDoor mainDoor;
    public Seating allSeating;

    [Header("Restaurant Build Objects")]
    public GameObject floorHolder;
    public List<FloorPiece> allFloors;
    public List<GameObject> userPlacedWalls;
    public List<GameObject> userPlacedDoors;
    public List<Object> savedStuff;

}
