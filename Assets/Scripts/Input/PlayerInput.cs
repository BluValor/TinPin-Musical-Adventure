using OpenTK.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;

public class PlayerInput
{
    private int _controllerNumber;

    private PlayerInputType? _input = null;

    public PlayerInputType? Input() 
    {
        lock (this)
        {
            PlayerInputType? result = this._input;
            this._input = null;
            return result;
        }
    }

    private void SetInput(PlayerInputType input)
    {
        lock (this)
        {
            this._input = input;
        }
    }
    
    public PlayerInput(int controllerNumber)
    {
        this._controllerNumber = controllerNumber;
        Thread t = new Thread(new ThreadStart(Update));
        t.Start();
    }

    public static MoveType TranslateInput(PlayerInputType i)
    {
        switch (i)
        {
            case PlayerInputType.Left: return MoveType.Left;
            case PlayerInputType.Right: return MoveType.Right;
            case PlayerInputType.Forward: return MoveType.Forward;
            case PlayerInputType.Backward: return MoveType.Backward;
            case PlayerInputType.Up: return MoveType.Up;
            default: return MoveType.Down;
        }
    }

    //// 0 - pad x (horizontal) // win x
    //// 1 - pad y (vertical) // win y
    //// 2 - przód-tył // win z
    //// 3 - obrót w długości // win obrót z
    //// 4 - obrót w pionie // win obrót x
    //// 5 - obrót w szerokości // win obrót y
    //// 6 - lewo-prawo // win slider
    //// 7 - góra-dół // win dial

    // 0 - pad x (horizontal) // win x
    // 1 - pad y (vertical) // win y
    // 2 - lewo-prawo // win z
    // 3 - obrót w szerokości // win obrót z // pitch
    // 4 - obrót w pionie // win obrót x // yaw
    // 5 - obrót w długości // win obrót y // roll -> (-1)
    // 6 - przód-tył // win slider -> (-1)
    // 7 - góra-dół // win dial

    // Update is called once per frame
    void Update()
    {
        //StringBuilder log = new StringBuilder("");
        //float input0 = -2.0f;
        while (true)
        {
            var state = Joystick.GetState(1);
            if (state.IsConnected)
                CheckMoves3(state);
            Thread.Sleep(5);
        }

        //if (state.IsConnected)
        //    for (int i = 3; i < 6; i++)
        //        log.Append(state.GetAxis(i) + " | ");

        //MonoBehaviour.print(log.Append(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond + " \n"));
    }

    double GetYaw(JoystickState state) => state.GetAxis(4) * Math.PI;
    double GetPitch(JoystickState state) => state.GetAxis(3) * Math.PI;
    double GetRoll(JoystickState state) => (-1) * state.GetAxis(5) * Math.PI;

    double GetLRAcc(JoystickState state) => state.GetAxis(2);
    double GetFBAcc(JoystickState state) => (-1) * state.GetAxis(6);
    double GetTBAcc(JoystickState state) => state.GetAxis(7);

    long GetMilis() => DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

    long timeStamp;
    long timeThreshold = 150;

    double lrAccTempSum;
    double fbAccTempSum;
    double tbAccTempSum;
    double accThreshold = 0.0f;

    void CheckMoves(JoystickState state)
    {
        double yaw = GetYaw(state);
        double pitch = GetPitch(state);
        double roll = GetRoll(state);

        double accelRealLR = GetLRAcc(state);
        double accelRealFB = GetFBAcc(state);
        double accelRealTB = GetTBAcc(state);

        float MIN = 0.17f;

        if (Math.Abs(accelRealLR) > MIN || Math.Abs(accelRealFB) > MIN || Math.Abs(accelRealTB) > MIN)
        {
            double rollAdjustedLR = (Math.Cos(roll) * accelRealLR) + (Math.Sin(roll) * accelRealTB);
            double rollAdjustedFB = accelRealFB;
            double rollAdjustedTB = ((-1) * Math.Sin(roll) * accelRealLR) + (Math.Cos(roll) * accelRealTB);

            //double pLR = accelRealLR;
            //double pFB = (Math.Cos(pitch) * accelRealFB) - (Math.Sin(pitch) * accelRealTB);
            //double pTB = (Math.Sin(pitch) * accelRealFB) + (Math.Cos(pitch) * accelRealTB);

            double realLR = rollAdjustedLR;
            double realFB = (Math.Cos(pitch) * rollAdjustedFB) - (Math.Sin(pitch) * rollAdjustedTB);
            double realTB = (Math.Sin(pitch) * rollAdjustedFB) + (Math.Cos(pitch) * rollAdjustedTB);

            //System.Threading.Thread.Sleep(50);
            //Console.WriteLine(
            //    accelRealLR.ToString().Substring(0, 10).PadLeft(10) + " | " +
            //    pLR.ToString().Substring(0, 10).PadLeft(10) + " ||| " + 
            //    accelRealFB.ToString().Substring(0, 10).PadLeft(10) + " | " +
            //    pFB.ToString().Substring(0, 10).PadLeft(10) + " ||| " + 
            //    accelRealTB.ToString().Substring(0, 10).PadLeft(10) + " | " +
            //    pTB.ToString().Substring(0, 10).PadLeft(10));

            //float realX = accelRealX;
            //float realY = accelRealY;
            //float realZ = accelRealZ;


            long tmpTime = GetMilis();

            if (tmpTime - timeStamp > timeThreshold)
            {
                timeStamp = tmpTime;
                lrAccTempSum = realLR;
                fbAccTempSum = realFB;
                tbAccTempSum = realTB;
            }
            else
            {
                bool reset = false;
                string message = "";

                if (Math.Abs(lrAccTempSum) > Math.Abs(fbAccTempSum) && Math.Abs(lrAccTempSum) > Math.Abs(tbAccTempSum))
                {
                    if (Math.Sign(lrAccTempSum) != Math.Sign(realLR) && Math.Abs(lrAccTempSum) > accThreshold)
                    {
                        message = lrAccTempSum > 0 ? "RIGHT\t" : "LEFT";
                        reset = true;
                    }
                }
                else if (Math.Abs(fbAccTempSum) > Math.Abs(tbAccTempSum))
                {
                    if (Math.Sign(fbAccTempSum) != Math.Sign(realFB) && Math.Abs(fbAccTempSum) > accThreshold)
                    {
                        message = fbAccTempSum > 0 ? "FORWARD\t" : "BACKWARD\t";
                        reset = true;
                    }
                }
                else
                {
                    if (Math.Sign(tbAccTempSum) != Math.Sign(realTB) && Math.Abs(tbAccTempSum) > accThreshold)
                    {
                        message = tbAccTempSum > 0 ? "UP\t" : "DOWN\t";
                        reset = true;
                    }
                }

                if (reset)
                {
                    StringBuilder log = new StringBuilder("");
                    log.Append("================> > > ");
                    log.Append(message);
                    log.Append(" < < <================ ");
                    log.Append("LR: " + this.lrAccTempSum + " | ");
                    log.Append("FB: " + this.fbAccTempSum + " | ");
                    log.Append("TB: " + this.tbAccTempSum);
                    Console.WriteLine(log.ToString());
                    lrAccTempSum = realLR;
                    fbAccTempSum = realFB;
                    tbAccTempSum = realTB;
                }
                else
                {
                    lrAccTempSum += realLR;
                    fbAccTempSum += realFB;
                    tbAccTempSum += realTB;
                }
            }
        }

    }

    int nextMoveLR = 0;
    long nextMoveLRTime = 0;
    int nextMoveFB = 0;
    long nextMoveFBTime = 0;
    int nextMoveTB = 0;
    long nextMoveTBTime = 0;

    void CheckMoves3(JoystickState state)
    {
        double yaw = GetYaw(state);
        double pitch = GetPitch(state);
        double roll = GetRoll(state);

        double accelRealLR = GetLRAcc(state);
        double accelRealFB = GetFBAcc(state);
        double accelRealTB = GetTBAcc(state);

        float MIN = 0.3f;


        double rollAdjustedLR = (Math.Cos(roll) * accelRealLR) + (Math.Sin(roll) * accelRealTB);
        double rollAdjustedFB = accelRealFB;
        double rollAdjustedTB = ((-1) * Math.Sin(roll) * accelRealLR) + (Math.Cos(roll) * accelRealTB);

        //double pLR = accelRealLR;
        //double pFB = (Math.Cos(pitch) * accelRealFB) - (Math.Sin(pitch) * accelRealTB);
        //double pTB = (Math.Sin(pitch) * accelRealFB) + (Math.Cos(pitch) * accelRealTB);

        double realLR = rollAdjustedLR;
        double realFB = (Math.Cos(pitch) * rollAdjustedFB) - (Math.Sin(pitch) * rollAdjustedTB);
        double realTB = (Math.Sin(pitch) * rollAdjustedFB) + (Math.Cos(pitch) * rollAdjustedTB);

        if (realLR > MIN)
        {
            if (nextMoveLR == -1)
            {
                SetInput(PlayerInputType.Left);
                nextMoveLR = 1;
            }
            else
            {
                nextMoveLR = 1;
                nextMoveLRTime = GetMilis();
            }
        }
        else if (realLR < -MIN)
        {
            if (nextMoveLR == 1)
            {
                SetInput(PlayerInputType.Right);
                nextMoveLR = -1;
            }
            else
            {
                nextMoveLR = -1;
                nextMoveLRTime = GetMilis();
            }
        }

        if (GetMilis() - nextMoveLRTime > 150)
        {
            nextMoveLR = 0;
        }

        if (realFB > MIN)
        {
            if (nextMoveFB == -1)
            {
                SetInput(PlayerInputType.Backward);
                nextMoveFB = 1;
            }
            else
            {
                nextMoveFB = 1;
                nextMoveFBTime = GetMilis();
            }
        }
        else if (realFB < -MIN)
        {
            if (nextMoveFB == 1)
            {
                SetInput(PlayerInputType.Forward);
                nextMoveFB = -1;
            }
            else
            {
                nextMoveFB = -1;
                nextMoveFBTime = GetMilis();
            }
        }

        if (GetMilis() - nextMoveFBTime > 150)
        {
            nextMoveFB = 0;
        }


        if (realTB > MIN)
        {
            if (nextMoveTB == -1)
            {
                SetInput(PlayerInputType.Down);
                nextMoveTB = 1;
            }
            else
            {
                nextMoveTB = 1;
                nextMoveTBTime = GetMilis();
            }
        }
        else if (realTB < -MIN)
        {
            if (nextMoveTB == 1)
            {
                SetInput(PlayerInputType.Up);
                nextMoveTB = -1;
            }
            else
            {
                nextMoveTB = -1;
                nextMoveTBTime = GetMilis();
            }
        }

        if (GetMilis() - nextMoveTBTime > 150)
        {
            nextMoveTB = 0;
        }
    }

    void CheckMoves2(JoystickState state)
    {
        double yaw = GetYaw(state);
        double pitch = GetPitch(state);
        double roll = GetRoll(state);

        double accelRealLR = GetLRAcc(state);
        double accelRealFB = GetFBAcc(state);
        double accelRealTB = GetTBAcc(state);

        //print(accelRealX + " " + accelRealY + " " + accelRealZ);

        float MIN = 0.15f;

        if (Math.Abs(accelRealLR) > MIN || Math.Abs(accelRealFB) > MIN || Math.Abs(accelRealTB) > MIN)
        {
            double rollAdjustedLR = accelRealLR;
            double rollAdjustedFB = (Math.Cos(roll) * accelRealFB) - (Math.Sin(roll) * accelRealTB);
            double rollAdjustedTB = (Math.Sin(roll) * accelRealFB) + (Math.Cos(roll) * accelRealTB);

            double realLR = (Math.Cos(pitch) * rollAdjustedLR) - (Math.Sin(pitch) * rollAdjustedTB);
            double realFB = rollAdjustedFB;
            double realTB = (Math.Sin(pitch) * rollAdjustedLR) + (Math.Cos(pitch) * rollAdjustedTB);

            long tmpTime = GetMilis();

            Console.WriteLine("~LR: " + realLR + " | ~FB: " + realFB + " | ~TB: " + realTB + " ||| LR: " + accelRealLR + " | FB: " + accelRealFB + " | TB: " + accelRealTB);

            //if (tmpTime - timeStamp > timeThreshold)
            //{
            //    timeStamp = tmpTime;
            //    xAccTempSum = realX;
            //    yAccTempSum = realY;
            //    zAccTempSum = realZ;
            //}
            //else
            //{
            //    bool reset = false;
            //    string message = "";
            //}
        }
    }

}
