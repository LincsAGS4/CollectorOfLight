using UnityEngine;
using System.Collections;

public abstract class scrPoolable : MonoBehaviour
{
	public abstract void Init(params object[] initParams);
}
