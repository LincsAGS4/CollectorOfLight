using UnityEngine;
using System.Collections;

public class EllekMoveScript : MobileObstacleMoveScript
{
    private float turnScale;         //Affects the rate at which the Ellek turns toward its intended destination angle (per update)

    private float speedDecayRate;    //The lambda in the exponential decay equation (may need tweaking, hence public)

    /*Assuming we want the powerups to last an amount of time, t:
    *
    *                     ( standardSpeed )
    * speedDecayRate = ln ( ------------- ) 
    *                     (  boostedSpeed ) 
    *                     -----------------
    *                            -t
    */
    
    private float powerupDuration;  //The duration of a powerup's effects, used in the above formula.

    public float rotationAngle;    //The angle the wheelchair is currently facing, and hence the angle the Ellek is turning towards
    private float maxGoalAngle;     //The maximum angle the Ellek can turn to, or even try to, currently set at 60 degrees / 1.047 radians

    public float maxSpeed;          /*The absolute maximum limit of the creature's speed (shouldn't be needed but the 
                                     * combination of upgrades + going downhill + speed powerup might need limiting)*/

    private bool boostActive;       /*Tells us whether the speed powerup is currently active or not (and hence whether 
                                     *the speed needs to be reduced)*/
    private float boostedSpeed;     //The speed the powerup changed the speed to (used for the exponential decay)
    private float lastBoostTime;    //The last time a speed powerup was collected
    private float timeSinceBoost;   //The time that has passed since the last speed powerup was collected

	// Use this for initialization
	void Start () 
    {
        maxSpeed = 0.20f;

        rotationAngle = 0.0f;
        maxGoalAngle = 1.047f;

        turnScale = (1.0f / 30.0f);

        boostActive = false;
        boostedSpeed = 0.15f;

        //Using the formula I derived for the decayrate, using the assumed speedboost.
        speedDecayRate = Mathf.Log((standardSpeed / boostedSpeed) / -powerupDuration);
	}
	
	protected override void SpecificMovement() 
    {
		//scale down the angle the player is turned at and use it to update the current angle
        float currentAngle = container.transform.eulerAngles.y + (rotationAngle * (turnScale) / (Mathf.PI / 180));
		container.transform.eulerAngles = new Vector3 (0, currentAngle, 0);

        //If speed has been changed, adjust speed towards standard speed using exponential decay
        //Speed at time t == maxSpeed * e ^ (speedDecayRate * t)
        /*if (boostActive)
        {
            timeSinceBoost = Time.time - lastBoostTime;

            currentSpeed = boostedSpeed * (Mathf.Exp(speedDecayRate * timeSinceBoost));

            //If speed has degraded sufficiently, stop degrading it.
            if (currentSpeed < standardSpeed)
            {
                currentSpeed = standardSpeed;
                boostActive = false;
            }
        }*/
	}

    //Attempt to change the angle of the Ellek, only accepts a range of +/- 60 degrees (max turning arc of the Ellek)
    public bool ChangeAngle(float newGoalAngle)
    {
        if (newGoalAngle > -1*maxGoalAngle && newGoalAngle < maxGoalAngle)
        {
            rotationAngle = newGoalAngle;
            return true;
        }
        else
        { return false; }
    }

    //Adjusts the current speed of the Ellek. Returns true if the desired addition leaves the speed under the maximum limit.
    public bool SpeedAdjust(float speedAddition)
    {
        if ((currentSpeed += speedAddition) < maxSpeed)
        {
            currentSpeed += speedAddition;
            boostedSpeed = currentSpeed;

            boostActive = true;

            return true;
        }
        else
        { return false; }
    }
}
