using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AimIK;
using UnityEngine.Experimental.U2D.IK;

public class AgentController : MonoBehaviour
{

	public float speed;
	public float walkDistance;
	public GameObject walkMarker;
	public GameObject runMarker;
	public GameObject skeleton;
	public float scale;

	public GameObject weapon;
    private int itemNum;
    private string weapName;
	public GameObject clip;
	private Sprite clipArt;
	private GameObject clipPrefab;

	public GameObject firePoint;
	public Animator animator;
	[SerializeField] private List<LimbSolver2D> _solvers;

	private Vector3 mousePos;
	public GameObject mouseTarget;
	private float mouseAngle;
	Vector2 moveToPos;
	Vector2 lastAttackedPos;

	private bool agentActive;
	private bool ragdoll;
	private bool canWalk;
	private bool canRun;
	private bool attacking;
	private Vector3 curAngle;
	private Vector3 curScale;

	private bool handgun;
	public int actionsUsed;

	private State state;
    private enum State
    {
        Standing,
        Aiming,
        Reloading,
        Walking,
        Running,
        Crouching,
        CrouchAiming,
        CrouchReloading,
        CrouchWalking,
    }

    private enum CanMoveState
    {
    	CanWalk,
    	CanRun,
    	Moving,
    	Aiming,
    	Inactive,
    }
    private CanMoveState curCanMove; 

    void Start()
    {
        state = State.Standing;
        agentActive = false;
        curCanMove = CanMoveState.Inactive;
        ragdoll = false;
        clip.SetActive(false);
        actionsUsed = 0;
    }

    void Update()
    {
    	DirectionToMouse();
    	displayMovementMarker();
        
    	if (agentActive)
    	{
            itemNum = GetComponent<AgentProfile>().getWeapon();
            //Debug.Log("Hi! " + itemNum);
            string name = GetComponent<AgentProfile>().getName();
            //Debug.Log("My name is " + name);
	        switch (state) 
	        {
	        	case State.Standing:
	        		determineMovePos();
	        		Stand();
	        		break;
	        	case State.Aiming:
	        		Aim();
	        		break;
	        	case State.Reloading:
	        		Reload();
	        		break;
	        	case State.Walking:
	        		Walk();
	        		break;
	        	case State.Running:
	        		Run();
	        		break;
	        	case State.Crouching:
	        		determineMovePos();
	        		Crouch();
	        		break;
	        	case State.CrouchAiming:
	        		Aim();
	        		break;
	        	case State.CrouchWalking:
	        		CrouchWalk();
	        		break;
	        	case State.CrouchReloading:
	        		CrouchReload();
	        		break;
	        }
	        DetermineWeapon();
	        if (Input.GetKeyDown("8"))
    		{
    			ragdoll = !ragdoll;
    			skeleton.GetComponent<Ragdoll>().ToggleRagdoll(ragdoll);
    		}

	    }
	    
	    if(attacking)
	    {
	    	transform.eulerAngles = curAngle;
	    	transform.localScale = curScale;
	    	skeleton.transform.eulerAngles = new Vector3(0, 0, 0);
	    	if((lastAttackedPos.x-transform.position.x) < 0f)
    		{
    			skeleton.transform.localScale = new Vector3(-scale*curScale.x, scale*curScale.y, 1);
    		}
    		else
    		{
    			skeleton.transform.localScale = new Vector3(scale*curScale.x, scale*curScale.y, 1);
    		}
	    }
	    else
	    {
	    	curAngle= transform.eulerAngles;
	    	curScale = transform.localScale;
	    }
    }

    void LateUpdate()
    {
    	if(state == State.Aiming || state == State.CrouchAiming)
    	{
 			animator.enabled = false;
 			skeleton.GetComponent<AimIKBehaviour2D>().enabled = true;
 			foreach (var solver in _solvers)
    		{
    			solver.weight = 0;
    		}
    		clip.SetActive(false);
    	}
    	else
    	{
    		if (!ragdoll)
    		{
    			animator.enabled = true;
    			foreach (var solver in _solvers)
    			{
    				solver.weight = 1;
    			}
    		}
    		clip.SetActive(true);
    		skeleton.GetComponent<AimIKBehaviour2D>().enabled = false;
    	}

        if(itemNum > 0)
        {
            //Debug.Log(itemNum);
            weapName = GetComponent<LaunchProjectile>().gunLib(itemNum);
            float weapSize = GetComponent<LaunchProjectile>().gunSize(itemNum);
            string weapArtPath = "Art/Weapons/Guns/" + weapName;
            //Debug.Log(weapArtPath);
            weapon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(weapArtPath);
            weapon.transform.localScale = new Vector3(weapSize, weapSize, 1f);
        }
    }

    private void DirectionToMouse()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 lookDir = mousePos - transform.position;
        mouseAngle = Mathf.Atan2(lookDir.x, lookDir.y)*Mathf.Rad2Deg;
    }

    public void ToggleAgentActive()
    {
    	agentActive = !agentActive;
    	if (!agentActive)
    	{
    		curCanMove = CanMoveState.Inactive;
    	}
    }

    private void canMove(float moveDist)
    {
    	if (moveDist < walkDistance)
    	{
    		curCanMove = CanMoveState.CanWalk;
    	}
    	else if (moveDist < walkDistance*1.5f)
    	{
    		curCanMove = CanMoveState.CanRun;
    	}
    	else
    	{
    		curCanMove = CanMoveState.CanRun;
    		float moveMax = 0f;
    		if(moveToPos.x > 0f)
    		{
    			moveMax = transform.position.x+(walkDistance*1.5f);
    		}
    		else if (moveToPos.x < 0f)
    		{
    			moveMax = transform.position.x-(walkDistance*1.5f);
    		}
    		moveToPos = new Vector2(moveMax, transform.position.y);
    	}
    }

    private void determineMovePos()
    {
    	moveToPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    	moveToPos = new Vector2(moveToPos.x, transform.position.y);
    	float moveDist = Vector2.Distance((Vector2)transform.position, moveToPos);
    	canMove(moveDist);
    }

    private void displayMovementMarker()
    {
    	switch (curCanMove)
    	{
    		case CanMoveState.CanWalk:
    			walkMarker.SetActive(true);
    			runMarker.SetActive(false);
    			walkMarker.transform.position = moveToPos;
    			break;
    		case CanMoveState.CanRun:
    			walkMarker.SetActive(false);
    			runMarker.SetActive(true);
    			runMarker.transform.position = moveToPos;
    			break;
    		case CanMoveState.Moving:
    			walkMarker.SetActive(false);
    			runMarker.SetActive(false);
    			break;
    		case CanMoveState.Aiming:
    			walkMarker.SetActive(false);
    			runMarker.SetActive(false);
    			break;
    		case CanMoveState.Inactive:
    			walkMarker.SetActive(false);
    			runMarker.SetActive(false);
    			break;
    	}
    }

    private void Stand()
    {
    	animator.SetFloat("Speed", 0);
    	if (Input.GetMouseButtonDown(0))
    	{
    		if(curCanMove == CanMoveState.CanWalk)
    		{
    			//Debug.Log("Walking");
    			state = State.Walking;
    		}
    		else if (curCanMove == CanMoveState.CanRun)
    		{
    			//Debug.Log("Running");
    			state = State.Running;
    		}
    	}
    	if (Input.GetKeyDown("space"))
    	{
    		animator.SetBool("Crouching", true);
    		state = State.Crouching;
    	}
    	if (Input.GetKeyDown("o"))
    	{
    		animator.SetBool("Aiming", true);
    		state = State.Aiming;
    		curCanMove = CanMoveState.Aiming;
    	}
    	if (Input.GetKeyDown("r"))
    	{
    		state = State.Reloading;
    	}
    }

    private void Aim()
    {
    	Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    	Vector2 direction = mousePosition - transform.position;
    	float angle = Vector2.SignedAngle(Vector2.right, direction);
    	if(Mathf.Abs(angle) > 90f)
    	{
    		transform.localScale = new Vector3(-1f, 1f, 1f);
    		firePoint.transform.localScale = new Vector3(-0.5f, 0.5f, 1f);
    		skeleton.GetComponent<AimIKBehaviour2D>().chestParts[1].rotationOffset = -190;
    		skeleton.GetComponent<AimIKBehaviour2D>().chestParts[2].rotationOffset = -190;
    		skeleton.GetComponent<AimIKBehaviour2D>().chestParts[3].rotationOffset = -180;
    		skeleton.GetComponent<AimIKBehaviour2D>().chestParts[4].rotationOffset = -180;
    		skeleton.GetComponent<AimIKBehaviour2D>().chestParts[5].rotationOffset = -180;
    	}
    	else
    	{
    		transform.localScale = new Vector3(1f, 1f, 1f);
    		firePoint.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
    		skeleton.GetComponent<AimIKBehaviour2D>().chestParts[1].rotationOffset = -10;
    		skeleton.GetComponent<AimIKBehaviour2D>().chestParts[2].rotationOffset = -10;
    		skeleton.GetComponent<AimIKBehaviour2D>().chestParts[3].rotationOffset = 0;
    		skeleton.GetComponent<AimIKBehaviour2D>().chestParts[4].rotationOffset = 0;
    		skeleton.GetComponent<AimIKBehaviour2D>().chestParts[5].rotationOffset = 0;
    	}
    	if(!attacking)
    	{
    		mouseTarget.transform.position = mousePos;
    	}
    	firePoint.transform.rotation = weapon.transform.rotation;
    	firePoint.transform.position = weapon.transform.position;
    	if(Mathf.Abs(angle) > 90f)
    	{
    		firePoint.transform.Rotate(0f, 180f, 0f);
    	}
    	
    	if (Input.GetKeyDown("o"))
    	{
    		state = State.Standing;
    		animator.SetBool("Aiming", false);
    	}
    	if (Input.GetMouseButtonDown(0) && !attacking)
    	{
    		Attack();
    		StartCoroutine(WaitAttack(State.Standing));
    	}
    }

    private void Reload()
    {
    	clip.SetActive(true);
    	
    	string clipArtPath = "Art/Weapons/Clips/" + weapName;
    	clipArt = Resources.Load<Sprite>(clipArtPath);
    	string clipPrefabPath = "Prefabs/Weapons/Clips/" + weapName;
    	clipPrefab = Resources.Load<GameObject>(clipPrefabPath);

    	clip.GetComponent<SpriteRenderer>().sprite = clipArt;

    	animator.SetTrigger("Reload");
        switch(GetComponent<LaunchProjectile>().reloadType(itemNum))
        {
            case "Pistol Clip":
                animator.SetBool("Revolver", false);
                Instantiate(clipPrefab, weapon.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(-10.0f, 10.0f)));
                break;
            case "Revolver Cast":
                animator.SetBool("Revolver", true);
                GetComponent<LaunchProjectile>().CasingBurst(itemNum, 6);
                break;
            default:
                break;
        }

    	state = State.Standing;
    	actionsUsed++;
    	clip.SetActive(false);
    }

    private void Walk()
    {
    	if ((Vector2)transform.position != moveToPos)
    	{
    		animator.SetFloat("Speed", speed);
    		curCanMove = CanMoveState.Moving;

    		if((moveToPos.x-transform.position.x) < 0f)
    		{
    			transform.localScale = new Vector3(-1f, 1f, 1f);
    			transform.eulerAngles = new Vector3(0, 0, 0);
    		}
    		else if ((moveToPos.x - transform.position.x) > 0f)
    		{
    			transform.localScale = new Vector3(1f, 1f, 1f);
    		}
    		if(transform.localScale.y < 1)
    		{
    			transform.eulerAngles = new Vector3(0, 0, 0);
    		}

    		float step = speed * Time.deltaTime;
    		transform.position = Vector2.MoveTowards(transform.position, moveToPos, step); // lastClickedPos is second param
    	}
    	else
    	{
    		state = State.Standing;
    		StartCoroutine(WaitMove(1));
    	}
    }

    private void Run()
    {
    	animator.SetFloat("Speed", speed);
    	transform.eulerAngles = new Vector3(0, 0, 0);
    	if ((Vector2)transform.position != moveToPos)
    	{
    		curCanMove = CanMoveState.Moving;

    		if((moveToPos.x - transform.position.x) < 0f)
    		{
    			transform.localScale = new Vector3(-1f, 1f, 1f);
    			transform.eulerAngles = new Vector3(0, 0, 0);
    		}
    		else if ((moveToPos.x - transform.position.x) > 0f)
    		{
    			transform.localScale = new Vector3(1f, 1f, 1f);
    		}
    		if(transform.localScale.y < 1)
    		{
    			transform.eulerAngles = new Vector3(0, 0, 0);
    		}

    		float step = (speed*1.5f) * Time.deltaTime;
    		
    		transform.position = Vector2.MoveTowards(transform.position, moveToPos, step);
    	}
    	else
    	{
    		state = State.Standing;
    		StartCoroutine(WaitMove(2));
    	}
    }

    private void Crouch()
    {
    	animator.SetFloat("Speed", 0);
    	if (Input.GetMouseButtonDown(0))
    	{
    		if(curCanMove == CanMoveState.CanWalk)
    		{
    			//Debug.Log("Crouch-Walking");
    			state = State.CrouchWalking;
    		}
    		else if (curCanMove == CanMoveState.CanRun)
    		{
    			//Debug.Log("Running");
    			state = State.Running;
    		}
    	}
    	if (Input.GetKeyDown("space"))
    	{
    		animator.SetBool("Crouching", false);
    		state = State.Standing;
    	}
    	if (Input.GetKeyDown("o"))
    	{
    		animator.SetBool("Aiming", true);
    		state = State.CrouchAiming;
    		curCanMove = CanMoveState.Aiming;
    	}
    	if (Input.GetKeyDown("r"))
    	{
    		state = State.CrouchReloading;
    	}
    }

    private void CrouchAim()
    {
    	Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    	Vector2 direction = mousePosition - transform.position;
    	float angle = Vector2.SignedAngle(Vector2.right, direction);
    	if(Mathf.Abs(angle) > 90f)
    	{
    		transform.localScale = new Vector3(-1f, 1f, 1f);
    		firePoint.transform.localScale = new Vector3(-0.5f, 0.5f, 1f);
    		skeleton.GetComponent<AimIKBehaviour2D>().chestParts[1].rotationOffset = -190;
    		skeleton.GetComponent<AimIKBehaviour2D>().chestParts[2].rotationOffset = -300;
    		skeleton.GetComponent<AimIKBehaviour2D>().chestParts[3].rotationOffset = -180;
    		skeleton.GetComponent<AimIKBehaviour2D>().chestParts[4].rotationOffset = -290;
    		skeleton.GetComponent<AimIKBehaviour2D>().chestParts[5].rotationOffset = -180;
    	}
    	else
    	{
    		transform.localScale = new Vector3(1f, 1f, 1f);
    		firePoint.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
    		skeleton.GetComponent<AimIKBehaviour2D>().chestParts[1].rotationOffset = -10;
    		skeleton.GetComponent<AimIKBehaviour2D>().chestParts[2].rotationOffset = -10;
    		skeleton.GetComponent<AimIKBehaviour2D>().chestParts[3].rotationOffset = 0;
    		skeleton.GetComponent<AimIKBehaviour2D>().chestParts[4].rotationOffset = 0;
    		skeleton.GetComponent<AimIKBehaviour2D>().chestParts[5].rotationOffset = 0;
    	}
    	if(!attacking)
    	{
    		mouseTarget.transform.position = mousePos;
    	}
    	firePoint.transform.rotation = weapon.transform.rotation;
    	firePoint.transform.position = weapon.transform.position;
    	if(Mathf.Abs(angle) > 90f)
    	{
    		firePoint.transform.Rotate(0f, 180f, 0f);
    	}
    	//transform.eulerAngles = new Vector3(0, 0, angle);
    	if (Input.GetKeyDown("o"))
    	{
    		state = State.Crouching;
    		//transform.eulerAngles = new Vector3(0, 0, 0);
    		animator.SetBool("Aiming", false);

    	}
    	if (Input.GetMouseButtonDown(0) && !attacking)
    	{
    		Attack();
    		StartCoroutine(WaitAttack(State.Crouching));
    	}

    }

    private void CrouchReload()
    {
    	clip.SetActive(true);

    	//string weap = GetComponent<LaunchProjectile>().gunLib(weapNum);
    	string clipArtPath = "Art/Weapons/Clips/" + weapName;
    	clipArt = Resources.Load<Sprite>(clipArtPath);
    	string clipPrefabPath = "Prefabs/Weapons/Clips/" + weapName;
    	clipPrefab = Resources.Load<GameObject>(clipPrefabPath);

    	clip.GetComponent<SpriteRenderer>().sprite = clipArt;

    	//Debug.Log("Reloading!");
    	animator.SetTrigger("Reload");
        switch(GetComponent<LaunchProjectile>().reloadType(itemNum))
        {
            case "Pistol Clip":
                animator.SetBool("Revolver", false);
                Instantiate(clipPrefab, weapon.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(-10.0f, 10.0f)));
                break;
            case "Revolver Cast":
                animator.SetBool("Revolver", true);
                GetComponent<LaunchProjectile>().CasingBurst(itemNum, 6);
                break;
            default:
                break;
        }

    	state = State.Crouching;
    	actionsUsed++;
    	clip.SetActive(false);
    }

    private void CrouchWalk()
    {
    	if ((Vector2)transform.position != moveToPos)
    	{
    		animator.SetFloat("Speed", speed);
    		curCanMove = CanMoveState.Moving;

    		if((moveToPos.x - transform.position.x) < 0f)
    		{
    			transform.localScale = new Vector3(-1f, 1f, 1f);
    			transform.eulerAngles = new Vector3(0, 0, 0);
    		}
    		else if ((moveToPos.x - transform.position.x) > 0f)
    		{
    			transform.localScale = new Vector3(1f, 1f, 1f);
    		}
    		if(transform.localScale.y < 1)
    		{
    			transform.eulerAngles = new Vector3(0, 0, 0);
    		}

    		float step = (speed/1.3f) * Time.deltaTime;
    		transform.position = Vector2.MoveTowards(transform.position, moveToPos, step);
    	}
    	else
    	{
    		state = State.Crouching;
    		StartCoroutine(WaitMove(1));
    	}
    }

    IEnumerator WaitMove(int dist)
    {
    	yield return new WaitForSeconds(0.1f);
    	actionsUsed += dist;
    }

    IEnumerator WaitAttack(State returnState) 
    {
    	yield return new WaitUntil(() => attacking == false);
    	attacking = true;
    	yield return new WaitForSeconds(1f);
    	attacking = false;
    	animator.SetBool("Aiming", false);
    	transform.eulerAngles = new Vector3(0, 0, 0);
    	skeleton.transform.eulerAngles = new Vector3(0, 0, 0);
    	if((lastAttackedPos.x-transform.position.x) < 0f)
    	{
    		transform.localScale = new Vector3(-1f, 1f, 1f);
    		transform.eulerAngles = new Vector3(0, 0, 0);
    	}
    	actionsUsed++;
    	state = returnState;
    }

    private void DetermineWeapon()
    {
    	animator.SetBool("Handgun", true);
    }

    private void Attack()
    {
    	lastAttackedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    	attacking = true;
        //Debug.Log(skeleton.GetComponent<AimIKBehaviour2D>().chestParts[6].positionOffset.x);
    	switch(itemNum)
        {
            // Handguns
            case 1:
                // .22 Auto
                //skeleton.GetComponent<AimIKBehaviour2D>().chestParts[6].positionOffset.x = -6;
                GetComponent<LaunchProjectile>().launch(1, 2, 1f, 0.01f, 0.7f, 0.1f, 0.01f, true);
                break;
            case 2:
                // .32 Revolver
                //skeleton.GetComponent<AimIKBehaviour2D>().chestParts[6].positionOffset.x = -6;
                GetComponent<LaunchProjectile>().launch(2, 3, 1.5f, 0.01f, 0.7f, 0.1f, 0.01f, false);
                break;
            default:
                break;
        }

    	// Handguns
    	
    	//GetComponent<LaunchProjectile>().launch(0, 3, 0.5f, 0f, 0.7f, 0.1f, 0.01f); // Test
    	//GetComponent<LaunchProjectile>().launch(1, 3, 0.5f, 0f, 0.7f, 0.1f, 0.01f); // AR
    	//GetComponent<LaunchProjectile>().launch(1, 8, 0f, 5f, 0.7f, 0.1f, 0.01f); // Shotgun
    	//GetComponent<LaunchProjectile>().launch(1, 30, 2f, 0.01f, 0.7f, 0.1f, 0.01f); // Minigun
    }

    public void EndAttack()
    {
    	attacking = false;
    }
}
