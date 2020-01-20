using System;
using UnityEngine;

[Serializable]
public class FightData
{
    [SerializeField]
    public float BossAnimationSpeed = 1.0f;

    [SerializeField]
    private int[] PlayerNumbers;

    [SerializeField]
    private MoveSequence[] MoveSequences;

    private int _currentIndex = 0;

    public FightData(int arraySize = 0)
    {
        this.MoveSequences = new MoveSequence[arraySize];
        this.PlayerNumbers = new int[arraySize];
    }

    public void AddMoveSequence(int playerNumber, MoveSequence moveSequence)
    {
        if (_currentIndex < MoveSequences.Length)
        {
            this.MoveSequences[this._currentIndex] = moveSequence;
            this.PlayerNumbers[this._currentIndex] = playerNumber;
            this._currentIndex++;
        }
    }

    public MoveSequence GetMovesForPlayerNumber(int playerNumber)
    {
        for (int i = 0; i < this.PlayerNumbers.Length; i++)
            if (this.PlayerNumbers[i] == playerNumber)
                return this.MoveSequences[i];

        throw new ArgumentException($"There is no player with number {playerNumber}!");
    }
}
