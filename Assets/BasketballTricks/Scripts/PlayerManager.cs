using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [SerializeField] private List<Player> _players = new List<Player>();
    [SerializeField] private Basketball _basketball;
    [SerializeField] private Transform _net;
    [SerializeField] private RectTransform _minimumMouseX;
    [SerializeField] private float _dragPlayerYOffset = -150f;
    [SerializeField] private LayerMask _floorMask = 1;
    [SerializeField] private Vector2 _spacingBetweenPlayersXZ = new Vector2(2f, 3f);

    public event System.Action RefreshPlayers = delegate { };

    public List<TimelineAction> TimelineActions { get; private set; } = new List<TimelineAction>();
    public event System.Action RefreshTimeline = delegate { };

    public List<Player> Players => _players;
    public Player GetPlayer(int index) => index < _players.Count ? _players[index] : null;

    private bool _simulating;
    private Player _placingPlayer;

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

    public bool NewPlayerToPlace()
    {
        foreach (var player in _players)
        {
            if (player.PlayerData == null)
            {
                _placingPlayer = player;
                _placingPlayer.SetAnimation("Placing");
                return true;
            }
        }
        return false;
    }

    public bool UpdatePlacingPlayer(Vector2 mousePos)
    {
        if (_placingPlayer == null) return false;

        var ray = Camera.main.ScreenPointToRay(mousePos + Vector2.up * _dragPlayerYOffset);
        if (mousePos.x > _minimumMouseX.position.x && Physics.Raycast(ray, out var hitInfo, 100f, _floorMask))
        {
            var position = hitInfo.point;
            foreach (var player in _players)
            {
                if (player.PlayerData != null)
                {
                    var playerPos = player.transform.position;
                    float xDist = Mathf.Abs(position.x - playerPos.x);
                    float zDist = Mathf.Abs(position.z - playerPos.z);
                    if (xDist * xDist / (_spacingBetweenPlayersXZ.x * _spacingBetweenPlayersXZ.x) + zDist * zDist / (_spacingBetweenPlayersXZ.y * _spacingBetweenPlayersXZ.y) <= 1)
                    {
                        _placingPlayer.UpdateCanPlace(position, false);
                        return false;
                    }
                }
            }
            _placingPlayer.UpdateCanPlace(position, true);
            return true;
        }
        // Should move anyways (Match floor plane?)
        var plane = new Plane(Vector3.up, Vector3.up * 0.002f);
        plane.Raycast(ray, out float enter);
        _placingPlayer.UpdateCanPlace(ray.GetPoint(enter), false);
        return false;
    }

    public bool AttemptPlacePlayer(PlayerData data, Vector2 mousePos)
    {
        if (_placingPlayer == null) return false;

        if (UpdatePlacingPlayer(mousePos))
        {
            _placingPlayer.Place(data);
            _placingPlayer = null;
            RefreshPlayers?.Invoke();
            return true;
        }
        _placingPlayer.UpdateCanPlace(Vector3.zero, false);
        _placingPlayer = null;
        return false;
    }

    public void AddAction(Player player, int actionIndex)
    {
        TimelineActions.Add(new TimelineAction(player, actionIndex));
        OnTimelineUpdated();
    }

    public void RemoveAction(int timelineIndex)
    {
        TimelineActions.RemoveAt(timelineIndex);
        OnTimelineUpdated();
    }

    private void OnTimelineUpdated()
    {
        RefreshTimeline?.Invoke();

        for (int i = 0; i < TimelineActions.Count; i++)
        {
            TimelineAction timelineAction = TimelineActions[i];
            var player = timelineAction.Player;
            if (player.PlayerData != null)
            {
                // Add visual to player
            }
        }
    }

    public void RunSimulation(SimulatePanelUI ui)
    {
        if (_simulating) return;
        // Verify if sequence is valid
        _simulating = true;
        StartCoroutine(SimulateRoutine(ui));
    }
    private IEnumerator SimulateRoutine(SimulatePanelUI ui)
    {
        ui.ResetScore();
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < TimelineActions.Count; i++)
        {
            TimelineAction timelineAction = TimelineActions[i];
            var player = timelineAction.Player;
            if (player.PlayerData != null)
            {
                var action = player.PlayerData.GetAction(timelineAction.ActionIndex);
                if (action.Points > 0) ui.AddPoints(action.Points);
                if (action.Mult > 0) ui.AddMult(action.Mult);
                player.SetActionText(action.Name, action.Duration);
                player.EmitParticles();
                Debug.Log($"Play Action: {action.Name} for {action.Duration} seconds. Get {action.Points} points and {action.Mult} mult.");
                if (action.Type == ActionType.Pass)
                {
                    var otherPlayer = TimelineActions[i + 1].Player;
                    player.FaceOtherPlayer(otherPlayer, action.Duration);
                    float timeScale = 1f / action.Duration;
                    for (float t = 0; t < 1f; t += Time.deltaTime * timeScale * 2f)
                    {
                        _basketball.transform.position = player.BasketballPosition;
                        yield return null;
                    }
                    for (float t = 0; t < 1f; t += Time.deltaTime * timeScale * 2f)
                    {
                        var pos = Vector3.Lerp(player.BasketballPosition, otherPlayer.BasketballPosition, t);
                        pos += Vector3.up * Mathf.Abs(0.5f - t) * 0.2f;
                        _basketball.transform.position = pos;
                        yield return null;
                    }
                }
                else if (action.Type == ActionType.Shot)
                {
                    player.FacePosition(_net.position, action.Duration);
                    float timeScale = 1f / action.Duration;
                    for (float t = 0; t < 1f; t += Time.deltaTime * timeScale)
                    {
                        _basketball.transform.position = player.BasketballPosition;
                        yield return null;
                    }
                }
                else
                {
                    float timeScale = 1f / action.Duration;
                    for (float t = 0; t < 1f; t += Time.deltaTime * timeScale)
                    {
                        _basketball.transform.position = player.BasketballPosition;
                        yield return null;
                    }
                }
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
