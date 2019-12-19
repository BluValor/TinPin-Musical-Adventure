using System;

[Serializable]
public struct MoveData
{
    public MoveType Move;
    public double Delay;

    public MoveData(MoveType move, double delay)
    {
        Move = move;
        Delay = delay;
    }
}
