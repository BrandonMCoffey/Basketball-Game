using DG.Tweening;
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
    [SerializeField] private CrowdController _crowdController;
    [SerializeField] private PlayerUIManager _playerUIManager;
    [SerializeField] private SlideInPanel _cardCatalogPanel;
    [SerializeField] private RectTransform _minimumMouseXShow;
    [SerializeField] private RectTransform _minimumMouseXPlace;
    [SerializeField, Range(-0.5f, 0.5f)] private float _dragPlayerYOffset = -0.1f;
    [SerializeField, Range(0f, 2f)] private float _dragPlayerXMult = 0.9f;
    [SerializeField] private LayerMask _floorMask = 1;
    [SerializeField] private Vector2 _spacingBetweenPlayersXZ = new Vector2(2f, 3f);
    [SerializeField] private Vector3 _outOfBoundsPlayerPos = new Vector3(10f, 0f, 0f);
    [SerializeField] private List<RandomPlayerArtData> _randomPlayerArt;

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

    private void Start()
    {
        if (_randomPlayerArt.Count > 0)
        {
            foreach (var player in _players)
            {
                player.PlayerArt.SetPlayerArt(_randomPlayerArt[Random.Range(0, _randomPlayerArt.Count)].GetData());
            }
        }
    }

    public Vector3 GetPlayerPosition(int index)
    {
        var player = GetPlayer(index);
        return player != null ? player.transform.position : transform.position;
    }

    public bool NewPlayerToPlace(PlayerData data)
    {
        foreach (var player in _players)
        {
            if (player.PlayerData == null)
            {
                _placingPlayer = player;
                _placingPlayer.SetAnimation("Placing");
                if (data.HasArtData) _placingPlayer.PlayerArt.SetPlayerArt(data.ArtData);
                return true;
            }
        }
        return false;
    }

    public Vector3 OffsetMousePosToPlayer(Vector2 mousePos)
    {
        mousePos.x = _dragPlayerXMult * (mousePos.x - Screen.width * 0.5f) + Screen.width * 0.5f;
        mousePos.y += _dragPlayerYOffset * Screen.height;
        return mousePos;
    }

    public Vector2 PlayerPosToMouse(int index)
    {
        var player = GetPlayer(index);
        if (player == null) return new Vector2(-500, 0);
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(player.transform.position);
        screenPoint.x = (screenPoint.x - Screen.width * 0.5f) / _dragPlayerXMult + Screen.width * 0.5f;
        screenPoint.y -= _dragPlayerYOffset * Screen.height;
        return screenPoint;
    }

    public bool UpdatePlacingPlayer(Vector2 mousePos)
    {
        if (_placingPlayer == null) return false;

        var ray = Camera.main.ScreenPointToRay(OffsetMousePosToPlayer(mousePos));
        if (mousePos.x > _minimumMouseXPlace.position.x && Physics.Raycast(ray, out var hitInfo, 100f, _floorMask))
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
        else if (mousePos.x <= _minimumMouseXShow.position.x)
        {
            _placingPlayer.UpdateCanPlace(_outOfBoundsPlayerPos, false);
            return false;
        }
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
            if (_players.All(p => p.PlayerData != null))
            {
                _cardCatalogPanel.SetShown(false);
                _players.Sort((a, b) => b.transform.position.x.CompareTo(a.transform.position.x));
                RefreshPlayers?.Invoke();
                _playerUIManager.ToggleSelectPlayer(0);
            }
            else
            {
                RefreshPlayers?.Invoke();
            }
            return true;
        }
        _placingPlayer.UpdateCanPlace(_outOfBoundsPlayerPos, false);
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
        _crowdController.SetPlaying(true);
        for (int i = 0; i < TimelineActions.Count; i++)
        {
            TimelineAction timelineAction = TimelineActions[i];
            var player = timelineAction.Player;
            if (player.PlayerData != null)
            {
                var action = player.PlayerData.GetAction(timelineAction.ActionIndex);
                if (action.Points > 0) ui.AddPoints(action.Points);
                if (action.Mult > 0) ui.AddMult(action.Mult);
                _crowdController.SetHype(ui.Points * ui.Mult / 500f);
                player.SetActionText(action.Name, action.Duration);
                player.SetAnimation("Action", 0.25f);
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
                    Vector3 startPos = player.BasketballPosition;
                    for (float t = 0; t < 1f; t += Time.deltaTime * timeScale * 2f)
                    {
                        var pos = Vector3.Lerp(startPos, otherPlayer.BasketballPosition, t);
                        float yOffset = Mathf.Abs(0.5f - t) * 2f;
                        pos += Vector3.up * (1f - yOffset * yOffset);
                        _basketball.transform.position = pos;
                        yield return null;
                    }
                }
                else if (action.Type == ActionType.Shot)
                {
                    player.FacePosition(_net.position, action.Duration);
                    float timeScale = 1f / action.Duration;
                    for (float t = 0; t < 1f; t += Time.deltaTime * timeScale * 2f)
                    {
                        _basketball.transform.position = player.BasketballPosition;
                        yield return null;
                    }

                    var startPos = player.BasketballPosition;
                    var endPos = _net.position;
                    var controlPoint = (startPos + endPos) / 2f + Vector3.up * (2f + Vector3.Distance(startPos, endPos) / 3f);

                    for (float t = 0; t < 1f; t += Time.deltaTime * timeScale * 2f)
                    {
                        float oneMinusT = 1f - t;
                        Vector3 pos = oneMinusT * oneMinusT * startPos + 2f * oneMinusT * t * controlPoint + t * t * endPos;
                        _basketball.transform.position = pos; 
                        yield return null;
                    }
                    _basketball.transform.position = endPos;

                    var groundPos = new Vector3(endPos.x, 0.22f, endPos.z);
                    yield return _basketball.transform.DOMove(groundPos, 0.3f).SetEase(Ease.InQuad).WaitForCompletion();
                    break;
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
                player.SetAnimation("Idle", 0.25f);
            }
        }
        yield return new WaitForSeconds(1f);
        _crowdController.SetPlaying(false);
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
