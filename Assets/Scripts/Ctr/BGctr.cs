using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGctr : MonoBehaviour {
    public  Image image;
	// Use this for initialization
	public void initialization() {
        image.sprite = ValueSheet.BGsprite[0];
	}
	

}
