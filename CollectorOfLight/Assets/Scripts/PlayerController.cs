using UnityEngine;
using System.Collections;

using KinectForWheelchair;

public class PlayerController : MonoBehaviour
{
    public InputController inputController;
    public EllekMoveScript ellekSystem;

	// Update is called once per frame
    void Update()
	{       
        // Get the input info
		SeatedInfo inputInfo = this.inputController.InputInfo;
		if (inputInfo == null)
			return;

		// Set the player position and direction
		if (inputInfo.Features == null)
			return;

		//Debug.Log (inputInfo.Features.Position);
        if (ellekSystem.ChangeAngle(Mathf.Tan(inputInfo.Features.Position.x / inputInfo.Features.Position.y)) == false)
        {
            Debug.Log("Turning did not register; player rotated too far.");
        }
        
		return;
	}
}
