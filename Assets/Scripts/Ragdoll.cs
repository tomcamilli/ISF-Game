using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.IK;

public class Ragdoll : MonoBehaviour
{

	[SerializeField] private Animator _anim;
	[SerializeField] private List<Collider2D> _colliders;
    [SerializeField] private List<Collider2D> _offColliders;
	[SerializeField] private List<HingeJoint2D> _joints;
	[SerializeField] private List<Rigidbody2D> _rbs;
	[SerializeField] private List<LimbSolver2D> _solvers;
    public Rigidbody2D torsoRb;
    public Rigidbody2D leftLegRb;
    public Rigidbody2D rightLegRb;
	public GameObject weapon;
    public GameObject clip;
    private bool forceOn;

    // Start is called before the first frame update
    void Start()
    {
        ToggleRagdoll(false);
        forceOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleRagdoll(bool ragdollOn)
    {
    	_anim.enabled = !ragdollOn;
        foreach (var col in _offColliders)
        {
            col.enabled = !ragdollOn;
        }
    	foreach (var col in _colliders)
    	{
    		col.enabled = ragdollOn;
    	}
    	foreach (var joint in _joints)
    	{
    		joint.enabled = ragdollOn;
    	}
    	foreach (var rb in _rbs)
    	{
    		rb.simulated = ragdollOn;
    	}
    	weapon.SetActive(!ragdollOn);
        clip.SetActive(!ragdollOn);
    	foreach (var solver in _solvers)
    	{
    		solver.weight = ragdollOn ? 0 : 1;
    	}
        if(ragdollOn && !forceOn)
        {
            float dir = -1f;
            forceOn = true;
            //StartCoroutine(hitForce());
            torsoRb.velocity = transform.right * dir * (Random.Range(25f, 45f));
            leftLegRb.velocity = transform.right * -1f * dir * (Random.Range(2f, 13f));
            rightLegRb.velocity = transform.right * -1f * dir * (Random.Range(2f, 13f));
        }

        // if ragdoll on, apply directional force depending on the direction of the killing blow

    }

    /*IEnumerator hitForce()
    {
        forceOn = true
        yield return new WaitForSeconds(0.2f);
        forceOn = false
    }*/
}
