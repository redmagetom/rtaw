using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.AI;

public class UIManager : MonoBehaviour
{
    public RosettaStone rs;
    public RestaurantInfo ri;
    public GameObject cameraController;
    public Camera mainCam;
    public Vector3 storedCamPos;
    public Quaternion storedCamRot;
    public Camera platingCam;
    public ContextMenu contextMenu;
    [Header("UI Status and Other")]
    private int UILayer = 5;
    public List<GameObject> allPanels;
    public GameObject activeObject;
    [Header("Statuses")]
    public bool draggingUIItem;
    public bool chopMenuOpen;
    public bool stoveMenuOpen;
    public bool plateMenuOpen;
    public bool plating;

    [Header("Other Buttons")]
    public GameObject exitPlatingButton;
   [Header("Main Sections")]
    public GameObject topBar;
    public GameObject bottomBar;
    public GameObject watchButton;
    public GameObject mapButton;
     [Header("Top Bar")]
    public Text totalMoneyDisplay;

    [Header("Large Pop up")]
    public GameObject largePopUpScreen;
    [Header("Restaurant Inventory")]
    public GameObject restInvPanel;
    public GameObject restInvHolder;
    public GameObject UIItemSlot;
    public Image itemDetailsImage;
    public Text itemDetailsDescription;
    public Text itemDetailsValue;
    public Text itemDetailsQuality;
    public Text itemDetailsFreshness;
    private int draggedItemInvPosition;
    private Item lastSelectedItem;
    [Header("Adding Recipe Stuff")]
    public GameObject createRecipeButton;
    public GameObject createRecipePanel;
    public InputField recipeName;
    public Text ingredientReadout;
    public Text processReadout;
    public GameObject addRecipeWarningSection;
    public Text addRecipeWarningText;
    
    public UIItemSlot draggedItem;
    [Header("Map Stuff")]
    public GameObject mapPanel;
    [Header("Grocery Panel")]
    public GameObject grocerPanel;

    [Header("Various Menus")]
    public ChopMenu chopMenu;
    public StoveMenu stoveMenu;
    public PlateMenu plateMenu;
    [Header("Recipe Book")]
    public RecipeUISection recipeUIItem;
    public GameObject recipeBookPanel;
    public DiscoveredItemPanel discoveredItemPanel;
    public GameObject componentRecipeSection;
    public GameObject componentRecipeSectionHolder;
    public GameObject recipeBookHoverPanel;
    [Header("Restaurant Menu")]
    public GameObject menuPanel;
    public GameObject menuItemsHolder;
    public GameObject availRecipesHolder;
    public Text restaurantNameDisplay;
    public GameObject menuItemPrefab;
    public GameObject addDishButton;
    public GameObject addDishPanel;
    public GameObject dishRecipeHolder;
    public InputField dishNameInput;

    // ------ OTHER OTHER STUFF -----------\
    [Header("Other")]

    // -------- HOVERING OVER UI ------------
    public float secondsToTriggerHoverEvent = 0.5f;
    public float objectHoverTime;
    public bool hovering;

    // ---------- STORED OBJECTS ------------
    private Plate selectedPlate;
    public List<int> platedItemInvIdx;
    public bool platingItem;
    

    // ---------saved camera information--------
    // plating locals
    private Vector3 platingCounterPos = new Vector3(0, 3, 3);
    private Vector3 platingCounterRot = new Vector3(40, 180, 0);
    private Vector3 platingPos = new Vector3(0, 1.2f, 1.2f);
    private Vector3 platingRot = new Vector3(45, -180, 0);

    [Header("Mode Toggle Stuff")]
    public GameObject restaurantModeScreens;

    [Header("Build Mode")]
    public BuildModeItem buildStyleSelectPrefab;
    public GameObject buildModeScreens;
    public BuildStyleBase selectedBuildStyle;
    public GameObject buildModeSelectionWindow;
    public GameObject buildModeSelectionHolder;

    void Start(){
        InitializeTopBar();
        UpdateInventorySlots();
        
    }


    void Update(){
        CheckHoveringStatus();


        if(draggingUIItem && draggedItem != null){
            draggedItem.transform.position = Input.mousePosition;
        }

        if(Input.GetMouseButtonUp(0)){
            if(draggingUIItem){
                ClickInventoryItem();   
            }
        }


        // left click
        if (Input.GetMouseButtonDown(0)){
        
            // Inventory stuff
            ClickInventoryItem();
          
            // grocer stuff
            if(grocerPanel.activeSelf){
                GrocerItemLeftClick();
            }

            if(plateMenuOpen){
                PlatingLeftClick();
            }

            if(addDishPanel.activeSelf){
                DishPanelLeftClick();
            }

        }



        // Right click
        if(Input.GetMouseButtonDown(1)){
            // contextMenu.transform.gameObject.SetActive(false);
            RaycastHit hit;
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)){
                if (hit.collider.gameObject.GetComponent<Interactable>() != null){
                    CreateContextMenu(hit);
                }
            }
        }
    }

    public void CheckHoveringStatus(){
        if(!hovering){
            objectHoverTime += Time.deltaTime;
            if(objectHoverTime > secondsToTriggerHoverEvent){
                GetHoverItems(GetEventSystemRaycastResults());
                hovering = true;
            }
        }
       
        if(Input.GetAxis("Mouse X") != 0){
            hovering = false;
            CheckUnHoverEvents();
            objectHoverTime = 0;
            return;
        }
        
    }

    private void CheckUnHoverEvents(){
        if(recipeBookHoverPanel.activeSelf){
            recipeBookHoverPanel.SetActive(false);
        }
    }


    public void GrocerMenuToggle(){
        if(!grocerPanel.activeSelf){
            grocerPanel.SetActive(true);
            grocerPanel.GetComponent<Grocer>().SelectRandomMerch();
            
            // get free slots
            int freeSlots = 0;
            for(var i = 0; i < ri.maxInventory; i++){
                if(ri.inventory[i]){
                    freeSlots += 1;
                }
            }

            grocerPanel.GetComponent<Grocer>().invCount.text = freeSlots + "/" + ri.maxInventory;
        } else {
            grocerPanel.SetActive(false);
        }
        TurnOffOtherPanels(grocerPanel);

    }

    public void UpdateAllUI(){
        UpdateInventorySlots();
        totalMoneyDisplay.text = ri.currentMoney.ToString();
    }


    public void InitializeTopBar(){
        totalMoneyDisplay.text = ri.currentMoney.ToString();
    }

    public void RestaurantInventoryToggle(){
        UpdateInventorySlots();
        if(!restInvPanel.activeSelf){
            restInvPanel.SetActive(true);
        } else {
            restInvPanel.SetActive(false);
        }
        TurnOffOtherPanels(restInvPanel);
    }

    public void RecipeBookToggle(){
        ReadyRecipesForBook();
        if(!recipeBookPanel.activeSelf){
            if(!discoveredItemPanel.populated){
                discoveredItemPanel.populated = true;
                discoveredItemPanel.PopulateDiscoveredItems();
            }
            recipeBookPanel.SetActive(true);

        } else {
            recipeBookPanel.SetActive(false);
        }
    }

    public void ReadyRecipesForBook(){
        foreach(Transform thing in componentRecipeSectionHolder.transform){
            Destroy(thing.gameObject);
        }

        foreach(Recipe rec in ri.recipes){
            var recItem = Instantiate(recipeUIItem);

            recItem.recipeName.text = rec.recipeName;
            if(rec.recipeType == Recipe.RecipeType.Component){
                recItem.transform.SetParent(componentRecipeSectionHolder.transform, worldPositionStays: false);
            } else {
                // add to dish section
            }
            
        }
    }
    public void PopUpAddComponentRecipe(){
        processReadout.text = "";
        createRecipePanel.SetActive(true);
        var proced = lastSelectedItem as ProcessedFood;
        ingredientReadout.text = proced.coreItem.itemName;
        foreach(string process in proced.processList){
            processReadout.text = processReadout.text + $"-{process} the {proced.coreItem.itemName}\n\n";
        }
    }

    public void AddComponentRecipe(){

        if(recipeName.text.Length < 5){
            addRecipeWarningSection.SetActive(true);
            addRecipeWarningText.text = "Recipe Name Must Be more than 5 Characters";
            return;
        }

        Recipe newRecipe = new Recipe();
        newRecipe.recipeName = recipeName.text;
        newRecipe.name = newRecipe.recipeName;
        newRecipe.recipeType = Recipe.RecipeType.Component;
        newRecipe.ingredients = new List<Food>();
        newRecipe.ingredients.Add((lastSelectedItem as ProcessedFood).coreItem as Food);
        newRecipe.steps = string.Join(",", (lastSelectedItem as ProcessedFood).processList);


        ri.recipes.Add(newRecipe);

        createRecipePanel.SetActive(false);
        lastSelectedItem = null;

        addRecipeWarningSection.SetActive(false);
        
    }



    public void UpdateInventorySlots(){
        foreach(Transform child in restInvHolder.transform){
            Destroy(child.gameObject);
        }
        for(var i = 0; i < ri.maxInventory; i++){
           var newSlot =  Instantiate(UIItemSlot);
           UIItemSlot ris = newSlot.GetComponent<UIItemSlot>();
           newSlot.transform.SetParent(restInvHolder.transform, worldPositionStays: false);

           if(ri.inventory[i] != null){
               if(!ri.discoveredItems.Contains(ri.inventory[i])){
                   ri.discoveredItems.Add(ri.inventory[i]);
               }
               ris.item = ri.inventory[i];
               ris.itemImage.sprite = ri.inventory[i].itemImage;
               newSlot.name = ri.inventory[i].itemName;
               newSlot.name = ri.inventory[i].itemName;
           } else {
               ris.itemImage.gameObject.SetActive(false);
               newSlot.name = "Empty Slot";
           }
        }
    }

    public void CheckForContextClose(){
        // if(!IsPointerOverUIElement()){contextMenu.gameObject.SetActive(false);}
    }

    public void MenuPanelToggle(){
        if(!menuPanel.activeSelf){
            menuPanel.SetActive(true);
            PopulateMenuPanels();
        } else {
            menuPanel.SetActive(false);
        }
    }

    public void CreateDishPanelToggle(){
        foreach(Transform child in availRecipesHolder.transform){
            Destroy(child.gameObject);
        }

        foreach(Transform child in dishRecipeHolder.transform){
            Destroy(child.gameObject);
        } 


        foreach(var availRecipe in ri.recipes){
            var menuItem = Instantiate(menuItemPrefab);
            menuItem.transform.SetParent(availRecipesHolder.transform, worldPositionStays: false);
            menuItem.GetComponent<UIMenuItemSection>().recipeName.text = availRecipe.recipeName;
            menuItem.GetComponent<UIMenuItemSection>().attachedRecipe = availRecipe;
        }


        if(!addDishPanel.activeSelf){
            addDishPanel.SetActive(true);
        } else {
            addDishPanel.SetActive(false);
        }

    }

    public void CreateDish(){
        Dish newDish = new Dish();
        newDish.containedRecipes = new List<Recipe>();
        foreach(Transform recipe in dishRecipeHolder.transform){
            newDish.containedRecipes.Add(recipe.gameObject.GetComponent<UIMenuItemSection>().attachedRecipe);
        }
        newDish.dishName = dishNameInput.text;
        newDish.name = newDish.dishName;
        for(var i = 0; i < ri.currentMenuItems.Count; i ++){
            if(ri.currentMenuItems[i] == null){
                ri.currentMenuItems[i] = newDish;
                break;
            }
        }

        PopulateMenuPanels();
        CreateDishPanelToggle();

    }


    private void PopulateMenuPanels(){

        int activeRecipes = 0;

        foreach(Transform child in menuItemsHolder.transform){
            if(child.gameObject.GetComponent<UIMenuItemSection>() != null){
                Destroy(child.gameObject);
            }
        }


        for(var i = 0; i < ri.maxMenuItems; i++){
            if(ri.currentMenuItems[i] != null){
                var menuItem = Instantiate(menuItemPrefab);
                menuItem.transform.SetParent(menuItemsHolder.transform, worldPositionStays: false);
                menuItem.GetComponent<UIMenuItemSection>().recipeName.text = ri.currentMenuItems[i].dishName;
                activeRecipes += 1;
            }
        }
        
        if(activeRecipes < ri.maxMenuItems){
            addDishButton.SetActive(true);
        } else {
            addDishButton.SetActive(false);
        }

        addDishButton.transform.SetAsLastSibling();

    }

    public void ChopMenuToggle(){
        if(!chopMenuOpen){
            chopMenuOpen = true;
            chopMenu.chopSelection.SetActive(true);
        } else {
            chopMenuOpen = false;
            chopMenu.chopSelection.SetActive(false);
        }
        
        
        
    }

    public void StoveMenuToggle(){
        if(!stoveMenuOpen){
            stoveMenuOpen = true;
            stoveMenu.cookSelection.SetActive(true);
        } else {
            stoveMenuOpen = false;
            stoveMenu.cookSelection.SetActive(false);
        }
    }


    public void PlateItemsMenuToggle(){
        StartCoroutine(I_PlatingMenuToggle());
    }

    private IEnumerator I_PlatingMenuToggle(){
        yield return null;
        while(Vector3.Distance(rs.player.transform.position, contextMenu.obj.gameObject.transform.position) > 2){
            if(Vector3.Distance(rs.player.transform.position, contextMenu.obj.gameObject.transform.position) < 2){
                break;
            }
            rs.player.GetComponent<NavMeshAgent>().destination = contextMenu.obj.gameObject.GetComponent<Interactable>().interactionLocation.transform.position;
            yield return new WaitForSeconds(1f);
        }
        if(!plateMenuOpen){
            storedCamPos = mainCam.transform.localPosition;
            storedCamRot = mainCam.transform.localRotation;
            plateMenuOpen = true;
            // plateMenu.platingSection.SetActive(true);
            mainCam.transform.SetParent(contextMenu.obj.gameObject.transform);
            LeanTween.moveLocal(mainCam.gameObject, platingCounterPos, 1);
            LeanTween.rotateLocal(mainCam.gameObject, platingCounterRot, 1);

        } else {
            mainCam.transform.SetParent(cameraController.transform);
            LeanTween.moveLocal(mainCam.gameObject, storedCamPos, 1);
            LeanTween.rotateLocal(mainCam.gameObject, storedCamRot.eulerAngles, 1);
            plateMenuOpen = false;
            // plateMenu.platingSection.SetActive(false);
        }
    }
    public void CreateContextMenu(RaycastHit hit){
        activeObject = hit.collider.gameObject;
        contextMenu.obj = activeObject.GetComponent<Interactable>();
        var screenSize = new Vector2(Screen.width, Screen.height);
        if(!contextMenu.transform.gameObject.activeSelf){contextMenu.transform.gameObject.SetActive(true);}
        var createPos = new Vector2(Input.mousePosition.x + (Screen.height * 0.025f), Input.mousePosition.y - (Screen.height * 0.025f));
        contextMenu.GetComponent<RectTransform>().position = createPos;
        contextMenu.LaunchMenu();
    }

    private void TurnOffOtherPanels(GameObject selectedPanel){
        foreach(Transform child in largePopUpScreen.transform){
            if(child.gameObject != selectedPanel){
                child.gameObject.SetActive(false);
            }
        }
    }


// ---------- GROCER ---------
public void GrocerItemLeftClick(){
    GrocerItemLeftClick(GetEventSystemRaycastResults());
}

private void GrocerItemLeftClick(List<RaycastResult> eventSystemRaysastResults){
    
        for (int index = 0; index < eventSystemRaysastResults.Count; index++) {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer) {
               
                // clicked in the grocer inventory
                if(curRaysastResult.gameObject.GetComponent<GrocerInventoryItem>()){    
                    Grocer grocer = grocerPanel.GetComponent<Grocer>();
                    int freeSlots = int.Parse(grocer.invCount.text.Split('/')[0]);

                    GrocerInventoryItem clicked = curRaysastResult.gameObject.GetComponent<GrocerInventoryItem>();
                    Item gii = clicked.item;
                    int clickedIdx = curRaysastResult.gameObject.transform.GetSiblingIndex();
                    
                    
                    // DO THINGS FORM CHECKOUT
                    if(clicked.inCheckout){
                        grocer.merchandise.Add(gii);
                        grocer.AddMerchItem(gii);
                        grocer.invCount.text = (freeSlots-1) + "/" + ri.maxInventory;
                        // grocer.checkoutItems.RemoveAt(clickedIdx);
                        grocer.checkoutTotal.text = (int.Parse(grocer.checkoutTotal.text) - gii.value).ToString();
                        Destroy(grocer.checkoutItemHolder.transform.GetChild(clickedIdx).gameObject);
                        return;
                    }

                    // DO THINGS IF IN GROCER INVENTORY
                    if(freeSlots == ri.maxInventory){
                        print("NO free slots");
                        return;
                    }
                    grocer.checkoutItems.Add(gii);
                    grocer.merchandise.RemoveAt(clickedIdx);
                    Destroy(grocer.groceryItemHolder.transform.GetChild(clickedIdx).gameObject);

                    var checkoutItem = Instantiate(grocer.grocerCheckoutPrefab);
                    checkoutItem.transform.SetParent(grocer.checkoutItemHolder.transform, worldPositionStays: false);
                    GrocerInventoryItem cii = checkoutItem.GetComponent<GrocerInventoryItem>();
                    Item ci = gii;
                    
                    cii.item = gii;
                    cii.itemName.text = ci.itemName;
                    cii.itemImage.sprite = ci.itemImage;
                    cii.inCheckout = true;
                    cii.itemCost.text = ci.value.ToString();

                    grocer.checkoutTotal.text = (int.Parse(grocer.checkoutTotal.text) + ci.value).ToString();
                    grocer.invCount.text = (freeSlots+1) + "/" + ri.maxInventory;
                }
            }
        }
}


// ----------- MENU --------------------
private void DishPanelLeftClick(){
    DishPanelLeftClick(GetEventSystemRaycastResults());
}

private void DishPanelLeftClick(List<RaycastResult> eventSystemRaysastResults){
    for (int index = 0; index < eventSystemRaysastResults.Count; index++) {
        RaycastResult curRaysastResult = eventSystemRaysastResults[index];
        if (curRaysastResult.gameObject.layer == UILayer) {
            
            // clicked in the create dish panel
            if(curRaysastResult.gameObject.GetComponent<UIMenuItemSection>()){   
                var dish = curRaysastResult.gameObject.GetComponent<UIMenuItemSection>();
                if(dish.transform.parent == availRecipesHolder.transform){
                    dish.transform.SetParent(dishRecipeHolder.transform, worldPositionStays: false);
                    return;
                }

                if(dish.transform.parent == dishRecipeHolder.transform){
                    dish.transform.SetParent(availRecipesHolder.transform, worldPositionStays: false);
                    return;
                }
                
                // PopulateMenuPanels();
            }
        }
    }
}

// ------------- PLATING ----------------
private void PlatingLeftClick(){
    if(restInvPanel.activeSelf || platingItem){return;}
    RaycastHit hit;
    Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
    if (Physics.Raycast(ray, out hit)){
        if (hit.collider.gameObject.GetComponent<Plate>() != null && !plating){
            plating = true;
            selectedPlate = hit.collider.gameObject.GetComponent<Plate>();
            selectedPlate.ReadyPlating();
            mainCam.transform.SetParent(hit.collider.gameObject.transform);
            LeanTween.moveLocal(mainCam.gameObject, platingPos, 1);
            LeanTween.rotateLocal(mainCam.gameObject, platingRot, 1);
            ToggleUIForPlating();
            return;
        }
      
        if(plating && selectedPlate && hit.transform.tag == "PlatedItem" && !selectedPlate.platingAreas.Contains(hit.transform.gameObject)){
            if(selectedPlate.platedItems[hit.transform.parent.GetSiblingIndex()] != null){
                var platedItemToRemove = (selectedPlate.platedItems[hit.transform.parent.GetSiblingIndex()]);
                
                for(var i = 0; i < ri.inventory.Count; i++){
                    if(ri.inventory[i] == null){
                        ri.inventory[i] = selectedPlate.platedItems[hit.transform.parent.GetSiblingIndex()];
                        break;
                    }
                }
                

                Destroy(hit.transform.gameObject);
                selectedPlate.platingAreas[hit.transform.parent.GetSiblingIndex()].gameObject.SetActive(true);
                selectedPlate.platedItems[hit.transform.parent.GetSiblingIndex()] = null;
            
                UpdateInventorySlots();

                return;
            }

        }

        if(hit.transform.tag == "PlatingPosition"){
            platingItem = true;
            selectedPlate = hit.collider.transform.parent.transform.parent.gameObject.GetComponent<Plate>();
            selectedPlate.selectedPlatePosition = hit.transform.parent.transform.GetSiblingIndex();
            RestaurantInventoryToggle();
        }

     
    }

}

private void ToggleUIForPlating(){
    if(plating){
        topBar.SetActive(false);
        bottomBar.SetActive(false);
        watchButton.SetActive(false);
        mapButton.SetActive(false);
        exitPlatingButton.SetActive(true);
    } else {
        topBar.SetActive(true);
        bottomBar.SetActive(true);
        watchButton.SetActive(true);
        mapButton.SetActive(true);
        exitPlatingButton.SetActive(false);
    }

}

public void ExitPlating(){
    plating = false;
    selectedPlate.HidePlatingOptions();
    mainCam.transform.SetParent(selectedPlate.gameObject.transform.parent.transform);
    LeanTween.moveLocal(mainCam.gameObject, platingCounterPos, 1);
    LeanTween.rotateLocal(mainCam.gameObject, platingCounterRot, 1);
    selectedPlate = null;
    
    ToggleUIForPlating();
}

// ---------- MAP --------------
    public void MapMenuToggle(){
        if(!mapPanel.activeSelf){
            mapPanel.SetActive(true);
        } else {
            mapPanel.SetActive(false);
        }
        TurnOffOtherPanels(mapPanel);
    }

// ------------- INVENTORY -------------
    public void ClickInventoryItem()
    {
        ClickInventoryItem(GetEventSystemRaycastResults());
    }
 
 

    private void ClickInventoryItem(List<RaycastResult> eventSystemRaysastResults) {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++) {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer) {
                // print(curRaysastResult.gameObject.name);
                if(curRaysastResult.gameObject.GetComponent<UIItemSlot>()){
 
                    var selItem = curRaysastResult.gameObject;
                    var info = selItem.GetComponent<UIItemSlot>();
                   
                    
                        

                    // ----------- Plating Actions --------------
                    if(plating){
                        if(info.item == null){return;}
                        selectedPlate.platedItems[selectedPlate.selectedPlatePosition] = info.item as Food;
                        GameObject createdItemModel = Instantiate(info.item.itemModel);
                        createdItemModel.transform.SetParent(selectedPlate.platingAreas[selectedPlate.selectedPlatePosition].transform.parent.transform);
                        createdItemModel.transform.localPosition = new Vector3(0,0,0);
                        createdItemModel.tag = "PlatedItem";
                        selectedPlate.platingAreas[selectedPlate.selectedPlatePosition].gameObject.SetActive(false);
                        platedItemInvIdx.Add(selItem.transform.GetSiblingIndex());
                        
                        ri.inventory[selItem.transform.GetSiblingIndex()] = null;
                       
                        UpdateInventorySlots();
                        RestaurantInventoryToggle();
                        StartCoroutine(ChangePlatingItemBool());
                        return;
                    }
              


                    // ------------ Chop Menu Actions -----------------
                    if(chopMenuOpen){
                       
                        // open the menu to select an item
                        if(!restInvPanel.activeSelf){
                            restInvPanel.SetActive(true);
                            return;
                        }

                        chopMenu.selectedIdx = selItem.transform.GetSiblingIndex();
                        chopMenu.selectedItem.item = info.item;
                        chopMenu.selectedItem.itemImage.sprite = info.itemImage.sprite;
                        restInvPanel.SetActive(false);
                        return;
                    }
        

                    
                    // --------------- Stove Menu Actions -------------
                    if(stoveMenuOpen){    
                        // open the menu to select an item
                        if(!restInvPanel.activeSelf){
                            restInvPanel.SetActive(true);
                            return;
                        }

                        stoveMenu.selectedIdx = selItem.transform.GetSiblingIndex();
                        stoveMenu.selectedItem.item = info.item;
                        stoveMenu.selectedItem.itemImage.sprite = info.itemImage.sprite;
                        restInvPanel.SetActive(false);
                        return;
                    }


                    // Select an item from the inventory and assign it if not currentlyd ragging
                    if(draggedItem.item == null){
                        if(info.item == null){return;}
       
                        // print("picking up object");
                        draggedItemInvPosition = selItem.transform.GetSiblingIndex();
                        draggedItem.transform.position = Input.mousePosition;
                        draggedItem.gameObject.SetActive(true);
                        draggedItem.itemImage.sprite = info.itemImage.sprite;
                        draggedItem.item = info.item;
                        draggingUIItem = true;
              

                    // show details on details section
                    if(restInvPanel.activeSelf){
                        itemDetailsImage.sprite = draggedItem.item.itemImage;
                        itemDetailsDescription.text = draggedItem.item.itemDescription;
                        itemDetailsValue.text = draggedItem.item.value.ToString();

                        Food foodinfo = draggedItem.item as Food;
                        ProcessedFood procFood = draggedItem.item as ProcessedFood;
                        // Food food = draggedItem.item as Food;

                        if(foodinfo != null){
                            // TODO: MAKE BETTER UI TO DISPLAY DAYS/HOURS  
                            itemDetailsFreshness.text = foodinfo.freshTime.ToString();
                        }
                        if(procFood != null){
                            itemDetailsQuality.text = procFood.processQuality.ToString();
                        }
                        
                    }





                        info.itemImage.gameObject.SetActive(false);
                        info.item = null;
                        ri.inventory[draggedItemInvPosition] = null;

                        // change layer to ignore item in further raycasts
                        draggedItem.gameObject.layer = 6;
                        UpdateInventorySlots();
                        return;

                        // if currently dragging an item do other things
                    } else { 
                        // print("Hitting over object");
                        // print(selItem.name);
                        if(info.item != null){  
                            ri.inventory[draggedItemInvPosition] = info.item;
                            var oldSlot = restInvHolder.transform.GetChild(draggedItemInvPosition).GetComponent<UIItemSlot>();
                            oldSlot.item = info.item;
                            oldSlot.itemImage.sprite = info.itemImage.sprite;                     
                        } 

                        info.item = draggedItem.item;
                        info.itemImage = draggedItem.itemImage;
                        ri.inventory[selItem.transform.GetSiblingIndex()] = draggedItem.item;
                        lastSelectedItem = info.item;    

                        draggedItem.item = null;
                        draggedItem.gameObject.SetActive(false);
                        draggingUIItem = false;

                        UpdateInventorySlots();
                        return;
                    }


                
                   
                } 
            }
        }
        
        // if dragging and item, and over nothing, return to prev position
        if(draggingUIItem){
            print("hitting nothing");
            ri.inventory[draggedItemInvPosition] = draggedItem.item;
            var oldItem = restInvHolder.transform.GetChild(draggedItemInvPosition).GetComponent<UIItemSlot>();
            oldItem.item = draggedItem.item;
            oldItem.itemImage.sprite = draggedItem.itemImage.sprite;

            draggedItem.item = null;
            draggedItem.gameObject.SetActive(false);
            draggingUIItem = false;
            UpdateInventorySlots();
            return;
        }


    }
 
    private void GetHoverItems(List<RaycastResult> eventSystemRaysastResults){
        for (int index = 0; index < eventSystemRaysastResults.Count; index++) {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer) {
                if(recipeBookPanel.activeSelf){       
                    if(curRaysastResult.gameObject.GetComponent<RecipeUISection>() != null){
                        recipeBookHoverPanel.SetActive(true);
                    }
                }
          
            }
        }
    }
    // //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }


    private IEnumerator ChangePlatingItemBool(){
        yield return new WaitForSeconds(0.25f);
        platingItem = false;
    }




// --------------------- BUILD MODE ------------------------------ 
    public void ToggleBuildMode(){
        if(rs.gpm.currentMode == GameplayManager.CurrentMode.RestaurantMode){
            rs.gpm.currentMode =  GameplayManager.CurrentMode.BuildMode;
            restaurantModeScreens.SetActive(false);
            buildModeScreens.SetActive(true);
        } else {
            rs.gpm.currentMode =  GameplayManager.CurrentMode.RestaurantMode;
            restaurantModeScreens.SetActive(true);
            buildModeScreens.SetActive(false);
            rs.gpm.HideBuildSpots();
            if(rs.gpm.roomDefinitionHolder.activeSelf){
                rs.gpm.ToggleShowRoomDef();
            }
        }
    }

    private void ClearStyleSelection(){
        selectedBuildStyle = null;
        foreach(Transform child in buildModeSelectionHolder.transform){
            Destroy(child.gameObject);
        }
    }
    
    public void PopulateWallStyleSelection(){
        ClearStyleSelection();
        foreach(var wall in rs.wallTypeVault){
            if(wall.unlocked){
                var selector = Instantiate(buildStyleSelectPrefab);
                var bmi = selector.GetComponent<BuildModeItem>();
                bmi.styleBase = wall;
                bmi.styleImage.sprite = wall.uiSprite;
                selector.transform.SetParent(buildModeSelectionHolder.transform, worldPositionStays: false);
                selector.GetComponent<Button>().onClick.AddListener(delegate{selectedBuildStyle = wall;});
            }
        }
    }

}




