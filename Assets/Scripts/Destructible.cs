using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{

	public float health;
    private float maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        checkDeath();
    }

    public void TakeDamage(float damage)
    {
    	health -= damage;
    }

    private void checkDeath()
    {
    	if (health <= 0f)
    	{
    		Destroy(gameObject);
    	}
    }
}
