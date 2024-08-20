using UnityEngine;
using UnityEngine.WiiU;
using System.Collections;

public class BalanceTester : MonoBehaviour
{
    private float calibrationTime = 0.0f;
    private float[] zeroSamples = new float[4] { 0.0f, 0.0f, 0.0f, 0.0f };
    private float zeroSampleCount = 0;

    void  OnEnable()
    {
        calibrationTime = 0.0f;
        zeroSamples = new float[4] { 0.0f, 0.0f, 0.0f, 0.0f };
        zeroSampleCount = 0;
    }

    void Update()
    {
        RemoteState input = Remote.Access(3).state;

        if (calibrationTime < 2.0f)
        {
            if (!Remote.balanceBoardCalibrating)
            {
                for (int i = 0; i < 4; i++)
                    zeroSamples[i] += BalanceBoardState.wbcStatus.GetPress(i);// input.balanceBoard.GetPress(i);

                zeroSampleCount++;
                calibrationTime += Time.deltaTime;
            }

            if (calibrationTime < 2.0f)
            {
                transform.localScale = Vector3.one * (Mathf.Sin(calibrationTime * 8) * 0.2f + 1);
            }
            else
            {
                double[] avgs = new double[4];
                for (int i = 0; i < 4; i++)
                {
                    avgs[i] = zeroSamples[i] / zeroSampleCount;
                }
                Remote.SetBalanceBoardZeroValues(avgs);
                transform.localScale = Vector3.one;
            }
        }
        else
        {
            TestWeight(ref input.balanceBoard, 0, "Sensor 0");
            TestWeight(ref input.balanceBoard, 1, "Sensor 1");
            TestWeight(ref input.balanceBoard, 2, "Sensor 2");
            TestWeight(ref input.balanceBoard, 3, "Sensor 3");
        }
    }

    private void  TestWeight(ref BalanceBoardState input, int corner, string name)
    {
        float scale = (float)input.GetWeight(corner) * 0.1f;
        transform.Find(name).localScale = new Vector3(scale, scale, 1);
    }
}
