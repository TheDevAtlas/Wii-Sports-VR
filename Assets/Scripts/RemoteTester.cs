using UnityEngine;
using UnityEngine.WiiU;
using System.Collections;

public class RemoteTester : MonoBehaviour
{
    public int channel;

    void Update()
    {
        RemoteState input = Remote.Access(channel).state;

        TestButton(ref input, RemoteButton.Up, "D Pad/Up");
        TestButton(ref input, RemoteButton.Down, "D Pad/Down");
        TestButton(ref input, RemoteButton.Left, "D Pad/Left");
        TestButton(ref input, RemoteButton.Right, "D Pad/Right");

        TestButton(ref input, RemoteButton.One, "Button/1");
        TestButton(ref input, RemoteButton.Two, "Button/2");
        TestButton(ref input, RemoteButton.A, "Button/A");
        TestButton(ref input, RemoteButton.B, "Button/B");

        TestButton(ref input, RemoteButton.Minus, "Function/Minus");
        TestButton(ref input, RemoteButton.Plus, "Function/Plus");

        TestAcceleration(input.acc.x, "Accelerometer/X");
        TestAcceleration(input.acc.y, "Accelerometer/Y");
        TestAcceleration(input.acc.z, "Accelerometer/Z");
    }

    private void TestButton(ref RemoteState input, RemoteButton button, string name)
    {
        transform.Find(name).GetComponent<Renderer>().material.color = input.IsPressed(button) ? Color.red : Color.white;
    }

    private void TestAcceleration(float input, string name)
    {
        transform.Find(name).localScale = new Vector3(1.0f, 1.0f, input);
    }
}
