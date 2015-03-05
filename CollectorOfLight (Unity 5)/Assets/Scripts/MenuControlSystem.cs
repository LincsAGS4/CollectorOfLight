using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using KinectForWheelchair;

public class MenuControlSystem : MonoBehaviour 
{
    //-2: Information Screen's Return to Menu
    public Button informationReturnButton;

    //-1: Loop back to 2

    // 0: Start Game
    public Button startGameButton;

    // 1: Upgrade Store
    public Button upgradeStoreButton;

    // 2: Information Screen
    public Button informationScreenButton;

    // 3: Loop back to 0

    public int selectedButton;

    //See table above, reference to end point
    private int finalButton;

    private float timeLastTriggered;
    public float timeToTrigger;
    public float scrollDelay;

    public float inputDelay;

    public InputController inputController;

	// Use this for initialization
	void Start () 
    {
        timeToTrigger = 0.0f;
        selectedButton = 0;
        finalButton = 2;
	}
	
	// Update is called once per frame
	void Update () 
    {      
        #region Keyboard Input
        if (Time.time > timeToTrigger)
        {
            if (Input.GetKey(KeyCode.A))
            {
                if (selectedButton != -2)
                { selectedButton -= 1; }
                timeToTrigger = Time.time + scrollDelay;

                if (selectedButton < 0)
                { selectedButton = finalButton; }
            }

            if (Input.GetKey(KeyCode.D))
            {
                if (selectedButton != -2)
                { selectedButton += 1; }
                timeToTrigger = Time.time + scrollDelay;

                if (selectedButton > finalButton)
                { selectedButton = 0; }
            }

            if (Input.GetKey(KeyCode.Space))
            {
                timeToTrigger = Time.time + scrollDelay;
                switch (selectedButton)
                {
                    case 0: startGameButton.onClick.Invoke();
                        break;
                    case 1: upgradeStoreButton.onClick.Invoke();
                        break;
                    case 2: informationScreenButton.onClick.Invoke();
                        selectedButton = -2;
                        break;
                    case -2: informationReturnButton.onClick.Invoke();
                        break;
                }
            }
        }

        inputDelay = Time.time;

        // Get the input info
        SeatedInfo inputInfo = this.inputController.InputInfo;

        #endregion

        #region Kinect Input
        if (inputInfo != null)
        {
            /*If player is turned far enough in one direction AND enough time has passed 
             *since the last time a menu option scrolled...*/
            if (inputInfo.Features.Angle > 45 && Time.time > timeToTrigger)
            {
                /*so long as we're not in the Information Screen, change the selected 
                 * t120 Seconds
                 * \button and increment the delay appropriately.*/
                if (selectedButton != -2)
                { selectedButton += 1; }
                timeToTrigger = Time.time + scrollDelay;

                if (selectedButton > finalButton)
                { selectedButton = 0; }
            }
            else if (inputInfo.Features.Angle > 45 && Time.time > timeToTrigger)
            {
                if (selectedButton != -2)
                { selectedButton -= 1; }
                timeToTrigger = Time.time + scrollDelay;

                if (selectedButton < 0)
                { selectedButton = finalButton; }
            }

            //this will need adjusting to be appropriately scaled when we have a kinect to test with
            if (inputInfo.Features.Position.y > 4)
            {
                switch (selectedButton)
                {
                    case 0: startGameButton.onClick.Invoke();
                        break;
                    case 1: upgradeStoreButton.onClick.Invoke();
                        break;
                    case 2: informationScreenButton.onClick.Invoke();
                        selectedButton = -2;
                        break;
                    case -2: informationReturnButton.onClick.Invoke();
                        break;
                }
            }
        }
        #endregion

        switch(selectedButton)
        {
            case 0:
                startGameButton.animator.Play("Highlighted");
                upgradeStoreButton.animator.Play("Disabled");
                informationScreenButton.animator.Play("Normal");
                break;
            case 1:
                startGameButton.animator.Play("Normal");
                upgradeStoreButton.animator.Play("Disabled");
                informationScreenButton.animator.Play("Normal");
                break;
            case 2:
                startGameButton.animator.Play("Normal");
                upgradeStoreButton.animator.Play("Disabled");
                informationScreenButton.animator.Play("Highlighted");
                
                break;
            case -2:
                informationReturnButton.animator.Play("Highlighted");
                break;
        }
	}
}
