using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{
    public Transform weapon;
	public Transform firePoint;
	//private bool projHit;
	private GameObject projectilePrefab;
	private GameObject casingPrefab;
    private GameObject fireFXPrefab;
	private int numFired;
	public Animator gunAnim;
	//private string gunName;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void launch(int gunNum, int numProj, float fireVary, float spreadVary, float hitChance, float critChance, float jamChance, bool crCasing)
    {
    	gunAnim.SetInteger("Weapon", gunNum);
    	string gunName = gunLib(gunNum);
    	string projPath = "Prefabs/Weapons/Bullets/" + gunName;
        projectilePrefab = Resources.Load<GameObject>(projPath);
    	projectilePrefab.GetComponent<Projectile>().HitChance(hitChance);
    	projectilePrefab.GetComponent<Projectile>().CritChance(critChance);
    	string casingPath = "Prefabs/Weapons/Casings/" + gunName;
    	casingPrefab = Resources.Load<GameObject>(casingPath);
        string fireFXPath = "Prefabs/Weapons/fireFX/" + gunName;
        fireFXPrefab = Resources.Load<GameObject>(fireFXPath);

        //firePoint.transform.position = firePoint.transform.right * fireX(gunNum);
        // 3.5 0.4

        if(Random.value < jamChance)
        {
    		Debug.Log("Gun jam!");
    		GetComponent<AgentController>().EndAttack();
        }
        else
        {
        	int cnt = 0;
        	numFired = 0;
        	while(cnt < numProj)
        	{
        		StartCoroutine(shotDelay(numProj, fireVary, spreadVary, crCasing));
        		cnt++;
        	}
        	
        }
    }

    IEnumerator shotDelay(int numProj, float delay, float spread, bool casing)
    {
    	float topDelay = delay *1.5f;
    	float bottomDelay = delay * 0.05f;
    	float xVary = Random.Range(-0.15f, 0.15f);
    	float yVary = Random.Range(-0.15f, 0.15f);
    	float topSpread = spread * 1.5f;
    	float bottomSpread = spread * -1.5f;
    	float spreadVal = Random.Range(topSpread, bottomSpread);
    	Vector3 newFirePos = new Vector3(firePoint.position.x+xVary, firePoint.position.y+yVary, 0);
    	//Vector3 newFireRot = new Vector3(0, 0, firePoint.rotation.z+spreadVal);
    	//Debug.Log(firePoint.rotation);
    	Vector3 newFireRot = firePoint.rotation.eulerAngles;
    	newFireRot = new Vector3(newFireRot.x, newFireRot.y, newFireRot.z+spreadVal);
    	//Debug.Log(newFireRot);

    	yield return new WaitForSeconds(Random.Range(bottomDelay, topDelay));
    	GameObject.Instantiate(projectilePrefab, newFirePos, Quaternion.Euler(newFireRot));
    	if (casing)
        {
            GameObject.Instantiate(casingPrefab, newFirePos, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
        }
        GameObject.Instantiate(fireFXPrefab, firePoint.position, Quaternion.Euler(newFireRot));
    	gunAnim.SetTrigger("Fire");
    	numFired++;
    	//Debug.Log(numFired + "/" + numProj);
    	if(numProj == numFired)
    	{
    		//Debug.Log("ending attack " + numFired + "/" + numProj);
    		GetComponent<AgentController>().EndAttack();
    	}
    }

    public void CasingBurst(int gunNum, int spawnNum)
    {
        string gunName = gunLib(gunNum);
        string casingPath = "Prefabs/Weapons/Casings/" + gunName;
        casingPrefab = Resources.Load<GameObject>(casingPath);
        StartCoroutine(CasingBurstTime(spawnNum));
    }

    IEnumerator CasingBurstTime(int spawnNum)
    {
        yield return new WaitForSeconds(0.2f);
        for(int i = 0; i < spawnNum; i++)
        {
            GameObject.Instantiate(casingPrefab, weapon.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
        }
    }


    public string gunLib(int n)
    {
    	switch(n)
    	{
    		case 1:
    			return "22Auto";
    			break;
            case 2:
                return "32Revolver";
                break;
    		default:
    			return "Null";
                break;
    	}
    }

    public float gunSize(int n)
    {
        switch(n)
        {
            case 1:
                return 1.3f;
                break;
            case 2:
                return 1f;
                break;
            default:
                return 1f;
                break;
        }
    }

    public string reloadType(int n)
    {
        // Fire Point forward position
        switch(n)
        {
            case 1:
                return "Pistol Clip";
                break;
            case 2:
                return "Revolver Cast";
                break;
            default:
                return "Null";
                break;
        }
    }
}
