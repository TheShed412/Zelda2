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

	bool rising = false;

	//These are for linecasting which is explained later
	public Transform groundCheck;
	public Transform lineStart, lineEnd;
	public Transform groundLineStart, groundLineEnd;

	/*These are needed so I can access the colliders. 
	 Link has a shield that I have set as a trigger for reasons I will explain later.*/
	public GameObject Link;
	public GameObject Shield;
	public GameObject Sword;

	private BoxCollider2D linkBox;
	private BoxCollider2D shieldBox;
	private BoxCollider2D swordBox;


	void Awake (){
		/*These initialize the Game Objects above when Link shows up on screen*/
		linkBox = Link.GetComponent<BoxCollider2D>();
		shieldBox = Shield.GetComponent<BoxCollider2D>();
		swordBox = Sword.GetComponent<BoxCollider2D>();
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

		if((GetComponent<Rigidbody2D>().velocity.y)>0){
			rising = true;
		}//if
		else if ((GetComponent<Rigidbody2D>().velocity.y)<0){
			rising = false;
		}else{
			rising = false;
		}//else if

		if (movement == true) {
			move = Input.GetAxis ("Horizontal");
			GetComponent<Rigidbody2D>().velocity = new Vector2 (move * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);
		}//if
		if (movement == false) {
			GetComponent<Rigidbody2D>().velocity = new Vector2 (0, GetComponent<Rigidbody2D>().velocity.y);
			move = 0;
		}//if

		/*CROUCHING*/
		if (!crouch) {
				//rigidbody2D.velocity = new Vector2 (move * maxSpeed, rigidbody2D.velocity.y);
				//I set the colliders back to the original positions
				linkBox.size = new Vector2(0.14f, 0.3f);
				shieldBox.offset = new Vector2(0.04f, 0.06f);
                linkBox.offset = new Vector2(0.0f, 0.0f);
				movement = true;
		}

		if(crouch){
			movement = false;
			GetComponent<Rigidbody2D>().velocity = new Vector2 (0, GetComponent<Rigidbody2D>().velocity.y);
			move = 0;
		}//f

		if (rising) {
			//I change the size of link's collider and shift the shield collider down
			//I also set the horizontal movement to 0.
			linkBox.size = new Vector2(0.14f, 0.25f);
            linkBox.offset = new Vector2(0.0f, -0.02f);
			shieldBox.offset = new Vector2(0.04f, -0.09f);
			if (crouch && grounded && !attack){
				movement = false;
				GetComponent<Rigidbody2D>().velocity = new Vector2 (0, GetComponent<Rigidbody2D>().velocity.y);
				move = 0;
			}else{
				movement = true;
				move = Input.GetAxis ("Horizontal");
			}//else if
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

		if(Input.GetKeyDown (KeyCode.L) && grounded && !crouch){
			attack = true;
			attackOnce = Time.time;
		}			

		if (attack == true) {
			if (grounded)
				movement = false;
			if(!grounded){
				movement = true;
			}//if
		}//if
	
		if ((Time.time - attackOnce) >= attackTime) {
			attack = false;
			movement = true;
		}//if

	}//Update

	//Sends a ray out in front of link to detect foreground objects
	void Raycasting()
	{
		if(Physics2D.Linecast(lineStart.position, lineEnd.position,1<<LayerMask.NameToLayer ("Foreground"))){
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

	}//Flip

	public void SwordBoxAttack()
	{
		swordBox.offset = new Vector2(0.22f, 0.08f);
		swordBox.size = new Vector2(0.40f, 0.06f);
	}//SwordBox

	public void SwordBoxRetract()
	{
		swordBox.offset = new Vector2(-0.11f, 0f);
		swordBox.size = new Vector2(0.12f, 0.06f);
	}//SwordBoxRetract

	public void ColliderCrouch()
	{
		linkBox.size = new Vector2(0.14f, 0.25f);
		linkBox.offset = new Vector2(0.0f, -0.02f);
		shieldBox.offset = new Vector2(0.04f, -0.09f);
	}//Jumping shield  animation


}