using UnityEngine;
using System.Collections;

public class scrRamp : scrPoolable {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.GetComponentInParent<EllekMoveScript>() != null)
        {
            Debug.Log("Hitting");
            gameObject.collider.enabled = false;
            c.gameObject.GetComponentInParent<EllekMoveScript>().Jump(5);
            
        }
    }

    public override void Init(params object[] initParams)
    {
        Expired = false;

        float x = (float)initParams[0];
        float z = (float)initParams[1];
        transform.position = new Vector3(x, scrLandscape.Instance.GetHeight(x, z), z);
        transform.up = scrLandscape.Instance.GetNormal(x, z);

    }
}
