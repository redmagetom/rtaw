using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BaseInformation : MonoBehaviour
{
    
    public string objectName;
    public GameObject debugLabel;

    // Start is called before the first frame update
    void Start()
    {
        debugLabel.GetComponent<TextMeshPro>().text = objectName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
