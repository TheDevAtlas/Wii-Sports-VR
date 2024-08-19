using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using WiiU = UnityEngine.WiiU;

public class UseOLV : MonoBehaviour
{
    [DllImport("OLVPlugin")]
    private static extern int OLV_Init();

    [DllImport("OLVPlugin")]
    private static extern void OLV_PostText(string msg);

    [DllImport("OLVPlugin")]
    private static extern string OLV_GetLatestPostedText();

    [DllImport("OLVPlugin")]
    private static extern string OLV_FilterWord(string word);

    void Start()
    {
        int result = OLV_Init();

        print("OLV init result: " + result);

        if (result == 0)
        {
            Debug.Log("Filter: " + FilterText("First test"));
            Debug.Log("Filter: " + FilterText("Second badworde"));
            Debug.Log("Filter: " + FilterText("Third fuck"));
        }
        else
        {
            print("SCRIPT: unable to initialize OLV.");

            WiiU.ErrorViewerArg arg = new WiiU.ErrorViewerArg();
            arg.errorCode = result;
            arg.errorType = WiiU.ErrorViewerArg.ErrorType.Code;
            WiiU.ErrorViewer.Invoke(arg);
        }
    }

    void Update()
    {
        WiiU.GamePadState myGamePad = WiiU.GamePad.access.state;

        if (myGamePad.IsTriggered(WiiU.GamePadButton.A))
        {
            print("Upload");
            OLV_PostText("Hello Miiverse");
        }
        if (myGamePad.IsTriggered(WiiU.GamePadButton.B))
        {
            print("Download");
            string lastPostedText = OLV_GetLatestPostedText();
            print("last posted text: " + lastPostedText);
        }
    }

    public string FilterText(string text)
    {
        print("Checking: " + text);
        string lastPostedText = OLV_FilterWord(text);
        return lastPostedText;
    }
}
