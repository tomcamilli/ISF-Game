using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AttackPopup : MonoBehaviour
{

	[SerializeField] private Transform damagePopup;
	[SerializeField] private Transform missPopup;
	[SerializeField] private Transform critPopup;
    [SerializeField] private Transform pointBlankPopup;
	private TextMeshPro textMesh;
	private bool popUpActive;

	private void Awake()
	{
	}

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DamageHit(int damageAmount, Vector3 spawnPos)
    {
    	textMesh = damagePopup.GetComponent<TextMeshPro>();
    	textMesh.SetText(damageAmount.ToString());
    	Instantiate(damagePopup, spawnPos, Quaternion.identity);
    }
    public void Miss(Vector3 spawnPos)
    {
    	textMesh = missPopup.GetComponent<TextMeshPro>();
    	textMesh.SetText("MISS");
    	Instantiate(missPopup, spawnPos, Quaternion.identity);
    }
    public void CriticalHit(int damageAmount, Vector3 spawnPos)
    {
    	textMesh = critPopup.GetComponent<TextMeshPro>();
    	textMesh.SetText(damageAmount.ToString());
    	Instantiate(critPopup, spawnPos, Quaternion.identity);
    }
    public void PointBlank(Vector3 spawnPos)
    {
        textMesh = pointBlankPopup.GetComponent<TextMeshPro>();
        textMesh.SetText("POINT BLANK");
        Instantiate(pointBlankPopup, spawnPos, Quaternion.identity);
    }
}
