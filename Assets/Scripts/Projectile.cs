using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

	public float speed = 20f;
	//public float damage = 10f;
	public int damageMin = 5;
	public int damageMax = 15;
	private float lt;
	public float lifetime = 10f;
	public float hitChance;
	public float critChance;

	public Rigidbody2D rb;
    public GameObject hitFX;

	private int targetLayer;
	private int shooterLayer;

	public bool enemyShot;
	private bool miss;


    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * speed;
        lt = lifetime;

        if (!enemyShot)
        {
        	shooterLayer = 16;
        }
        else
        {
        	shooterLayer = 13;
        }
        //miss = false;
    }

    void Update()
    {
    	lt -= 0.2f;
    	if (lt <= 0)
    	{
    		Destroy(gameObject);
    	}
    }

    public void HitChance(float hc)
    {
    	hitChance = hc;
    }
    public void CritChance(float cc)
    {
    	critChance = cc;
    }

    void OnTriggerEnter2D (Collider2D hitInfo)
    {
    	targetLayer = hitInfo.gameObject.layer;
    	if(targetLayer != shooterLayer && targetLayer != 8)
    	{
    		if (targetLayer == 18)
    		{
    			//Debug.Log("Hitting the floor!");
                Instantiate(hitFX, gameObject.transform.position, gameObject.transform.rotation);
    			Destroy(gameObject);
    		}
    		else
    		{
    			float chance = Random.value;
    			//* 0.85f
    			if(lt > lifetime*0.95f)
    			{
    				chance = chance * 0.85f;
    				float xVar = Random.Range(-0.5f, 0.5f);
    				float yVar = Random.Range(-0.5f, 0.5f);
    				GetComponent<AttackPopup>().PointBlank(new Vector3(transform.position.x+xVar, transform.position.y+yVar, 0));
    			}
	    		if(chance < hitChance)
	    		{
	    			int damage = (int) Random.Range(damageMin, damageMax);
	    			if(chance < critChance)
	    			{
	    				damage += damageMax;
	    				GetComponent<AttackPopup>().CriticalHit(damage, transform.position);
	    			}
	    			else
	    			{
	    				GetComponent<AttackPopup>().DamageHit(damage, transform.position);
	    			}
	    			Destructible target = hitInfo.GetComponent<Destructible>();
	    			if (target != null)
	            	{
	    				target.TakeDamage(damage);
	    			}
                    Instantiate(hitFX, gameObject.transform.position, gameObject.transform.rotation);
	    			Destroy(gameObject);
	    		}
	    		else
	    		{
	    			GetComponent<AttackPopup>().Miss(transform.position);

	    		}
    		}
    		
    	}
    }
}
