using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Popup : MonoBehaviour
{
	public float shaky;
	public float speed;
	public float lifetime;
	private TextMeshPro t;
	private float lt;
	private Color textColor;
	private float disappearSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
    	t = GetComponent<TextMeshPro>();
        lt = lifetime;
        textColor = t.color;
    }

    // Update is called once per frame
    void Update()
    {
    	float shakyX = Random.Range(-shaky, shaky);
        transform.position += new Vector3(shakyX, speed) * Time.deltaTime;
        lt -= 0.2f;
        if (lt <= 0f)
        {
        	textColor.a -= disappearSpeed * Time.deltaTime;
        	t.color = textColor;
        	if (textColor.a < 0)
        	{
        		Destroy(gameObject);
        	}
        }
    }
}
