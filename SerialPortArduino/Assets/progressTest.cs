using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class progressTest : MonoBehaviour {
    public float progress;
    public float maxProgress = 2500.0f;
    public Image Bar;
    public GameObject g;
    int current_turns;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        current_turns = g.GetComponent<SerialPortExample>().turns;
    	Bar.fillAmount = (float) current_turns / maxProgress;
	}
}
