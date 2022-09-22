using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe : ScriptableObject
{
    public enum RecipeType{Component, Dish}
    public RecipeType recipeType;
    public string recipeName;
    public List<Food> ingredients;
    public string steps;
}
