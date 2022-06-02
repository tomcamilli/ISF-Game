using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionCamera : MonoBehaviour
{
	public Transform[] targets;
    private int activeNum = 0;

    public float smoothSpeed;
    public Vector3 offset;

    void Start()
    {
        initAgents();
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.2f);
        targets[activeNum].GetComponent<AgentController>().ToggleAgentActive();
    }

    void Update()
    {
        if (Input.GetKey("0") && GetComponent<Camera>().orthographicSize < 10)
        {
            GetComponent<Camera>().orthographicSize *= 1.05f;
        }
        if (Input.GetKey("9") && GetComponent<Camera>().orthographicSize > 3)
        {
            GetComponent<Camera>().orthographicSize *= 0.95f;
        }
        if (targets[activeNum].GetComponent<AgentController>().actionsUsed >= 2 || Input.GetKeyDown("p"))
        {
            targets[activeNum].GetComponent<AgentController>().actionsUsed = 0;
            switchAgent();
        }
        //targets[activeNum].GetComponent<AgentProfile>().getName();
    }

    void LateUpdate()
    {
    	Vector3 desiredPosition = targets[activeNum].position + offset;
    	Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

    	transform.position = smoothedPosition;

    	transform.LookAt(targets[activeNum]);
        //Debug.Log(transform.position.x);//.y += 1;
        //transform.position = new Vector3(transform.position.x, transform.position.y+1, transform.position.z);
    }

    private void switchAgent()
    {
        targets[activeNum].GetComponent<AgentController>().ToggleAgentActive();
        activeNum++;
        GetComponent<Camera>().orthographicSize = 6f;
            
        if (activeNum > targets.Length-1)
        {
            activeNum = 0;
        }
        targets[activeNum].GetComponent<AgentController>().ToggleAgentActive();
    }

    private void initAgents()
    {
        //foreach (Transform agent in targets)
        //{
        //    agent.GetComponent<AgentProfile>().setName();
        //}
        targets[0].GetComponent<AgentProfile>().setName("Thomas", "Camilli", "Tommy");
        targets[0].GetComponent<AgentProfile>().setWeapon(2);
        targets[1].GetComponent<AgentProfile>().setName("Alexander", "Viola", "AJ");
        targets[1].GetComponent<AgentProfile>().setWeapon(1);
        targets[2].GetComponent<AgentProfile>().setName("Kevin", "Klaskala", "Kevin");
        targets[2].GetComponent<AgentProfile>().setWeapon(1);
    }
}
