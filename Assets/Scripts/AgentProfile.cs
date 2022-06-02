using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentProfile : MonoBehaviour
{

	private string firstName;
	private string lastName;
	private string preferredName;
    public int gunNum;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setName(string fn, string ln, string pn)
    {
    	firstName = fn;
    	lastName = ln;
    	preferredName = pn;
    }
    public string getName()
    {
    	//Debug.Log(preferredName + " " + gunNum);
        return preferredName;
    }

    public void setWeapon(int gn)
    {
        gunNum = gn;
    }
    public int getWeapon()
    {
        return gunNum;
    }
}
