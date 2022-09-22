using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ES3Serializable]
[CreateAssetMenu(fileName = "New Food", menuName ="Items/Food")]
public class Food : Item
{
    public Item baseitem;
    public bool vegetarian;
    // TODO: Add Region of origin to distinguish for grocer
    public int freshTime;
    public bool unprocessed = true;
    public bool fullyProcessed;

    [Header("Flavor Profiles")]
// Flavor Profiles Salty, "Sweet, "Sour, Bitter, Umami, Spicy
    public float saltyValue;
    public float sweetValue;
    public float sourValue;
    public float bitterValue;
    public float umamiValue;
    public float spicyValue;

}
