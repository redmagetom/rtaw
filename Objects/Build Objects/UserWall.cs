using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserWall : MonoBehaviour
{
    public WallType wallType;
    void OnTriggerEnter(Collider col){
        if(col.gameObject.GetComponent<WallPlacement>() != null){
            col.gameObject.SetActive(false);
        }
    }
}
