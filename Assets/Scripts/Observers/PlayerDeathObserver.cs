using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathObserver
{
    public Action<Player> UpdateAction;

    public void UpdateObserver(Player player) => UpdateAction?.Invoke(player);
}
