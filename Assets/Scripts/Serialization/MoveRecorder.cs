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

    private LinkedList<MoveData> _moveHistory = new LinkedList<MoveData>();
    private double? _previousMoveTime;

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

    void SaveMove(MoveType move)
    {
        double currentTime = AudioSettings.dspTime;
        _moveHistory.AddLast(new MoveData(
            move, _previousMoveTime.HasValue ? currentTime - _previousMoveTime.Value : currentTime - RecordingStartTime));
        _previousMoveTime = currentTime;
    }

    private void OnDestroy()
    {
        if (SaveFilePath != null)
        {
            //var data = JsonArrayHelper.ToJson(this._moveHistory.AsEnumerable().ToArray(), true);
            MoveSequence moveData = new MoveSequence();
            moveData.MovesInTime = this._moveHistory.AsEnumerable().ToArray();
            moveData.Speed = this.MoveSpeed;
            var data = JsonUtility.ToJson(moveData, true);

            StreamWriter writer = new StreamWriter(SaveFilePath, true);
            writer.Write(data);
            writer.Close();
        }
    }
}
