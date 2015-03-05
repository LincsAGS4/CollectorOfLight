using UnityEngine;
using System.Collections;

public class scrFruit : scrPoolable {

    public override void Init(params object[] initParams)
    {
        Expired = false;

        float x = (float)initParams[0];
        float z = (float)initParams[1];
        transform.position = new Vector3(x, scrLandscape.Instance.GetHeight(x, z) + 5, z);
    }
}
