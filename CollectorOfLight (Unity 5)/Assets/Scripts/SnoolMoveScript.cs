using UnityEngine;
using System.Collections;

public class SnoolMoveScript : MobileObstacleMoveScript 
{
    //Piggums move SLOWLY in a set direction for a time, then change direction every now and then
    //They react to the player's presence by ceasing all movement
    private float directionChangeTime;
    public float directionChangeDelay;

    protected override void SpecificMovement()
    {
        if (Time.time > directionChangeTime)
        {

        }
    }
}
