using UnityEngine;
using System.Collections;

public class GGJPlayerControl : MonoBehaviour
{
	[HideInInspector]
	public bool facingRight = true;			// For determining which way the player is currently facing.
	[HideInInspector]
	public bool jump = false;				// Condition for whether the player should jump.

	public float moveForce = 365f;			// Amount of force added to move the player left and right.
	public float maxSpeed = 10f;			// The fastest the player can travel in the x axis.
	public AudioClip[] jumpClips;			// Array of clips for when the player jumps.
	public float jumpForce = 1000f;			// Amount of force added when the player jumps.
	public AudioClip[] taunts;				// Array of clips for when the player taunts.
	public float tauntProbability = 50f;	// Chance of a taunt happening.
	public float tauntDelay = 1f;			// Delay for when the taunt should happen.


	private int tauntIndex;					// The index of the taunts array indicating the most recent taunt.
	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	private bool grounded = false;			// Whether or not the player is grounded.
	private bool jumping = false;
	private bool combatting = false;
	private bool climbing = false;
	public bool ladder = false;
	private Animator anim;					// Reference to the player's animator component.
	private PlayerHealth playerHealth;
	//private Transform transform;

	void Awake()
	{
		// Setting up references.
		groundCheck = GameObject.FindGameObjectWithTag("Ground").GetComponent<Transform>();
		anim = GetComponent<Animator>();
		playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();

	}


	void Update()
	{
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		if (groundCheck == null)
			Debug.Log ("Null Pointer here!");
		grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));  



		// If the jump button is pressed and the player is grounded then the player should jump.
		if(Input.GetButtonDown("Jump") && grounded)
			jumping = true;

		// If the combat button is pressed and the player is grounded then the player should fight.
		if (Input.GetButtonDown("Combat") && grounded)
			combatting = true;

		// If the climbing button is pressed and the player is in front of a ladder then the player should climb.
		if (Input.GetButtonDown("Climbing") && ladder)
			climbing = true;

		// If the jumping parameter of the state machine was true set it to false;
		if(anim.GetBool("jumping"))
			anim.SetBool("jumping" , false);

		// If the climbing parameter of the state machine was true set it to false;
		if (anim.GetBool ("climbing"))
			anim.SetBool ("climbing", false);

		// If the combatting parameter of the state machine was true set it to false;
		if (anim.GetBool ("combatting"))
			anim.SetBool ("combatting" , false);
	}


	void FixedUpdate ()
	{
		// Cache the horizontal input.
		float h = Input.GetAxis("Horizontal");
		//Debug.Log (h);

		// The Speed animator parameter is set to the absolute value of the horizontal input.
		anim.SetFloat("h", Mathf.Abs(h));

		//Debug.Log(anim.GetFloat("h"));

		// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
		if(h * rigidbody2D.velocity.x < maxSpeed)
			// ... add a force to the player.
			rigidbody2D.AddForce(Vector2.right * h * moveForce);
		Debug.Log (Vector2.right);

		// If the player's horizontal velocity is greater than the maxSpeed...
		if(Mathf.Abs(rigidbody2D.velocity.x) > maxSpeed)
			// ... set the player's velocity to the maxSpeed in the x axis.
			rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);

		// If the input is moving the player right and the player is facing left...
		if (h > 0 && !facingRight) {
			// ... flip the player.
			Flip ();
		}
		// Otherwise if the input is moving the player left and the player is facing right...
		else if (h < 0 && facingRight) {
			// ... flip the player.
			Flip ();
		}
		// If the player should jump...
		if(jumping)
		{
			// Set the Jump animator trigger parameter.
			anim.SetTrigger("jumping");
			 
			// Play a random jump audio clip.
			int i = Random.Range(0, jumpClips.Length);
			//AudioSource.PlayClipAtPoint(jumpClips[i], transform.position);

			// Add a vertical force to the player.
			rigidbody2D.AddForce(new Vector2(0f, jumpForce));

			// Make sure the player can't jump again until the jump conditions from Update are satisfied.
			jumping = false;
		}

		if (climbing) {
			anim.SetTrigger("climbing");
			climbing = false;
		}

		if (combatting) {

			while (combatting) {
				anim.SetTrigger ("combatting");
			}

			combatting = false;
		}
	}
	
	
	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Adjust the sprite to face the correct direction.
		if (!facingRight)
			transform.right = new Vector3 (-1, 0, 0);
		else
			transform.right = new Vector3 (1, 0, 0);
	}


	public IEnumerator Taunt()
	{
		// Check the random chance of taunting.
		float tauntChance = Random.Range(0f, 100f);
		if(tauntChance > tauntProbability)
		{
			// Wait for tauntDelay number of seconds.
			yield return new WaitForSeconds(tauntDelay);

			// If there is no clip currently playing.
			if(!audio.isPlaying)
			{
				// Choose a random, but different taunt.
				tauntIndex = TauntRandom();

				// Play the new taunt.
				audio.clip = taunts[tauntIndex];
				audio.Play();
			}
		}
	}


	int TauntRandom()
	{
		// Choose a random index of the taunts array.
		int i = Random.Range(0, taunts.Length);

		// If it's the same as the previous taunt...
		if(i == tauntIndex)
			// ... try another random taunt.
			return TauntRandom();
		else
			// Otherwise return this index.
			return i;
	}
}
