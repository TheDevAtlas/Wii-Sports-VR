using UnityEngine;
using UnityEngine.WiiU;
using System.Collections;

public class ClassicTester : MonoBehaviour
{
    public int channel;

    void Update()
    {
        RemoteState input = Remote.Access(channel).state;

        TestButton(ref input.classic, ClassicButton.X, "XYAB/X");
        TestButton(ref input.classic, ClassicButton.Y, "XYAB/Y");
        TestButton(ref input.classic, ClassicButton.A, "XYAB/A");
        TestButton(ref input.classic, ClassicButton.B, "XYAB/B");

        TestButton(ref input.classic, ClassicButton.L, "LR/L");
        TestButton(ref input.classic, ClassicButton.R, "LR/R");
        TestButton(ref input.classic, ClassicButton.ZL, "LR/ZL");
        TestButton(ref input.classic, ClassicButton.ZR, "LR/ZR");

        TestButton(ref input.classic, ClassicButton.Up, "D Pad/Up");
        TestButton(ref input.classic, ClassicButton.Down, "D Pad/Down");
        TestButton(ref input.classic, ClassicButton.Left, "D Pad/Left");
        TestButton(ref input.classic, ClassicButton.Right, "D Pad/Right");

        TestButton(ref input.classic, ClassicButton.Minus, "Function/Minus");
        TestButton(ref input.classic, ClassicButton.Plus, "Function/Plus");

        TestStick(input.classic.leftStick, "Left Stick");
        TestStick(input.classic.rightStick, "Right Stick");

        TestTrigger(input.classic.leftTrigger, "Left Stick/Bar");
        TestTrigger(input.classic.rightTrigger, "Right Stick/Bar");
    }

    private void TestButton(ref UnityEngine.WiiU.ClassicState cl, ClassicButton button, string name)
    {
        transform.Find(name).GetComponent<Renderer>().material.color = cl.IsPressed(button) ? Color.red : Color.white;
    }

    private void TestStick(Vector2 input, string name)
    {
        Transform target = transform.Find(name);
        target.localRotation = Quaternion.FromToRotation(Vector3.right, new Vector3(input.x, input.y, 0.0f));
        target.localScale = new Vector3(input.magnitude, 1.0f, 1.0f);
    }

    private void TestTrigger(float input, string name)
    {
        transform.Find(name).GetComponent<Renderer>().material.color = input > 0.0 ? Color.red : Color.white;
    }
}
