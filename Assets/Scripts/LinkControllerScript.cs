using UnityEngine;
using System.Collections;

public class LinkControllerScript : MonoBehaviour {

	/*Make a second attack animation for jumping and walking*/

	Animator anim;//This creates a reference to the animator so it can access the variables 
	public LayerMask WhatIsGround;
	public PhysicsMaterial2D wallSlip;//Part of that fantastically bug I fixed
	
	float move = 0f;
	float groundRadius = 0.05f;
	float attackOnce = 0f;
	float attackTime = 0.767f;
	public float maxSpeed = 10f;
	public float jumpForce = 700f;
	public float gravity = -9.8f;

	/*Most of these are variables accessed by the Animator*/
	bool grounded = false;
	bool groundLine = false;
	bool facingRight = true;
	bool crouch = false;
	bool attack = false;
	bool movement = true;
	bool quickAttack = false;

	//These are for linecasting which is explained later
	public Transform groundCheck;
	public Transform lineStart, lineEnd;
	public Transform groundLineStart, groundLineEnd;

	/*These are needed so I can access the colliders. 
	 Link has a shield that I have set as a trigger for reasons I will explain later.*/
	public GameObject Link;
	public GameObject Shield;

	private BoxCollider2D linkBox;
	private BoxCollider2D shieldBox;


	void Awake (){
		/*These initialize the Game Objects above when Link shows up on screen*/
		linkBox = Link.GetComponent<BoxCollider2D>();
		shieldBox = Shield.GetComponent<BoxCollider2D>();
		}//Awake

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}//Start
	
	void FixedUpdate () 
	{
		/*This checks if Link is on the ground which is used by the jump and crouch animations*/
		grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, WhatIsGround);

		/*This checks if link is close to the ground to activate the crouch animation when Link lands.
		 It does a linecast which acts like a collider set as a trigger in a way.*/
		groundLine = Physics2D.Linecast(groundLineStart.position, groundLineEnd.position,1<<LayerMask.NameToLayer ("Foreground"));

		/*These are the variables the Animator accesses.*/
		anim.SetBool ("Ground", grounded);
		anim.SetFloat ("vSpeed", GetComponent<Rigidbody2D>().velocity.y);
		anim.SetFloat ("Speed", Mathf.Abs(move));
		anim.SetBool ("Crouch", crouch);
		anim.SetBool ("Close to the ground", groundLine);
		anim.SetBool ("Attack", attack);
		anim.SetBool ("Quick Attack", quickAttack);

		crouch = Input.GetButton ("Vertical");
		//move = Input.GetAxis ("Horizontal");

		if (movement == true) {
			GetComponent<Rigidbody2D>().velocity = new Vector2 (move * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);
			move = Input.GetAxis ("Horizontal");
		}

		if (movement == false) {
			GetComponent<Rigidbody2D>().velocity = new Vector2 (0, GetComponent<Rigidbody2D>().velocity.y);
			move = 0;
		}

		/*CROUCHING*/
		if (!crouch) {
				//rigidbody2D.velocity = new Vector2 (move * maxSpeed, rigidbody2D.velocity.y);
				//I set the colliders back to the original positions
				linkBox.size = new Vector2(0.14f, 0.3f);
				shieldBox.offset = new Vector2(0.04f, 0.06f);
				movement = true;
		}

		if (crouch) {
			//I change the size of link's collider and shift the shield collider down
			//I also set the horizontal movement to 0.
			GetComponent<Rigidbody2D>().velocity = new Vector2 (0, GetComponent<Rigidbody2D>().velocity.y);
			linkBox.size = new Vector2(0.14f, 0.25f);
			shieldBox.offset = new Vector2(0.04f, -0.09f);
			movement = false;
		}
		/*CROUCHING*/



		/*FANCY STUFF I DIDN'T FIGURE OUT ON MY OWN*/
		if (move > 0 && !facingRight)
						Flip();
		else if (move < 0 && facingRight)
						Flip();//Go to Flip()
		/*FANCY STUFF I DIDN'T FIGURE OUT ON MY OWN*/
	}//FixedUpdate

	// Update is called once per frame
	void Update()
	{
		if (Link.GetComponent<BoxCollider2D> () != null) {
						Raycasting ();//go to Raycasting()
		}

		/*The jumping controls. Probably need a little work. 
		 I don't like that they are only accesing K. I want
		 to use Input.GetButtonDown but I can't get that to work and it's
		 Sunday night. Eat a fish.*/
		if (grounded && Input.GetKeyDown (KeyCode.K)) 
		{
			anim.SetBool("Ground", false);
			GetComponent<Rigidbody2D>().AddForce( new Vector2(0.0f, jumpForce));
		}
		/*I am in a similar boat with this. It will neeed to be changed but I'm too busy
		 getting the animations not to suck. These last two ifs are for the attack btw*/

		if(Input.GetKeyDown (KeyCode.L)){
			attack = true;
			attackOnce = Time.time;
		}			

		if (attack == true) {
			if (grounded)
				movement = false;
			if(!grounded){
				movement = true;
			}
		}
	
		if ((Time.time - attackOnce) >= attackTime) {
						attack = false;
						movement = true;
				}

	}//Update

	//Sends a ray out in front of link to detect foreground objects
	void Raycasting()
	{
		/*THIS is that 8 hour bug fix I was dealing with last week! it looks simple but like, It was a bitch to figure out. 
		 Basically I am casting a line down the front of Link that, when it comes in contact with a collider in
		 the foreground layer, sets Links friction to 0 letting him just slide down the wall instead of getting stuck. 
		 */

		if(Physics2D.Linecast(lineStart.position, lineEnd.position,1<<LayerMask.NameToLayer ("Foreground")))
		{
			Link.GetComponent<Collider2D>().sharedMaterial = wallSlip;

		}//Foreground if
	}//Raycasting

	//Changes the scale on the screen to turn the other direction
	void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;

		/*This basically flips link so that 2 mirroed links didn't need to be drawn*/
	}//Flip


}
