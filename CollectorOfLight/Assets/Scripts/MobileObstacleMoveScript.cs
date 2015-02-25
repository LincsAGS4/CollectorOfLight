using UnityEngine;
using System.Collections;

public abstract class MobileObstacleMoveScript : MonoBehaviour 
{
    public GameObject container;	//The empty gameobject determining position & rotation of the base of the object

    public InputController inputController;

    protected float currentSpeed;   //The current speed the creature is moving at
    public float adjustedSpeed;     //The speed of the creature adjusted to account for slopes
    public float standardSpeed;     /*The default speed of the creature, that it will return to after its speed is
                                     *altered (This is determined by the upgrade system)*/
    
    public float verticalAngle;

    protected Time time;              //Used to track time for exponential decays

	// Use this for initialization
	void Start () 
    {
        currentSpeed = 0.1f;
        standardSpeed = 0.10f;
        
	}
	
	// Update is called once per frame
	void Update () 
    {
        SpecificMovement();

        //TODO: Replace this with Alex's normal method from terrain
        #region angleSpeedRaycasting - Raycast the central position of the ellek and find its angle off the ground using the perpendicular of the normal

        Vector3 raycastDirection = new Vector3(0, -1, 0);
        RaycastHit speedCheckCollision = new RaycastHit();

        //Create a layer mask to ignore the player's collisions by bitshifting the index of the playerCollider layer (8)
        int layerMask = 1 << 8;

        //In order to collide with ONLY the player, the bitmask must be inverted.
        layerMask = ~layerMask;

        Physics.Raycast(container.transform.position, raycastDirection, out speedCheckCollision, Mathf.Infinity, layerMask);
        Debug.DrawRay(container.transform.position, (speedCheckCollision.normal*5));

        //vertical angle is 90 degrees - the vertical component of the normals' angle
        /*float*/ verticalAngle = (Mathf.PI/2) - (Mathf.Asin(speedCheckCollision.normal.y / speedCheckCollision.normal.magnitude));
        
        #endregion
        
        //create an adjustedSpeed based on the angle of the normal
		if (verticalAngle >= 0) 
		{ adjustedSpeed = currentSpeed * (1 + (verticalAngle/60)); }
		else if (verticalAngle < 0) 
		{ adjustedSpeed = currentSpeed / (1 + (verticalAngle/60)); }
		else 
		{ adjustedSpeed = currentSpeed;	}

		Vector3 newPosition = new Vector3();

        //Move object along its current facing
        //Convert move speed to translation, translate Ellek along xz directions by its speed
		newPosition = container.transform.position;

		newPosition.x += adjustedSpeed * Mathf.Sin(container.transform.eulerAngles.y);
        newPosition.z += adjustedSpeed * Mathf.Cos(container.transform.eulerAngles.y);

        //TODO: Replace this with Alex's normal method from terrain
        #region yPosRaycasting - Raycast the centre of the ellek on the xz plane (after movement) to get a y position for the Ellek
		Vector3 raycastOrigin = newPosition;
		raycastOrigin.y += 100;
		RaycastHit collision = new RaycastHit();

        Physics.Raycast(raycastOrigin, raycastDirection, out collision, Mathf.Infinity, layerMask);
        #endregion

        //update the new position with the y position from raycasting
		newPosition.y = collision.point.y;
        //The position won't be changed from here on out, so it can now be pushed to the container
        container.transform.position = newPosition;

        //TODO: Replace this with Alex's normal method from terrain
        #region RotatingToNormal
        //raycast at the new position
		RaycastHit orientationRaycast = new RaycastHit();
        Physics.Raycast(container.transform.position, raycastDirection, out orientationRaycast, Mathf.Infinity, layerMask);

        //find the perpendicular of the normal (facingVector) and use it to get x and z rotations by resolving components
        Vector3 normalPerpFinder = new Vector3(-orientationRaycast.normal.z, 0, orientationRaycast.normal.x);
        Vector3 facingVector = 100 * Vector3.Cross(orientationRaycast.normal, normalPerpFinder);
        
        Vector2 xyPart = new Vector2(facingVector.x, facingVector.y);
        Vector2 yzPart = new Vector2(facingVector.z, facingVector.y);

        Vector3 newOrientation = container.transform.eulerAngles;

        newOrientation.z = Mathf.Asin(xyPart.y / xyPart.magnitude);
        newOrientation.x = Mathf.Asin(yzPart.y / yzPart.magnitude);

        container.transform.eulerAngles = newOrientation;
        #endregion
    }

    //to be overwritten by an inheriting class with the specific adjustments to direction and speed BEFORE adjustments
    protected abstract void SpecificMovement();
}
