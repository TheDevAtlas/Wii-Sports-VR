using UnityEngine;
using UnityEngine.WiiU;
using System.Collections;

public class NunchukTester : MonoBehaviour
{
    public int channel;

    void Update()
    {
        RemoteState inputRemote = Remote.Access(channel).state;

        TestButton(ref inputRemote, RemoteButton.NunchukC, "Button/C");
        TestButton(ref inputRemote, RemoteButton.NunchukZ, "Button/Z");

        TestStick(inputRemote.nunchuk.stick, "Stick");

        TestAcceleration(inputRemote.nunchuk.acceleration.x, "Accelerometer/X");
        TestAcceleration(inputRemote.nunchuk.acceleration.y, "Accelerometer/Y");
        TestAcceleration(inputRemote.nunchuk.acceleration.z, "Accelerometer/Z");
    }

    private void TestButton(ref RemoteState input, RemoteButton button, string name)
    {
        transform.Find(name).GetComponent<Renderer>().material.color = input.IsPressed(button) ? Color.red : Color.white;
    }

    private void TestStick(Vector2 input, string name)
    {
        Transform target = transform.Find(name);
        target.localRotation = Quaternion.FromToRotation(Vector3.right, new Vector3(input.x, input.y, 0.0f));
        target.localScale = new Vector3(input.magnitude, 1.0f, 1.0f);
    }

    private void TestAcceleration(float input, string name)
    {
        transform.Find(name).localScale = new Vector3(1, 1, input);
    }
}
