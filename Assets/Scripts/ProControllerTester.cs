using UnityEngine;
using UnityEngine.WiiU;
using System.Collections;

public class ProControllerTester : MonoBehaviour
{
    public int channel;

    void Update()
    {
        ProControllerState input = Remote.Access(channel).state.pro;

        TestButton(ref input, ProControllerButton.X, "XYAB/X");
        TestButton(ref input, ProControllerButton.Y, "XYAB/Y");
        TestButton(ref input, ProControllerButton.A, "XYAB/A");
        TestButton(ref input, ProControllerButton.B, "XYAB/B");

        TestButton(ref input, ProControllerButton.L, "LR/L");
        TestButton(ref input, ProControllerButton.R, "LR/R");
        TestButton(ref input, ProControllerButton.ZL, "LR/ZL");
        TestButton(ref input, ProControllerButton.ZR, "LR/ZR");

        TestButton(ref input, ProControllerButton.Up, "D Pad/Up");
        TestButton(ref input, ProControllerButton.Down, "D Pad/Down");
        TestButton(ref input, ProControllerButton.Left, "D Pad/Left");
        TestButton(ref input, ProControllerButton.Right, "D Pad/Right");

        TestButton(ref input, ProControllerButton.Minus, "Function/Minus");
        TestButton(ref input, ProControllerButton.Plus, "Function/Plus");

        TestButton(ref input, ProControllerButton.StickL, "Left Stick/Bar");
        TestButton(ref input, ProControllerButton.StickR, "Right Stick/Bar");

        TestStick(input.leftStick, "Left Stick");
        TestStick(input.rightStick, "Right Stick");
    }

    private void TestButton(ref ProControllerState input, ProControllerButton button, string name)
    {
        transform.Find(name).GetComponent<Renderer>().material.color = input.IsPressed(button) ? Color.red : Color.white;
    }

    private void TestStick(Vector2 input, string name)
    {
        Transform target = transform.Find(name);
        target.localRotation = Quaternion.FromToRotation(Vector3.right, new Vector3(input.x, input.y, 0.0f));
        target.localScale = new Vector3(input.magnitude, 1.0f, 1.0f);
    }
}
