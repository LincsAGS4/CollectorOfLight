using UnityEngine;
using System.Collections;

public abstract class scrPoolable : MonoBehaviour
{
	public bool Expired { get; protected set; }
	public bool DestroyOnExpire = false;

	public abstract void Init(params object[] initParams);

	protected virtual void ExpireWhenOutOfBounds()
	{
		Expired = !scrLandscape.Instance.Contains(transform.position);
	}

	protected virtual void Update()
	{
		ExpireWhenOutOfBounds();

		if (Expired && DestroyOnExpire)
			Destroy (gameObject);
	}
}
