using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casing : MonoBehaviour
{
	private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
    	rb = GetComponent<Rigidbody2D>();
    	rb.velocity = RandomVector();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector2 RandomVector()
    {
    	float x = Random.Range(-3f, 3f);
    	float y = Random.Range(0, 3f);
    	return new Vector2(x, y);
    }
}
