using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 using Mirror;
 
 public class CameraController : NetworkBehaviour
 {
 
     
    void Update(){
       if (!isLocalPlayer)
        {
            gameObject.GetComponent<Camera>().enabled = false;
            gameObject.GetComponent<AudioListener>().enabled = false;
        }
    }
}
