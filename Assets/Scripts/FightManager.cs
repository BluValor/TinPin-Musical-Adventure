using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    public bool RunTestFight = false;
    public float TestFightAttackSpeed;

    public string DataPath;
    public string MoveHistoryDestination;
    public int NumberOfPlayers;

    public MovementPath[] Paths;
    public float[][] Delays;
    public GameObject PlayerPrefab;
    public GameObject AttackSpawnerPrefab;
    public GameObject MusicManagerPrefab;
    public AudioClip Tune;
    public float MusicDelay = 5.0f;

    private double _fightStartTime;
    private MoveRecorder _moveRecorder;

    void Start()
    {
        _fightStartTime = AudioSettings.dspTime + MusicDelay;
        var musicManagerObject = Instantiate(MusicManagerPrefab);
        var musicManager = musicManagerObject.GetComponent<MusicManager>();
        musicManager.Tune.clip = this.Tune;
        musicManager.PlayMusicStartTime = _fightStartTime;

        MoveSequence MovesData;

        if (RunTestFight)
        {
            _moveRecorder = this.gameObject.AddComponent<MoveRecorder>();
            _moveRecorder.RecordingStartTime = _fightStartTime;
            _moveRecorder.MoveSpeed = this.TestFightAttackSpeed;

            if (MoveHistoryDestination != null)
                _moveRecorder.SaveFilePath = MoveHistoryDestination;

            MovesData = new MoveSequence();
            MovesData.Speed = this.TestFightAttackSpeed;
            MovesData.MovesInTime = new MoveData[] { new MoveData(MoveType.Forward, 0) };
        }
        else
        {
            var recordReader = new RecordReader(this.DataPath);
            MovesData = recordReader.MovesData;
        }

        
        for (int i = 0; i < NumberOfPlayers; i++)
        {
            var playerObject = Instantiate(
                PlayerPrefab, this.Paths[i].PlayerTransform.position, this.Paths[i].PlayerTransform.rotation);
            var player = playerObject.GetComponent<Player>();
            player.PlayerNumber = i + 1;
            player.Attack.PlayerInput = new PlayerInput(i + 1);

            if (RunTestFight)
                player.Attack.AddMoveObserver(_moveRecorder.MovePerformedObserver);

            var attackSpawnerObject = Instantiate(
                AttackSpawnerPrefab, this.Paths[i].AttakSpawnerTransform.position, this.Paths[i].AttakSpawnerTransform.rotation);
            var attackSpawner = attackSpawnerObject.GetComponent<AttackSpawner>();
            attackSpawner.Player = player;
            attackSpawner.AttackPath = this.Paths[i];
            attackSpawner.MovesData = MovesData;
            attackSpawner.SpawningStartTime = _fightStartTime;
        }
    }
    
    void Update()
    {
        
    }
}
