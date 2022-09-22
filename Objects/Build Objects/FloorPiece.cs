using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPiece : MonoBehaviour
{
    public bool borderPiece;
    public Vector3 currentPos;
    
    public List<GameObject> placementOptions;
  
    public List<WallPiece> wallPieces;
  
    public List<GameObject> wallPlacements;

    public List<GameObject> userWalls;
    public List<GameObject> doors;
    public GameObject roomHighlight;

    public Material placeableHighlight;
    [Header("Audio For Build")]
    public AudioClip wallPlacementSound;
    public AudioClip floorPlacementSound;


  void Awake(){
    SetPlacements();
  }
  private void SetPlacements(){
    try{
      foreach(var place in wallPlacements){
        place.GetComponent<MeshRenderer>().material = placeableHighlight;
        place.AddComponent<WallPlacement>();
      }
    } catch{}
  }





  public void GetAvailableSpaces(){
      foreach(var option in placementOptions){
          RaycastHit hit;
          Vector3 above = option.GetComponent<Renderer>().bounds.center;
          Vector3 below = option.GetComponent<Renderer>().bounds.center;
          below.y -= 5f;
          above.y += 5f;
          Debug.DrawRay(above, below-above, Color.green, 10f);
          if(Physics.Raycast(above, below - above, out hit, LayerMask.GetMask("Floors"))){
            Debug.Log(hit.transform.name);
              if(hit.transform.gameObject.GetComponent<TerrainCollider>() != null){
                
                option.SetActive(true);
              }
          }
      }


      foreach(var wallPlacement in wallPlacements){
        wallPlacement.gameObject.AddComponent<WallPlacement>();
          RosettaStone rs = GameObject.Find("RosettaStone").GetComponent<RosettaStone>();
          if(!rs.ri.userPlacedWalls.Contains(wallPlacement)){
            wallPlacement.SetActive(false);
          }  
      }
  }


  public void GetPossibleWallPositions(){
      foreach(var option in wallPlacements){
          RaycastHit hit;
          Vector3 op = option.GetComponent<Renderer>().bounds.center;
          Vector3 startPoint = gameObject.GetComponent<Renderer>().bounds.center;
          startPoint.y += 2;
            Debug.DrawRay(startPoint, (op - startPoint), Color.green, 4.5f);
          if(!Physics.Raycast(startPoint, (op - startPoint), out hit, 4.5f, ~LayerMask.GetMask("Wall Placement Spot"))){ 
            option.SetActive(true);       
          }          
      }
  }



}
