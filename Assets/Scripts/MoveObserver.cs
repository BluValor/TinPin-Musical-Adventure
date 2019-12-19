using System;

public class MoveObserver
{
    public Action<MoveType> UpdateAction;

    public void UpdateObserver(MoveType move) => UpdateAction?.Invoke(move);
}
