using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    public bool RunTestFight = false;
    public float TestFightAttackSpeed;

    public string DataPath;
    public string MoveHistoryDestination;

    public MovementPath[] Paths;
    public float[][] Delays;
    public GameObject PlayerPrefab;
    public GameObject AttackSpawnerPrefab;
    public GameObject MusicManagerPrefab;
    public AudioClip Tune;
    public double MusicDelay = 5.0f;
    public double SpawnDelay;
    public EndMenu EndMenu;
    public BossAnimation BossAnimation;

    private double _fightStartTime;
    private MoveRecorder _moveRecorder;
    private List<Player> _players = new List<Player>();

    void Start()
    {
        _fightStartTime = AudioSettings.dspTime + MusicDelay;
        var musicManagerObject = Instantiate(MusicManagerPrefab);
        var musicManager = musicManagerObject.GetComponent<MusicManager>();
        musicManager.Tune.clip = this.Tune;
        musicManager.PlayMusicStartTime = _fightStartTime;
        this.BossAnimation.MoveStartTime = _fightStartTime;

        if (RunTestFight)
        {
            _moveRecorder = this.gameObject.AddComponent<MoveRecorder>();
            _moveRecorder.RecordingStartTime = _fightStartTime;
            _moveRecorder.MoveSpeed = this.TestFightAttackSpeed;

            if (MoveHistoryDestination != null)
                _moveRecorder.SaveFilePath = MoveHistoryDestination;

            MoveSequence movesData = new MoveSequence();
            movesData.Speed = this.TestFightAttackSpeed;
            movesData.MovesInTime = new MoveData[] { new MoveData(MoveType.Forward, 0) };

            for (int i = 0; i < GameSettings.PlayerNumber; i++)
            {
                Player newPlayer = SpawnPlayer(i + 1, i, movesData);
                this._players.Add(newPlayer);
            }
        }
        else
        {
            var recordReader = new RecordReader(this.DataPath);
            FightData fightData = recordReader.MovesData;

            this.BossAnimation.AnimationSpeed = fightData.BossAnimationSpeed;

            for (int i = 1; i <= GameSettings.PlayerNumber; i++)
            {
                Player newPlayer = SpawnPlayer(i, i - 1, fightData.GetMovesForPlayerNumber(i));
                this._players.Add(newPlayer);
            }
        }
    }

    private Player SpawnPlayer(int playerNumber, int index, MoveSequence moves)
    {
        var playerObject = Instantiate(
            PlayerPrefab, this.Paths[index].PlayerTransform.position, this.Paths[index].PlayerTransform.rotation);
        Player player = playerObject.GetComponent<Player>();
        if (playerObject.transform.position.x < this.gameObject.transform.position.x)
            player.IsFlipped = true;
        player.PlayerNumber = playerNumber;
        player.Attack.PlayerInput = new PlayerInput(playerNumber);

        PlayerDeathObserver observer = new PlayerDeathObserver();
        observer.UpdateAction = this.OnPlayerDeath;
        player.AddPlayerDeathObserver(observer);

        if (RunTestFight)
            player.Attack.AddMoveObserver(_moveRecorder.MovePerformedObserver);

        var attackSpawnerObject = Instantiate(
            AttackSpawnerPrefab, this.Paths[index].AttakSpawnerTransform.position, this.Paths[index].AttakSpawnerTransform.rotation);
        var attackSpawner = attackSpawnerObject.GetComponent<BossAttackSpawner>();
        attackSpawner.Target = player;
        attackSpawner.AttackPath = this.Paths[index];
        attackSpawner.MovesData = moves;
        attackSpawner.SpawnDelay = this.SpawnDelay;
        attackSpawner.SpawningStartTime = this._fightStartTime;

        return player;
    }

    public void OnPlayerDeath(Player player)
    {
        print(this._players.Count + ", " + player.PlayerNumber);

        this._players.Remove(player);
        if (this._players.Count == 0)
            OnFightEnd();

    }

    public void OnFightEnd()
    {
        this.EndMenu.GameResult = this._players.Count == 0 ? GameResult.Defeat : GameResult.Victory;
    }
}
