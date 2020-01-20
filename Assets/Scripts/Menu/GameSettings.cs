using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettings
{
    private static int _playerNumber = 1;
    public static int PlayerNumber
    {
        get => _playerNumber;
        set
        {
            if (value < 1 || value > 2)
                throw new ArgumentException("Player number should be between 1 and 2!");
            else
                _playerNumber = value;
        }
    }
    
    public static float Volume
    {
        get => AudioListener.volume;
        set
        {
            if (value < 0.0f || value > 1.0f)
                throw new ArgumentException("Volume should be between 0 and 1!");
            else
                AudioListener.volume = value;
        }
    }
}
