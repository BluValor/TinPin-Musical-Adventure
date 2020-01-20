using System;

public class MoveObserver
{
    public Action<MoveType, int> UpdateAction;

    public void UpdateObserver(MoveType move, int playerNumber) => UpdateAction?.Invoke(move, playerNumber);
}
