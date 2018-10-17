using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageItem : MonoBehaviour {

	// Use this for initialization
	void Start () {
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);
        Color color = new Color(r, g, b);

        GetComponent<Image>().color = color;
        GetComponentInChildren<Text>().text = gameObject.name;
	}
	
}
