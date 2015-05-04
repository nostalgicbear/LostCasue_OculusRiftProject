using UnityEngine;
using System.Collections;

public class vSyncController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60; //-1 sets it to unlimited
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
