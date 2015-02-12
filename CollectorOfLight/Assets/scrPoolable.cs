using UnityEngine;
using System.Collections;

public abstract class scrPoolable : MonoBehaviour
{
	public bool Expired { get; protected set; }

	public abstract void Init(params object[] initParams);
}
