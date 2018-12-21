using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

    private Puck m_Puck;

	// Use this for initialization
	void Start () {
        m_Puck = DataMgr.GetIntanstance().m_Pack;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
