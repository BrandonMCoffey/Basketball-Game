using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [SerializeField] private List<Player> _players = new List<Player>();
    [SerializeField] private LayerMask _floorMask = 1;
    [SerializeField] private Vector2 _spacingBetweenPlayersXZ = new Vector2(2f, 3f);

    public event System.Action RefreshPlayers = delegate { };

    public List<TimelineAction> TimelineActions { get; private set; } = new List<TimelineAction>();
    public event System.Action RefreshTimeline = delegate { };

    public List<Player> Players => _players;
    public Player GetPlayer(int index) => index < _players.Count ? _players[index] : null;

    private bool _simulating;

    private void OnValidate()
    {
        if (_players.Count == 0)
        {
            _players = GetComponentsInChildren<Player>().ToList();
        }
    }

    private void Awake()
    {
        Instance = this;
    }
    public Vector3 GetPlayerPosition(int index)
    {
        var player = GetPlayer(index);
        return player != null ? player.transform.position : transform.position;
    }

    public bool AttemptPlacePlayer(PlayerData data, Vector2 mousePos)
    {
        var ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out var hitInfo, 100f, _floorMask))
        {
            var position = hitInfo.point;
            foreach (var player in _players)
            {
                if (player.PlayerData != null)
                {
                    var playerPos = player.transform.position;
                    float xDist = Mathf.Abs(position.x - playerPos.x);
                    float zDist = Mathf.Abs(position.z - playerPos.z);
                    if (xDist < _spacingBetweenPlayersXZ.x && zDist < _spacingBetweenPlayersXZ.y)
                    {
                        // Existing player is too close (TODO: Visuals to show if can place)
                        return false;
                    }
                }
            }
            foreach (var player in _players)
            {
                if (player.PlayerData == null)
                {
                    player.Place(position, data);
                    RefreshPlayers?.Invoke();
                    return true;
                }
            }
        }
        return false;
    }

    public void AddAction(Player player, int actionIndex)
    {
        TimelineActions.Add(new TimelineAction(player, actionIndex));
        RefreshTimeline?.Invoke();
    }

    public void RemoveAction(int timelineIndex)
    {
        TimelineActions.RemoveAt(timelineIndex);
        RefreshTimeline?.Invoke();
    }

    public void RunSimulation(SimulatePanelUI ui)
    {
        if (_simulating) return;
        _simulating = true;
        StartCoroutine(SimulateRoutine(ui));
    }
    private IEnumerator SimulateRoutine(SimulatePanelUI ui)
    {
        ui.ResetScore();
        yield return new WaitForSeconds(1f);
        foreach (var timelineAction in TimelineActions)
        {
            var player = timelineAction.Player;
            if (player.PlayerData != null)
            {
                var action = player.PlayerData.GetAction(timelineAction.ActionIndex);
                if (action.Points > 0) ui.AddPoints(action.Points);
                if (action.Mult > 0) ui.AddMult(action.Mult);
                player.SetActionText(action.Name, action.Duration);
                player.EmitParticles();
                Debug.Log($"Play Action: {action.Name} for {action.Duration} seconds. Get {action.Points} points and {action.Mult} mult.");
                yield return new WaitForSeconds(action.Duration);
            }
        }
    }
}

public struct TimelineAction
{
    public Player Player;
    public int ActionIndex;

    public TimelineAction(Player player, int actionIndex)
    {
        Player = player;
        ActionIndex = actionIndex;
    }
}
