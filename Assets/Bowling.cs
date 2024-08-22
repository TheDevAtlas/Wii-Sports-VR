using UnityEngine;
using WiiU = UnityEngine.WiiU;

public class Bowling : MonoBehaviour
{
	public int channel;
	public GameObject bowlingBall; // Reference to your bowling ball object
	private Rigidbody ballRigidbody;
	private bool isHoldingBall = true;
	public Transform hand;

	private Vector3 previousPosition;
	private float controllerSpeed;

	void Start()
	{
		ballRigidbody = bowlingBall.GetComponent<Rigidbody>();
	}

	void Update()
	{
		//WiiU.MotionPlusState data = WiiU.Remote.Access(channel).state.motionPlus;

		// Calculate the controller's speed
		controllerSpeed = (hand.position - previousPosition).magnitude / Time.deltaTime;
		previousPosition = hand.position;

		// Check if the A button is pressed
		if (WiiU.Remote.Access(channel).state.IsTriggered(WiiU.RemoteButton.A))
		{
			// Reset ball position and attach it to the hand
			ResetBallPosition();
			isHoldingBall = true;
		}
		else if (WiiU.Remote.Access(channel).state.IsReleased(WiiU.RemoteButton.A) && isHoldingBall)
		{
			// Release the ball and apply the velocity
			ReleaseBall();
			isHoldingBall = false;
		}
	}


	private void ResetBallPosition()
	{
		// Position the ball at the hand position and reset its velocity
		bowlingBall.transform.position = hand.position;
		ballRigidbody.velocity = Vector3.zero;
		ballRigidbody.angularVelocity = Vector3.zero;
		ballRigidbody.isKinematic = true; // Ensure the ball is kinematic while holding
		bowlingBall.transform.parent = hand.transform;
	}

	private void ReleaseBall()
	{
		// Apply velocity and release the ball
		ballRigidbody.isKinematic = false; // Make the ball non-kinematic so physics applies
		ballRigidbody.velocity = transform.forward * CalculateThrowingForce();

		bowlingBall.transform.parent = null;
	}

	private float CalculateThrowingForce()
	{
		// Calculate the throwing force based on the speed of the controller
		float baseForce = 10f; // Base force value
		float speedMultiplier = 2f; // Multiplier to adjust the impact of the controller speed
		return baseForce + (controllerSpeed * speedMultiplier);
	}
}
