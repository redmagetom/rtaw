using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MAX INVENTORY 28 per container
public class Container : Interactable
{
    public int containerSize = 0;
    public List<Item> containerContents;
}
