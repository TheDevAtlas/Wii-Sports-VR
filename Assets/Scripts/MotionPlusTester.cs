using UnityEngine;
using UnityEngine.WiiU;
using System.Collections;

public class MotionPlusTester : MonoBehaviour
{
    public int channel;

    void Update()
    {
        MotionPlusState data = Remote.Access(channel).state.motionPlus;

        var look = -data.dir.Y;
        var up = data.dir.Z;

        look.x *= -1;
        up.x *= -1;

        transform.localRotation = Quaternion.LookRotation(look, up);
    }
}
