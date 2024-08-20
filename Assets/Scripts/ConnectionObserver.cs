using UnityEngine;
using UnityEngine.WiiU;
using System.Collections;

public class ConnectionObserver : MonoBehaviour
{
    public int channel = 0;

    private GameObject balance;
    private GameObject classic;
    private GameObject nunchuk;
    private GameObject procon;
    private GameObject remote;
    private MotionPlusTester mplus;

    void Awake()
    {
        if (channel == 3) balance = transform.Find("Balance").gameObject;
        classic = transform.Find("Classic").gameObject;
        nunchuk = transform.Find("Nunchuk").gameObject;
        procon = transform.Find("Pro Controller").gameObject;
        remote = transform.Find("Remote").gameObject;
        mplus = remote.GetComponent<MotionPlusTester>();
    }

    void Update()
    {
        RemoteDevType type = RemoteDevType.Unknown;
        Remote rem = Remote.Access(channel);

        var status = rem.Probe(out type);
        if (status == RemoteError.None)
        {
            if (channel == 3) balance.SetActive(type == RemoteDevType.Balance);
            if (type == RemoteDevType.Core || type == RemoteDevType.MotionPlus)
            {
                classic.SetActive(false);
                nunchuk.SetActive(false);
                procon.SetActive(false);
                remote.SetActive(true);
            }
            else if (type == RemoteDevType.Nunchuk || type == RemoteDevType.MotionPlusNunchuk)
            {
                classic.SetActive(false);
                nunchuk.SetActive(true);
                procon.SetActive(false);
                remote.SetActive(true);
            }
            else if (type == RemoteDevType.Classic || type == RemoteDevType.MotionPlusClassic)
            {
                classic.SetActive(true);
                nunchuk.SetActive(false);
                procon.SetActive(false);
                remote.SetActive(true);
            }
            else if (type == RemoteDevType.ProController)
            {
                classic.SetActive(false);
                nunchuk.SetActive(false);
                procon.SetActive(true);
                remote.SetActive(false);
            }
            else
            {
                classic.SetActive(false);
                nunchuk.SetActive(false);
                procon.SetActive(false);
                remote.SetActive(false);
            }
            mplus.enabled = (rem.motionPlus.mode != MotionPlusMode.Off);
        }
        else if (status == RemoteError.NoController)
        {
            if (channel == 3) balance.SetActive(false);
            classic.SetActive(false);
            nunchuk.SetActive(false);
            procon.SetActive(false);
            remote.SetActive(false);
            mplus.enabled = false;
        }
    }
}
