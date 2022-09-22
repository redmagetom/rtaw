using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public UIManager uim;
    public float camMoveSpeed;
    public float camRotateSpeed;
    public float scrollSpeed;
    public KeyCode moveForward;
    public KeyCode moveBackward;
    public KeyCode moveRight;
    public KeyCode moveLeft;



    private bool scrollable = true;
  
    void Update()
    {
        // dont do anything if panel open
        foreach(var panel in uim.allPanels){
            if(panel.activeSelf){return;}
        }
        UpdateCamera();
      
    }





    private void UpdateCamera(){
        if(Input.GetKey(moveForward)){
            gameObject.transform.Translate(new Vector3(0,0,camMoveSpeed * Time.deltaTime));
        }
        if(Input.GetKey(moveBackward)){
            gameObject.transform.Translate(new Vector3(0,0,-(camMoveSpeed * Time.deltaTime)));
        }
        if(Input.GetKey(moveRight)){
            gameObject.transform.Translate(new Vector3(camMoveSpeed * Time.deltaTime,0,0));
        }
        if(Input.GetKey(moveLeft)){
            gameObject.transform.Translate(new Vector3(-(camMoveSpeed * Time.deltaTime),0,0));
        }

        if(Input.GetMouseButton(2)){
            // print("clicking");
            transform.eulerAngles += camRotateSpeed * new Vector3(0, Input.GetAxis("Mouse X") * Time.deltaTime, 0);
        }
        


            
        if(!scrollable && transform.position.y > 1.2 && transform.position.y < 45){
            scrollable = true;
            }

        if(scrollable){
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            transform.Translate(0, -(scroll * scrollSpeed), 0, Space.Self);
        }
      
        if(transform.position.y < 1.2 && scrollable){
            scrollable = false;
            Vector3 restPos = new Vector3(transform.position.x, 1.21f, transform.position.z);
            transform.LeanMove(restPos, 0.5f);
        } 

        if(transform.position.y > 45 && scrollable){
            scrollable = false;
            Vector3 restPos = new Vector3(transform.position.x, 44.9f, transform.position.z);
            transform.LeanMove(restPos, 0.5f);
        }
    }



}


