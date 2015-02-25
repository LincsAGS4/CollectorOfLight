using UnityEngine;
using System.Collections;

using KinectNet20;
using KinectForWheelchair;

public class InputController : MonoBehaviour
{

	// Variables for output
	public KinectForWheelchair.SeatedInfo InputInfo;

	// Kinect stuff
	KinectSensor sensor;
	Skeleton[] skeletonData;

	// Input processor
	SeatedInfoProcessor inputProcessor;


	// Use this for initialization
	void Start ()
	{
		// Find a Kinect sensor
		KinectSensorCollection kinectSensors = KinectSensor.KinectSensors;
		if(kinectSensors.Count == 0)
		{
			this.sensor = null;
			throw new UnityException("Could not find a Kinect sensor.");
		}

		// Enable the skeleton stream
		this.sensor = kinectSensors[0];
		this.sensor.SkeletonStream.Enable();
		if(!this.sensor.SkeletonStream.IsEnabled)
            throw new UnityException("Sensor could not be enabled.");

		// Create the input processor
		this.inputProcessor = new SeatedInfoProcessor();
		//inputProcessor = new InputProcessor(this.sensor.CoordinateMapper, DepthImageFormat.Resolution320x240Fps30);
		this.InputInfo = null;

		Debug.Log("Hello");
		return;
	}

	void OnApplicationQuit()
	{
		// Dispose the Kinect sensor
		if(sensor != null)
		{
			sensor.Dispose();
			sensor = null;
		}
        
        Debug.Log ("Goodbye");
        return;
    }

	// Update is called once per frame
	void Update ()
	{
        if (this.sensor != null)
        {
            // Retrieve skeleton data
            using (SkeletonFrame frame = this.sensor.SkeletonStream.OpenNextFrame(0))
            {
                if (frame != null)
                {
                    // Allocate memory if needed
                    if (skeletonData == null || skeletonData.Length != frame.SkeletonArrayLength)
                    {
                        skeletonData = new Skeleton[frame.SkeletonArrayLength];
                    }

                    frame.CopySkeletonDataTo(skeletonData);
                }
            }


            if (skeletonData == null)
                return;

            // Compute seated infos
            SeatedInfo[] seatedInfos = this.inputProcessor.ComputeSeatedInfos(skeletonData);

            // Get seated skeleton
            int skeletonIndex = -1;
            for (int i = 0; i < skeletonData.Length; i++)
            {
                if (seatedInfos[i].Posture == Posture.Seated)
                    skeletonIndex = i;
            }

            // Compute seated info
            if (skeletonIndex != -1)
            {
                this.InputInfo = seatedInfos[skeletonIndex];
                Debug.Log("Tracking skeleton.");
            }

            /*
            if (this.InputInfo != null)
                Debug.Log ("Have info." + this.InputInfo.Posture);

            if (this.InputInfo != null && this.InputInfo.Features != null)
                Debug.Log ("Have seated info.");
            */
        }
		return;
	}

}
