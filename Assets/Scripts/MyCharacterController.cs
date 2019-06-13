using UnityEngine;
using System.Collections;

public class MyCharacterController : MonoBehaviour {


	public float rotatespeed;
	public float speed;
	public float jumpspeed;

	private bool walking;
	private bool running;
	private bool actioning;
	private bool moving;
	private bool idle;
	private int modifier;
	private bool onground;
	private int powerups;
	private float speedmult;

	// Use this for initialization
	void Start () {
		GetComponent<Animation>().GetClip ("Idle").wrapMode = WrapMode.Loop;
		GetComponent<Animation>().GetClip ("Walk").wrapMode = WrapMode.Loop;
		GetComponent<Animation>().GetClip ("Action").wrapMode = WrapMode.Once;
	
		modifier = 1;

		actioning = false;
		onground = true;
		powerups = 0;

		GetComponent<Rigidbody>().freezeRotation = true;
	}

	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.R)){
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

		float horizontal = Input.GetAxis ("Horizontal");
		float vertical = Input.GetAxis ("Vertical");

		onground = Mathf.Abs (GetComponent<Rigidbody>().velocity.y) < 0.1;

		moving = (Mathf.Abs (vertical) > 0.5f);

		if(!moving)
		{
			running = false;
			walking = false;
			idle = true;
		}

		bool shift = Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift);

		if(moving){
			idle = false;
			if(shift){
				walking = false;
				running = true;
			} else {
				walking = true;
				running = false;
			}
		}
		
		if(Input.GetKey (KeyCode.E))
		{
			GetComponent<Animation>().CrossFade ("Action", 0.1f);
		}

		if(Input.GetKey(KeyCode.Space) && !actioning && onground){
			Vector3 currvel = GetComponent<Rigidbody>().velocity;
			currvel.y = jumpspeed;
			GetComponent<Rigidbody>().velocity = currvel;
			onground = false;
			//animation.CrossFade("Jump", 0.1f);
		}
		
		actioning = GetComponent<Animation>().IsPlaying("Action");

		speedmult = 1;

		if(!actioning && walking)
		{
			modifier = 1;
			speedmult = modifier * Mathf.Pow (1.25f, powerups);
			GetComponent<Animation>()["Walk"].speed = speedmult;
			GetComponent<Animation>().CrossFade ("Walk", 0.5f);
		}

		if(!actioning && running)
		{
			modifier = 2;
			speedmult = modifier * Mathf.Pow (1.25f, powerups);
			GetComponent<Animation>()["Walk"].speed = speedmult;
			GetComponent<Animation>().CrossFade ("Walk", 0.5f);
		}

		if(!actioning && idle){
			GetComponent<Animation>().CrossFade ("Idle", 0.5f);
		}

		Vector3 rotation = new Vector3 (0f, horizontal * rotatespeed * modifier * Time.deltaTime, 0f);
		transform.Rotate (rotation);

		float angle = transform.rotation.eulerAngles.y;
		float movex = speedmult * speed * vertical * Mathf.Sin (angle * Mathf.Deg2Rad) * Time.deltaTime;
		float movez = speedmult * speed * vertical * Mathf.Cos (angle * Mathf.Deg2Rad) * Time.deltaTime;

		Vector3 currentpos = transform.position;

		transform.position = new Vector3 (currentpos.x + movex, currentpos.y, currentpos.z + movez);

		printStatus ();

	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag.Equals ("Good")){
			powerups++;
			other.gameObject.SetActive(false);
		}

		if(other.gameObject.tag.Equals("Bad")){
			powerups--;
			other.gameObject.SetActive(false);
		}
	}

	void printStatus(){

		//print("Walking:" + walking + "\tRunning:" + running + "\tIdle:"+ idle + "\tActioning:"+ actioning + "\tJumping:" + jumping);
		//print ("On Ground:" + onground);
	}
}
