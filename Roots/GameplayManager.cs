using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class GameplayManager : MonoBehaviour
{
    public enum CurrentMode {RestaurantMode, BuildMode, DefineRooms}
    public CurrentMode currentMode;
    public enum SelectedBuildCategory{None, Floors, Walls, Doors, Kitchen, Dining, Bedroom, Decor, Required}
    public SelectedBuildCategory buildCategory;
    public enum BuildEditMode{None, Placement, Style}
    public BuildEditMode buildEditMode;
    public RestaurantInfo ri;
    public RosettaStone rs;
    public GameObject patronStagingPos;
    public Patron patronBasePrefab;
    [Header("Patron Lists")]
    public List<Patron> activePatrons;
    public List<Patron> patronsNeedingGreeting;
    [Header("Host Lists")]
    public List<Host> activeHosts;

    [Header("Build Mode Stuff")]
    public FloorPiece floorPiece;
    public RestaurantDoor mainDoor;
    [Header("Rooms")]
    private List<GameObject> tempRoomList;
    private Vector3 savedCamPosBeforeRooms;
    private Quaternion savedCamRotBeforeRooms;
     public GameObject roomDefinitionHolder;
    public Room.RoomType definingRoomType = Room.RoomType.None;
    public List<GameObject> diningRoomTiles;
    public List<GameObject> kitchenTiles;
    public List<GameObject> bedroomTiles;
   
    void Start(){
        HideBuildSpots();
    }

    void Update(){
        if(Input.GetMouseButtonDown(0)){

            // ----------- BUILD MODE -------------
            if(currentMode == CurrentMode.BuildMode){
                RaycastHit hit;
                Ray ray = rs.mainCam.ScreenPointToRay(Input.mousePosition);


                // ------------ FLOORS -----------
                if(buildCategory == SelectedBuildCategory.Floors){
                    if(buildEditMode == BuildEditMode.Placement){
                        if(Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Floor Build Spot"))){
                            PlaceFloor(hit.transform.position);
                            return;
                        }
                        if(Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Floors"))){
                            // if(hit.transform.gameObject.GetComponent<Terrain>() == null){
                            //     return;
                            // }
                            if(ri.allFloors.Count == 1){
                                Debug.Log("Cant Delete Last Floor");
                                return;
                            }
                            if(hit.transform.gameObject.GetComponent<FloorPiece>() != null){
                                StartCoroutine(RemoveFloor(hit.transform.gameObject));
                            }    
                            return;
                        }
                    }

                }

                // ---------- WALLS --------------
                if(buildCategory == SelectedBuildCategory.Walls){
                    if(buildEditMode == BuildEditMode.Placement){
                    // --------- Placing User Walls -------- 
                        if(Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Wall Placement Spot"))){
                            StartCoroutine(PlaceWall(hit.transform.gameObject));
                            return;
                        }

                        if(Physics.Raycast(ray, out hit, Mathf.Infinity)){
                            if(ri.userPlacedWalls.Contains(hit.transform.gameObject)){
                                StartCoroutine(RemoveWall(hit.transform.gameObject));
                                return;
                            }
                        }  
                    }

                    if(buildEditMode == BuildEditMode.Style){
                        if(Physics.Raycast(ray, out hit)){
                            // outer wall piece
                            Debug.Log(hit.transform.gameObject.name);
                            
                            if(hit.transform.parent.gameObject.GetComponent<WallPiece>() != null){
                                var wp = hit.transform.parent.gameObject.GetComponent<WallPiece>();
                                wp.wallType = rs.uiManager.selectedBuildStyle as WallType;
                                hit.transform.gameObject.GetComponent<MeshRenderer>().material = rs.uiManager.selectedBuildStyle.mat;
                                return;
                            }

                            if(hit.transform.gameObject.GetComponent<UserWall>() != null){
                                var uw = hit.transform.gameObject.GetComponent<UserWall>();
                                // Debug.Log("clicked user wall");
                                uw.wallType = rs.uiManager.selectedBuildStyle as WallType;
                                hit.transform.gameObject.GetComponent<MeshRenderer>().material = rs.uiManager.selectedBuildStyle.mat;
                                return;
                            }
                        }
                    }
      
                }

                // -------- DOORS -------------------
                if(buildCategory == SelectedBuildCategory.Doors){
                    if(buildEditMode == BuildEditMode.Placement){
                        if(Physics.Raycast(ray, out hit, Mathf.Infinity)){
                            if(ri.userPlacedWalls.Contains(hit.transform.gameObject)){
                                StartCoroutine(PlaceDoor(hit.transform.gameObject));
                                return;
                            }
                            if(ri.userPlacedDoors.Contains(hit.transform.gameObject)){
                                StartCoroutine(RemoveDoor(hit.transform.gameObject));
                            }
                        }
                    }
                }
  
            }
        
        
            // --------- DEFINE ROOM ------------
            if(currentMode == CurrentMode.DefineRooms){
                if(definingRoomType == Room.RoomType.None){
                    Debug.Log("Please Select Room Type");
                    return;
                }
                RaycastHit hit;
                Ray ray = rs.mainCam.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Floors"))){
                    if(hit.transform.gameObject.GetComponent<FloorPiece>() != null){
                        StartCoroutine(FindRooms(hit.transform.gameObject));
                    }
                }
                
            }
        }
    }
    void FixedUpdate(){
        HostsLookForPatrons();
    }



    public void HostsLookForPatrons(){
        foreach(Host host in activeHosts){
            if(host.hostState == Host.HostState.Standby){
                // have hosts look for patrons wiating to be greeted and taken to table
            
                for(var i = 0; i < patronsNeedingGreeting.Count; i++){
                    host._nma.destination = patronsNeedingGreeting[i].transform.position;
                    host.hostState = Host.HostState.GreetingPatron;
                    host.tiedPatrons.Add(patronsNeedingGreeting[i]);
                    host.targetedPatron = patronsNeedingGreeting[i];
                    patronsNeedingGreeting.Remove(patronsNeedingGreeting[i]);
                }
            }

        }

        foreach(Patron patron in activePatrons){
            // add patrons to the queue waiting for a seat
            if(!patron.greeted && patron.greetingState == Patron.GreetingStates.ReadyToBeGreeted&& !patronsNeedingGreeting.Contains(patron)){
                patronsNeedingGreeting.Add(patron);
            }
        }
    }


    public void SpawnPatron(){
        Patron newPatron = Instantiate(patronBasePrefab, patronStagingPos.transform.position, Quaternion.identity);
        activePatrons.Add(newPatron);
        var nvm = newPatron.GetComponent<NavMeshAgent>();
        nvm.destination = ri.mainDoor.waitingLocation.transform.position;
    }
   

    public void ToggleFloorPlacementMode(){
        FindAllFloorBuildSpots();
    }

    // ---------- Build Categories --------- 
    public void SetBuildEditToPlace(){
        rs.uiManager.buildModeSelectionWindow.SetActive(false);

        buildEditMode = BuildEditMode.Placement;
        if(buildCategory == SelectedBuildCategory.Floors){
            ToggleFloorPlacementMode();
        }
        if(buildCategory == SelectedBuildCategory.Walls){
            HideBuildSpots();
            foreach(var floor in ri.allFloors){
                floor.GetPossibleWallPositions();
            }
        }
    }

    public void SetBuildEditToStyle(){
        buildEditMode = BuildEditMode.Style;
        HideBuildSpots();
        rs.uiManager.buildModeSelectionWindow.SetActive(true);

        if(buildCategory == SelectedBuildCategory.Walls){
            rs.uiManager.PopulateWallStyleSelection();
        }
        

    }

    public void SetCatToFloors(){
        buildCategory = SelectedBuildCategory.Floors;
        if(buildEditMode == BuildEditMode.Placement){
            ToggleFloorPlacementMode();
        }
    }



    public void SetCatToWalls(){
        buildCategory = SelectedBuildCategory.Walls;
        HideBuildSpots();
        if(buildEditMode == BuildEditMode.Placement){
            foreach(var floor in ri.allFloors){
                floor.GetPossibleWallPositions();
            }
        }
 
    }

    public void SetCatToDoors(){
        buildCategory = SelectedBuildCategory.Doors;
        HideBuildSpots();
    }

    public void SetCatToReq(){
        buildCategory = SelectedBuildCategory.Required;
        HideBuildSpots();
    }




// -------- BUILD MODE STUFF ------------

    public void FindAllFloorBuildSpots(){
        foreach(var floor in ri.allFloors){
            foreach(var placement in floor.placementOptions){
                if(placement.activeSelf){
                    placement.SetActive(false);
                } 
            }

            Debug.Log(floor.name);
            foreach(var wall in floor.wallPieces){     
                wall.gameObject.SetActive(false);                   
            }
            
            foreach(var userwall in ri.userPlacedWalls){
                userwall.SetActive(false);
            }
        
            floor.GetAvailableSpaces();
        }
    }

    public void HideBuildSpots(){
        bool needWalls = true;
        foreach(var floor in ri.allFloors){
            foreach(var placement in floor.placementOptions){
                if(placement.activeSelf){
                    placement.SetActive(false);
                }
            }

            // see if walls need to pop back in
            foreach(var walltest in floor.wallPieces){
                if(walltest.gameObject.activeSelf){
                    needWalls = false;
                    break;
                }
            }

            // pop in walls if needed
            if(needWalls){
                foreach(var wall in floor.wallPieces){
                    wall.DetectNeed();
                }
            }

            foreach(var userwall in ri.userPlacedWalls){
                userwall.SetActive(true);
            }

            foreach(var wallPlacement in floor.wallPlacements){
                wallPlacement.SetActive(false);
            }
        }
    }

    public void PlaceFloor(Vector3 pos){
        StartCoroutine(I_PlaceFloor(pos));
    }

    private IEnumerator I_PlaceFloor(Vector3 pos){
        pos.y = 0;
        var newFloorPiece = Instantiate(floorPiece, pos, Quaternion.identity);
        newFloorPiece.transform.LeanScale(new Vector3(0,0,0), 0);
        newFloorPiece.transform.SetParent(ri.floorHolder.transform);
        LeanTween.scale(newFloorPiece.gameObject, new Vector3(1,1,1), 0.1f);
        yield return new WaitForSeconds(0.1f);
        var fp = newFloorPiece.GetComponent<FloorPiece>();      
        AudioSource s = newFloorPiece.gameObject.AddComponent<AudioSource>();
        s.pitch = 1 + Random.Range(-0.5f, 0.5f);
        s.PlayOneShot(fp.floorPlacementSound);
        // yield return new WaitForSeconds(0.1f);
        ri.allFloors.Add(newFloorPiece);
        FindAllFloorBuildSpots();
        yield return new WaitForSeconds(1f);
        Destroy(s);
    }

    public IEnumerator RemoveFloor(GameObject floorPiece){
        foreach(var floorPlace in floorPiece.GetComponent<FloorPiece>().placementOptions){
            floorPlace.SetActive(false);
        }
        LeanTween.scale(floorPiece, new Vector3(0,0,0), 0.1f);  
        var fp = floorPiece.GetComponent<FloorPiece>();      
        foreach(var uw in fp.userWalls){
            if(ri.userPlacedWalls.Contains(uw)){
                ri.userPlacedWalls.Remove(uw);
            }
        }
        AudioSource s = floorPiece.AddComponent<AudioSource>();
        s.pitch = 1 + Random.Range(-0.5f, 0.5f);
        s.PlayOneShot(fp.floorPlacementSound);
        yield return new WaitForSeconds(0.1f);
        ri.allFloors.Remove(floorPiece.GetComponent<FloorPiece>());
        yield return new WaitForSeconds(0.1f);
        Destroy(floorPiece);
        yield return new WaitForEndOfFrame();
        FindAllFloorBuildSpots();
    }

    public IEnumerator PlaceWall(GameObject hit){
        var idx = hit.transform.GetSiblingIndex();
        GameObject wallPiece = hit.transform.parent.transform.parent.GetComponent<FloorPiece>().userWalls[idx].gameObject;
        Debug.Log(hit.transform.gameObject.name);
        hit.transform.gameObject.GetComponent<WallPlacement>().markedForDel = true;
        wallPiece.SetActive(true);
        var storedPos = wallPiece.transform.position;
        Vector3 animStartPos = storedPos;
        animStartPos.y += 30;

        wallPiece.transform.position = animStartPos;
        float animTime = 0.25f * (Random.Range(1, 10)*0.1f);
        
        LeanTween.move(wallPiece, storedPos, animTime).setEaseOutQuint();
        var floorObject = wallPiece.transform.parent.transform.parent.gameObject;
        FloorPiece fp = floorObject.GetComponent<FloorPiece>();
        // AudioSource aud = floorObject.GetComponent<AudioSource>();
        
        AudioSource s = wallPiece.AddComponent<AudioSource>();
        s.pitch = 1 + Random.Range(-0.5f, 0.5f);
        yield return new WaitForSeconds(animTime);
        s.PlayOneShot(fp.wallPlacementSound);
        ri.userPlacedWalls.Add(wallPiece);
        
        
    }

    public IEnumerator RemoveWall(GameObject hit){
        FloorPiece theFloor = hit.transform.parent.transform.parent.gameObject.GetComponent<FloorPiece>();
        int wallIdx = hit.transform.GetSiblingIndex();
        ri.userPlacedWalls.Remove(hit.transform.gameObject);
        hit.transform.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        theFloor.wallPlacements[wallIdx].SetActive(true);
    }

    public IEnumerator PlaceDoor(GameObject hit){
        FloorPiece theFloor = hit.transform.parent.transform.parent.gameObject.GetComponent<FloorPiece>();
        int wallIdx = hit.transform.GetSiblingIndex();
        ri.userPlacedWalls.Remove(hit.transform.gameObject);
        hit.transform.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        theFloor.doors[wallIdx].SetActive(true);
        ri.userPlacedDoors.Add(theFloor.doors[wallIdx]);
    }
    public IEnumerator RemoveDoor(GameObject hit){
        FloorPiece theFloor = hit.transform.parent.transform.parent.gameObject.GetComponent<FloorPiece>();
        int doorIdx = hit.transform.GetSiblingIndex();
        ri.userPlacedDoors.Remove(hit.transform.gameObject);
        hit.transform.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        theFloor.userWalls[doorIdx].SetActive(true);
        ri.userPlacedWalls.Add(theFloor.userWalls[doorIdx]);
    }

    // ------------ ROOM STUFF --------------
    public void ToggleShowRoomDef(){
        if(!roomDefinitionHolder.activeSelf){
            HideBuildSpots();
            roomDefinitionHolder.SetActive(true);  
            currentMode = CurrentMode.DefineRooms;  
            definingRoomType = Room.RoomType.None;
            Vector3 curPos = rs.mainCamHolder.transform.localPosition;
            savedCamPosBeforeRooms = curPos;
            savedCamRotBeforeRooms = rs.mainCamHolder.transform.rotation; 

            LeanTween.move(rs.mainCamHolder, CenterCam(curPos.y), 0.25f);
            LeanTween.rotateLocal(rs.mainCam.gameObject, new Vector3(90,0,0), 0.25f);

        } else {
            HideRoomHighlights();
            roomDefinitionHolder.SetActive(false);
            definingRoomType = Room.RoomType.None;
            currentMode = CurrentMode.BuildMode;

            Vector3 curPos = rs.mainCamHolder.transform.localPosition;
            LeanTween.move(rs.mainCamHolder, savedCamPosBeforeRooms , 0.25f);
            LeanTween.rotateLocal(rs.mainCam.gameObject, new Vector3(50,0,0), 0.25f);
            LeanTween.rotateLocal(rs.mainCamHolder, savedCamRotBeforeRooms.eulerAngles, 0.25f);
        }
        
    }

    private Vector3 CenterCam(float yVal){
        float zCenter = 0;
        float xCenter = 0;
        int count = 0;
        foreach(var floor in ri.allFloors){
            Vector3 transformed = transform.TransformPoint(floor.transform.position);
            zCenter += transformed.z;
            xCenter += transformed.x;
            count += 1;
        }
        return new Vector3(xCenter/count, yVal, (zCenter/count));
    }
    private void HideRoomHighlights(){
        foreach(var t in ri.allFloors){
            t.roomHighlight.SetActive(false);
        }
    }
    public void SetRoomDefKitchen(){
        HideRoomHighlights();
        definingRoomType = Room.RoomType.Kitchen;
        foreach(var tile in kitchenTiles){
            tile.GetComponent<FloorPiece>().roomHighlight.SetActive(true);
        }
    }
    public void SetRoomDefDining(){
        HideRoomHighlights();
        definingRoomType = Room.RoomType.Dining;
        foreach(var tile in diningRoomTiles){
            tile.GetComponent<FloorPiece>().roomHighlight.SetActive(true);
        }
    }
    public void SetRoomDefBed(){
        HideRoomHighlights();
        definingRoomType = Room.RoomType.Bedroom;
        foreach(var tile in bedroomTiles){
            tile.GetComponent<FloorPiece>().roomHighlight.SetActive(true);
        }
    }
    private List<Vector3> AllRoomCenters(){
        List<Vector3> returnedCenters = new List<Vector3>();
        foreach(var floor in ri.allFloors){
            Vector3 center = floor.gameObject.GetComponent<Renderer>().bounds.center;
            returnedCenters.Add(center);
            // Debug.DrawRay(center, Vector3.up, Color.green, 10f);
        }
        return returnedCenters;
    }


    public IEnumerator FindRooms(GameObject hitFloor){
        List<GameObject> roomTiles = new List<GameObject>();
        roomTiles.Add(hitFloor);

        Vector3 firstHitCenter = hitFloor.GetComponent<Renderer>().bounds.center;
        firstHitCenter.y += 1;

        List<Vector3> checkDirs = new List<Vector3>();

        Vector3 northCheck = firstHitCenter;
        northCheck.z += 2.5f;
        Vector3 southCheck = firstHitCenter;
        southCheck.z -= 2.5f;
        Vector3 eastCheck = firstHitCenter;
        eastCheck.x += 2.5f;
        Vector3 westCheck = firstHitCenter;
        westCheck.x -= 2.5f;

        checkDirs.Add(northCheck);
        checkDirs.Add(southCheck);
        checkDirs.Add(eastCheck);
        checkDirs.Add(westCheck);


        List<GameObject> tilesToCheck = new List<GameObject>();
        foreach(var dir in checkDirs){
            RaycastHit firstWallCheck;
            // Debug.DrawRay(firstHitCenter, (dir - firstHitCenter), Color.green, 0.25f);
            if(!Physics.Raycast(firstHitCenter, (dir - firstHitCenter), out firstWallCheck, 2f)){
                RaycastHit downcast;

                // Debug.DrawRay(dir, Vector3.down, Color.magenta, 0.25f);
                if(Physics.Raycast(dir, Vector3.down, out downcast)){
                    if(downcast.transform.gameObject.GetComponent<FloorPiece>() != null){
                        if(!tilesToCheck.Contains(downcast.transform.gameObject)){
                            tilesToCheck.Add(downcast.transform.gameObject);
                            roomTiles.Add(downcast.transform.gameObject);
                        }
                    } 
                }
            }
        }

        while(tilesToCheck.ToList().Count > 0){
            foreach(var tile in tilesToCheck.ToList()){
                // yield return new WaitForSeconds(0.25f);
                Vector3 tileCenter = tile.GetComponent<Renderer>().bounds.center;
                tileCenter.y += 1;

                List<Vector3> tileCheckDirs = new List<Vector3>();

                Vector3 tileNorthCheck = tileCenter;
                tileNorthCheck.z += 2.5f;
                Vector3 tileSouthCheck = tileCenter;
                tileSouthCheck.z -= 2.5f;
                Vector3 tileEastCheck = tileCenter;
                tileEastCheck.x += 2.5f;
                Vector3 tileWestCheck = tileCenter;
                tileWestCheck.x -= 2.5f;

                tileCheckDirs.Add(tileNorthCheck);
                tileCheckDirs.Add(tileSouthCheck);
                tileCheckDirs.Add(tileEastCheck);
                tileCheckDirs.Add(tileWestCheck);

                foreach(var tileDir in tileCheckDirs){
                    RaycastHit tileCheck;
                    // Debug.DrawRay(tileCenter, (tileDir - tileCenter), Color.green, 0.25f);
                    if(!Physics.Raycast(tileCenter, (tileDir - tileCenter), out tileCheck, 2f)){
                        RaycastHit tileDown;

                        // Debug.DrawRay(tileDir, Vector3.down, Color.magenta, 0.25f);
                        if(Physics.Raycast(tileDir, Vector3.down, out tileDown)){
                            if(tileDown.transform.gameObject.GetComponent<FloorPiece>() != null){
                                if(!tilesToCheck.Contains(tileDown.transform.gameObject) && !roomTiles.Contains(tileDown.transform.gameObject)){
                                    tilesToCheck.Add(tileDown.transform.gameObject);
                                }
                                if(!roomTiles.Contains(tileDown.transform.gameObject)){
                                    roomTiles.Add(tileDown.transform.gameObject);
                                }
                            }
                        }
                    }
                }

                tilesToCheck.Remove(tile);

            }
            // print(tilesToCheck.ToList().Count());
            yield return new WaitForSeconds(0.002f);
        }

        if(definingRoomType == Room.RoomType.Kitchen){
           kitchenTiles = roomTiles;            
           SetRoomDefKitchen();     
        }
        if(definingRoomType == Room.RoomType.Dining){
           diningRoomTiles = roomTiles;            
           SetRoomDefDining();     
        }
        if(definingRoomType == Room.RoomType.Bedroom){
           bedroomTiles = roomTiles;            
           SetRoomDefBed();     
        }

        yield return null;
    }


}
