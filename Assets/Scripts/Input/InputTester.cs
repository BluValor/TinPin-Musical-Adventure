using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OpenTK.Input;

using UnityEngine;

public class InputTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // 0 - pad x (horizontal) // win x
    // 1 - pad y (vertical) // win y
    // 2 - przód-tył // win z
    // 3 - obrót w długości // win obrót z
    // 4 - obrót w pionie // win obrót x
    // 5 - obrót w szerokości // win obrót y
    // 6 - lewo-prawo // win slider
    // 7 - góra-dół // win dial

    // Update is called once per frame
    void Update()
    {
        //StringBuilder log = new StringBuilder("");
        //float input0 = -2.0f;
        var state = Joystick.GetState(1);
        if (state.IsConnected)
            CheckMoves(state);

        //if (state.IsConnected)
        //    for (int i = 3; i < 6; i++)
        //        log.Append(state.GetAxis(i) + " | ");

                //MonoBehaviour.print(log.Append(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond + " \n"));
    }

    float GetYaw(JoystickState state) => state.GetAxis(4);
    float GetPitch(JoystickState state) => state.GetAxis(5);
    float GetRoll(JoystickState state) => state.GetAxis(3);

    float GetXAcc(JoystickState state) => state.GetAxis(2);
    float GetYAcc(JoystickState state) => (-1) * state.GetAxis(6);
    float GetZAcc(JoystickState state) => state.GetAxis(7);

    long GetMilis() => DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

    long timeStamp;
    long timeThreshold = 150;

    float xAccTempSum;
    float yAccTempSum;
    float zAccTempSum;
    float accThreshold = 0.0f;

    void CheckMoves(JoystickState state)
    {
        float yaw = GetYaw(state);
        float pitch = GetPitch(state);
        float roll = GetRoll(state);

        float accelRealX = GetXAcc(state);
        float accelRealY = GetYAcc(state);
        float accelRealZ = GetZAcc(state);

        //print(accelRealX + " " + accelRealY + " " + accelRealZ);

        float MIN = 0.17f;

        if (Mathf.Abs(accelRealX) > MIN || Mathf.Abs(accelRealY) > MIN || Mathf.Abs(accelRealZ) > MIN)
        {
            float rollAdjustedX = accelRealX;
            float rollAdjustedY = (Mathf.Cos(roll) * accelRealY) - (Mathf.Sin(roll) * accelRealZ);
            float rollAdjustedZ = (Mathf.Sin(roll) * accelRealY) + (Mathf.Cos(roll) * accelRealZ);

            float realX = (Mathf.Cos(pitch) * rollAdjustedX) - (Mathf.Sin(pitch) * rollAdjustedZ);
            float realY = rollAdjustedY;
            float realZ = (Mathf.Sin(pitch) * rollAdjustedX) + (Mathf.Cos(pitch) * rollAdjustedZ);

            //float realX = accelRealX;
            //float realY = accelRealY;
            //float realZ = accelRealZ;


            long tmpTime = GetMilis();

            if (tmpTime - timeStamp > timeThreshold)
            {
                timeStamp = tmpTime;
                xAccTempSum = realX;
                yAccTempSum = realY;
                zAccTempSum = realZ;
            }
            else
            {
                bool reset = false;
                string message = "";

                if (Math.Abs(xAccTempSum) > Math.Abs(yAccTempSum) && Math.Abs(xAccTempSum) > Math.Abs(zAccTempSum))
                {
                    if (Mathf.Sign(xAccTempSum) != Mathf.Sign(realX) && Mathf.Abs(xAccTempSum) > accThreshold)
                    {
                        message = xAccTempSum > 0 ? "FORWARD\t" : "BACKWARD";
                        reset = true;
                    }
                }
                else if (Mathf.Abs(yAccTempSum) > Mathf.Abs(zAccTempSum))
                {
                    if (Mathf.Sign(yAccTempSum) != Mathf.Sign(realY) && Mathf.Abs(yAccTempSum) > accThreshold)
                    {
                        message = yAccTempSum > 0 ? "LEFT\t" : "RIGHT\t";
                        reset = true;
                    }
                }
                else
                {
                    if (Mathf.Sign(zAccTempSum) != Mathf.Sign(realZ) && Mathf.Abs(zAccTempSum) > accThreshold)
                    {
                        message = zAccTempSum > 0 ? "UP\t" : "DOWN\t";
                        reset = true;
                    }
                }

                if (reset)
                {
                    StringBuilder log = new StringBuilder("");
                    log.Append("================> > > ");
                    log.Append(message);
                    log.Append(" < < <================ ");
                    log.Append("X: " + this.xAccTempSum + " | ");
                    log.Append("Y: " + this.yAccTempSum + " | ");
                    log.Append("Z: " + this.zAccTempSum);
                    print(log.ToString());
                    xAccTempSum = 0;
                    yAccTempSum = 0;
                    zAccTempSum = 0;
                }
                else
                {
                    xAccTempSum += realX;
                    yAccTempSum += realY;
                    zAccTempSum += realZ;
                }
            }
        }

    }
}