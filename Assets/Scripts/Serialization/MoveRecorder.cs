using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MoveRecorder : MonoBehaviour
{
    public MoveObserver MovePerformedObserver = new MoveObserver();
    public double RecordingStartTime;
    public float MoveSpeed;

    private LinkedList<Tuple<int, MoveData>> _moveHistory = new LinkedList<Tuple<int, MoveData>>();

    private string _saveFilePath;
    public string SaveFilePath
    {
        get => _saveFilePath == null ? null : _saveFilePath + "/record_" + DateTime.Now.ToString().Replace(' ', '_').Replace('.', '-').Replace(':', '-') + ".txt";
        set => _saveFilePath = value;
    }
    
    void Start()
    {
        MovePerformedObserver.UpdateAction = SaveMove;
    }

    void SaveMove(MoveType move, int playerNumber)
    {
        _moveHistory.AddLast(
            new Tuple<int, MoveData>(
                playerNumber,
                new MoveData(move, AudioSettings.dspTime - RecordingStartTime)));
    }

    private void OnDestroy()
    {
        //if (SaveFilePath != null)
        //{
        //    //var data = JsonArrayHelper.ToJson(this._moveHistory.AsEnumerable().ToArray(), true);
        //    MoveSequence moveData = new MoveSequence();
        //    moveData.MovesInTime = this._moveHistory.AsEnumerable().ToArray();
        //    moveData.Speed = this.MoveSpeed;
        //    var data = JsonUtility.ToJson(moveData, true);

        //    StreamWriter writer = new StreamWriter(SaveFilePath, true);
        //    writer.Write(data);
        //    writer.Close();
        //}

        FightData container = new FightData(GameSettings.PlayerNumber);

        var y = this._moveHistory.Count;

        if (SaveFilePath != null)
        {
            for (int playerNumber = 1; playerNumber <= GameSettings.PlayerNumber; playerNumber++)
            {
                MoveSequence moveSequence = new MoveSequence();
                moveSequence.MovesInTime = this._moveHistory.AsEnumerable().Where(x => x.Item1 == playerNumber).Select(x => x.Item2).ToArray();
                moveSequence.Speed = this.MoveSpeed;
                container.AddMoveSequence(playerNumber, moveSequence);
            }
        }

        var data = JsonUtility.ToJson(container, true);
        StreamWriter writer = new StreamWriter(SaveFilePath, true);
        writer.Write(data);
        writer.Close();
    }
}
