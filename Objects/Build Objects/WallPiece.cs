using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPiece : MonoBehaviour
{
    public GameObject checkPos;
    public WallType wallType;

    public void DetectNeed(){
        gameObject.SetActive(true);
        StartCoroutine(I_DectectNeeded());
    }

    void OnTriggerEnter(Collider col){
        if(col.gameObject.GetComponent<WallPlacement>() != null){
            col.gameObject.SetActive(false);
        }
    }
    private IEnumerator I_DectectNeeded(){
        // -- ALSO SHOWS WALLS
        RaycastHit hit;
        Vector3 above = checkPos.GetComponent<Renderer>().bounds.center;
        Vector3 below = checkPos.GetComponent<Renderer>().bounds.center;
        below.y -= 5f;
        above.y += 5f;
        
        if(Physics.Raycast(above, below - above, out hit)){
            if(hit.transform.gameObject.GetComponent<FloorPiece>() == null){
                var storedPos = gameObject.transform.position;
                Vector3 animStartPos = storedPos;
                animStartPos.y += 30;

                gameObject.transform.position = animStartPos;
                float animTime = 0.25f * (Random.Range(1, 10)*0.1f);
                
                LeanTween.move(gameObject, storedPos, animTime).setEaseOutQuint();
                var floorObject = gameObject.transform.parent.transform.parent.gameObject;
                FloorPiece fp = floorObject.GetComponent<FloorPiece>();
                // AudioSource aud = floorObject.GetComponent<AudioSource>();
                
                AudioSource s = gameObject.AddComponent<AudioSource>();
                s.pitch = 1 + Random.Range(-0.5f, 0.5f);
                yield return new WaitForSeconds(animTime);
                s.PlayOneShot(fp.wallPlacementSound);
                fp.borderPiece = true;
            } else {
                gameObject.SetActive(false);
            }

        }
    }
}
