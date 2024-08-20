using UnityEngine;
using UnityEngine.WiiU;
using System.Collections;

public class Probe : MonoBehaviour
{
    private RemoteDevType[] devTypes = new RemoteDevType[4];

    private string DeviceTypeToString(RemoteDevType type)
    {
        if (type == RemoteDevType.Core) return "Remote";
        if (type == RemoteDevType.Nunchuk) return "Remote with Nunchuck";
        if (type == RemoteDevType.Classic) return "Remote with Classic Controller";
        if (type == RemoteDevType.Balance) return "Balance Board";
        if (type == RemoteDevType.MotionPlus) return "Motion Plus";
        if (type == RemoteDevType.MotionPlusNunchuk) return "Motion Plus with Nunchuk";
        if (type == RemoteDevType.MotionPlusClassic) return "Motion Plus with Classic Controller";
        if (type == RemoteDevType.ProController) return "Pro Controller";
        if (type == RemoteDevType.NotFound) return "---";
        return "Unsupported";
    }

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            devTypes[i] = RemoteDevType.Unknown;
        }
    }

    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            RemoteDevType type = RemoteDevType.Unknown;
            Remote rem = Remote.Access(i);
            RemoteError err = rem.Probe(out type);
            if (err == RemoteError.None)
            {
                if (type == RemoteDevType.Core)
                {
                    rem.motionPlus.Enable(MotionPlusMode.Standard);
                }
                else if (type == RemoteDevType.MotionPlusNunchuk && rem.motionPlus.mode != MotionPlusMode.MotionPlusAndNunchuk)
                {
                    rem.motionPlus.Enable(MotionPlusMode.MotionPlusAndNunchuk);
                }
                else if (type == RemoteDevType.MotionPlusClassic && rem.motionPlus.mode != MotionPlusMode.MotionPlusAndClassic)
                {
                    rem.motionPlus.Enable(MotionPlusMode.MotionPlusAndClassic);
                }
                devTypes[i] = type;
            }
            else if (err == RemoteError.NoController)
            {
                devTypes[i] = RemoteDevType.NotFound;
            }
        }
    }

    void OnGUI()
    {
        string text = "";
        for (int i = 0; i < 4; i++)
        {
            text += i + ": " + DeviceTypeToString(devTypes[i]) + "\n";
        }
        GUI.Label(new Rect(6, 6, 150, 70), text);

        bool allow = Remote.allowBalanceBoard;
        if (GUI.Button(new Rect(150, 6, 150, 70), "Allow Balance Board\n" + (allow ? "(ENABLED)" : "(DISABLED)")))
        {
            Remote.allowBalanceBoard = !allow;
        }
    }
}
