using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerUnit : NetworkBehaviour {

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        //runs on all player units
        if(Input.GetKeyDown(KeyCode.Space))
        {
            this.transform.Translate(0, 1, 0);
        }
        
    }
}
