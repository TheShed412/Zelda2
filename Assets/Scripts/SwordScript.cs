using UnityEngine;
using System.Collections;

public class SwordScript : MonoBehaviour {

	public BoxCollider2D Sword;
	public GameObject Link;

	int flip = LinkControllerScript.flip;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.gameObject.tag == "Block"){
			Destroy(col.gameObject);
			flip = LinkControllerScript.flip;
			Link.GetComponent<Rigidbody2D>().AddForce(new Vector2(200f*flip, 0f));
		}//if
		
	}//OnCollisionEnter
}
