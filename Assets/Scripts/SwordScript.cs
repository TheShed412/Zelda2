using UnityEngine;
using System.Collections;

public class SwordScript : MonoBehaviour {

	public BoxCollider2D Sword;
	public GameObject Link;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.gameObject.tag == "Block"){
			//Destroy(col.gameObject);
			print ("This shouldn't be here");
		}//if
		
	}//OnCollisionEnter
}
