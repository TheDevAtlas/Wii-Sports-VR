using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WiiU = UnityEngine.WiiU;

public class BowlingBallNo : MonoBehaviour {
	
	// 0 Is Left, 1 Is Right //
	public int channel;

	public GameObject bowlingBall;
	public Transform armHold;

	WiiU.Remote remote;
	WiiU.MotionPlus motionPlus;

	public float speed;
	Transform startTransform;

	void Start()
	{
		remote = WiiU.Remote.Access (0);
		motionPlus = remote.motionPlus;

		motionPlus.Enable (WiiU.MotionPlusMode.Standard);

		startTransform = bowlingBall.transform;
	}

	void Update()
	{
		WiiU.RemoteState state = remote.state;

		if(state.IsTriggered(WiiU.RemoteButton.A))
		{
			bowlingBall.transform.SetParent (armHold);
			bowlingBall.transform.rotation = startTransform.rotation;
			bowlingBall.transform.position = Vector3.zero;
			bowlingBall.GetComponent<Rigidbody> ().isKinematic = true;
		}

		if (state.IsReleased (WiiU.RemoteButton.B))
		{
			bowlingBall.transform.SetParent (null);

			Rigidbody rb = bowlingBall.GetComponent<Rigidbody> ();
			rb.isKinematic = false;

			rb.AddForce (transform.forward * speed);
		}
	}
}
