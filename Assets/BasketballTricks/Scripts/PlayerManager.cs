using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [SerializeField] private List<Player> _players = new List<Player>();
    [SerializeField] private float _arcGravity = 9.81f;
    [SerializeField] private bool _allowMultipleShots;

    [Header("References")]
    [SerializeField] private Basketball _basketball;
    [SerializeField] private Transform _net;
    [SerializeField] private CrowdController _crowdController;
    [SerializeField] private PlayerUIManager _playerUIManager;
    [SerializeField] private SlideInPanel _cardCatalogPanel;
    [SerializeField] private CinemachineTargetGroup _cameraGroup;

    [Header("Drag & Drop Placement")]
    [SerializeField] private RectTransform _minimumMouseXShow;
    [SerializeField] private RectTransform _minimumMouseXPlace;
    [SerializeField, Range(-0.5f, 0.5f)] private float _dragPlayerYOffset = -0.1f;
    [SerializeField, Range(0f, 2f)] private float _dragPlayerXMult = 0.9f;
    [SerializeField] private LayerMask _floorMask = 1;
    [SerializeField] private Vector2 _spacingBetweenPlayersXZ = new Vector2(2f, 3f);
    [SerializeField] private Vector3 _outOfBoundsPlayerPos = new Vector3(10f, 0f, 0f);
    [SerializeField] private List<RandomPlayerArtData> _randomPlayerArt;

    private const float _tossPrepTime = 0.5f;
    private const float _catchAnimationLeadTime = 0.5f;
    private const float _catchHoldTime = 0.5f;
    private const float _shotPrepTime = 0.5f;

    public List<GameAction> TimelineActions { get; private set; } = new List<GameAction>();
    public static event System.Action RefreshTimeline = delegate { };
    public static event System.Action RefreshPlayers = delegate { };
    public static event System.Action<float> UpdateHype = delegate { };

    public float Hype { get; private set; }

    private bool _simulating;
    private Player _placingPlayer;

    public List<Player> Players => _players;
    public bool Simulating => _simulating;
    public Player GetPlayer(int index) => index < _players.Count ? _players[index] : null;

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
        foreach (var player in _players)
        {
            _cameraGroup.AddMember(player.transform, 1, 2);
        }
    }

    public Vector3 GetPlayerPosition(int index)
    {
        var player = GetPlayer(index);
        return player != null ? player.transform.position : transform.position;
    }

    public bool NewPlayerToPlace(GameCard data)
    {
        foreach (var player in _players)
        {
            if (player.CardData == null)
            {
                _placingPlayer = player;
                _placingPlayer.SetAnimation(PlayerAnimation.Dangle, 0f);
                if (data.PlayerData.HasArtData) _placingPlayer.PlayerArt.SetPlayerArt(data.PlayerData.ArtData);
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
                if (player.CardData != null)
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

    public bool AttemptPlacePlayer(GameCard data, Vector2 mousePos)
    {
        if (_placingPlayer == null) return false;

        if (UpdatePlacingPlayer(mousePos))
        {
            _placingPlayer.Place(data);
            _placingPlayer = null;
            if (_players.All(p => p.CardData != null))
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

    public void PreviewSequence(List<GameAction> actions)
    {
        TimelineActions = new List<GameAction>(actions);
        OnTimelineUpdated();
    }

    public void AddAction(Player player, int actionIndex)
    {
        TimelineActions.Add(new GameAction(player, actionIndex));
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
            GameAction timelineAction = TimelineActions[i];
            var player = timelineAction.Player;
            if (player.CardData != null)
            {
                // Add visual to player
            }
        }
    }

    public bool RunSimulation()
    {
        if (_simulating || !IsSequenceValid()) return false;
        _simulating = true;
        StartCoroutine(SimulateRoutine());
        return true;
    }

    private bool IsSequenceValid()
    {
        for (int i = 0; i < TimelineActions.Count; i++)
        {
            GameAction timelineAction = TimelineActions[i];
            var player = timelineAction.Player;
            if (player.CardData == null)
            {
                Debug.LogWarning("Cannot simulate sequence: A player has no data assigned.");
                return false;
            }
            var action = player.CardData.GetAction(timelineAction.ActionIndex);
            switch (action.Type)
            {
                case ActionType.Pass:
                    if (i + 1 >= TimelineActions.Count || TimelineActions[i + 1].Player == player)
                    {
                        Debug.LogWarning("Cannot simulate sequence: Invalid pass action.");
                        return false;
                    }
                    break;
                case ActionType.Shot:
                    if (!_allowMultipleShots && i != TimelineActions.Count - 1)
                    {
                        Debug.LogWarning("Cannot simulate sequence: Shot action must be last in sequence.");
                        return false;
                    }
                    break;
            }
        }
        return true;
    }

    private IEnumerator SimulateRoutine()
    {
        Hype = 0;
        UpdateHype?.Invoke(Hype);
        var startPlayer = TimelineActions[0].Player;
        startPlayer.SetAnimation(PlayerAnimation.IdleHold);
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            _basketball.transform.position = startPlayer.BasketballPosition;
            yield return null;
        }
        _crowdController.SetPlaying(true);
        Player playerWithBall = TimelineActions[0].Player;
        foreach (var player in _players)
        {
            _cameraGroup.RemoveMember(player.transform);
        }
        _cameraGroup.RemoveMember(_basketball.transform);
        _cameraGroup.RemoveMember(_net);
        _cameraGroup.AddMember(playerWithBall.transform, 1, 2);
        for (int i = 0; i < TimelineActions.Count; i++)
        {
            GameAction timelineAction = TimelineActions[i];
            var player = timelineAction.Player;
            if (player.CardData == null) continue;

            // Pass
            if (playerWithBall != player)
            {
                _cameraGroup.AddMember(player.transform, 1, 2);
                Debug.Log($"Basic pass from {playerWithBall.name} to {player.name}");
                // Face each other
                playerWithBall.FaceOtherPlayer(player, 0.2f);
                for (float t = 0; t < 0.2f; t += Time.deltaTime)
                {
                    _basketball.transform.position = playerWithBall.BasketballPosition;
                    yield return null;
                }

                playerWithBall.SetAnimation(PlayerAnimation.Pass);
                for (float t = 0; t < _tossPrepTime; t += Time.deltaTime)
                {
                    _basketball.transform.position = playerWithBall.BasketballPosition;
                    yield return null;
                }

                // Pass the ball (arc)
                Vector3 startPos = playerWithBall.BasketballPosition;
                Vector3 endPos = player.HeadPosition;

                bool catchTriggered = false;
                float splitTime = 0.5f;
                float timeScale = 1f;
                float catchT = Mathf.Max(0f, splitTime - _catchAnimationLeadTime) * timeScale;

                Vector3 displacementXZ = new Vector3(endPos.x - startPos.x, 0, endPos.z - startPos.z);
                Vector3 velocityXZ = displacementXZ * timeScale;
                float displacementY = endPos.y - startPos.y;
                float velocityY = (displacementY * timeScale) + 0.5f * _arcGravity * splitTime;

                for (float t = 0; t < 1f; t += Time.deltaTime * timeScale)
                {
                    Vector3 xzPos = startPos + velocityXZ * t;
                    float yPos = startPos.y + velocityY * t - 0.5f * _arcGravity * t * t;
                    float catchLerpDelta = catchTriggered ? (t - catchT) / (1 - catchT) : 0;
                    _basketball.transform.position = Vector3.Lerp(new Vector3(xzPos.x, yPos, xzPos.z), player.BasketballPosition, catchLerpDelta);

                    if (!catchTriggered && t >= catchT)
                    {
                        catchTriggered = true;
                        player.SetAnimation(PlayerAnimation.Catch);
                        _cameraGroup.RemoveMember(playerWithBall.transform);
                    }
                    yield return null;
                }

                // Pass the ball (catch hold)
                for (float t = 0; t < _catchHoldTime; t += Time.deltaTime)
                {
                    _basketball.transform.position = player.BasketballPosition;
                    yield return null;
                }
            }
            playerWithBall.SetAnimation(PlayerAnimation.Idle);
            playerWithBall = player;

            var action = player.CardData.GetAction(timelineAction.ActionIndex);
            Hype += action.HypeGain.GetValue(action.ActionLevel);
            _crowdController.SetHype(Hype / 100f);
            UpdateHype?.Invoke(Hype);
            player.SetActionText(action.Name, action.Duration);
            player.EmitParticles();
            Debug.Log($"Play Action: {action.Name} for {action.Duration} seconds. Get {action.HypeGain} points.");
            if (action.Type == ActionType.Pass)
            {
                float splitTime = 0.5f;// Mathf.Max(0f, action.Duration - _tossPrepTime - _catchHoldTime) * 0.5f;

                // Face each other
                var otherPlayer = TimelineActions[i + 1].Player;
                _cameraGroup.AddMember(otherPlayer.transform, 1, 2);
                player.FaceOtherPlayer(otherPlayer, splitTime);
                player.SetAnimation(PlayerAnimation.IdleHold);
                for (float t = 0; t < splitTime; t += Time.deltaTime)
                {
                    _basketball.transform.position = player.BasketballPosition;
                    yield return null;
                }

                // Pass the ball (toss hold)
                player.SetAnimation(action.Animation);
                for (float t = 0; t < _tossPrepTime; t += Time.deltaTime)
                {
                    _basketball.transform.position = player.BasketballPosition;
                    yield return null;
                }

                // Pass the ball (arc)
                Vector3 startPos = player.BasketballPosition;
                Vector3 endPos = otherPlayer.HeadPosition; // Assumes the player catches near the head (TODO: Make this better)

                bool catchTriggered = false;
                float timeScale = 1f / splitTime;
                float catchT = Mathf.Max(0f, splitTime - _catchAnimationLeadTime) * timeScale;

                Vector3 displacementXZ = new Vector3(endPos.x - startPos.x, 0, endPos.z - startPos.z);
                Vector3 velocityXZ = displacementXZ * timeScale;
                float displacementY = endPos.y - startPos.y;
                float velocityY = (displacementY * timeScale) + 0.5f * _arcGravity * splitTime;

                for (float t = 0; t < 1f; t += Time.deltaTime * timeScale)
                {
                    Vector3 xzPos = startPos + velocityXZ * t;
                    float yPos = startPos.y + velocityY * t - 0.5f * _arcGravity * t * t;
                    float catchLerpDelta = catchTriggered ? (t - catchT) / (1 - catchT) : 0;
                    _basketball.transform.position = Vector3.Lerp(new Vector3(xzPos.x, yPos, xzPos.z), otherPlayer.BasketballPosition, catchLerpDelta);

                    if (!catchTriggered && t >= catchT)
                    {
                        catchTriggered = true;
                        otherPlayer.SetAnimation(PlayerAnimation.Catch);
                        _cameraGroup.RemoveMember(player.transform);
                    }
                    yield return null;
                }

                // Pass the ball (catch hold)
                for (float t = 0; t < _catchHoldTime; t += Time.deltaTime)
                {
                    _basketball.transform.position = otherPlayer.BasketballPosition;
                    yield return null;
                }
                playerWithBall = otherPlayer;
            }
            else if (action.Type == ActionType.Shot)
            {
                float splitTime = Mathf.Max(0.01f, action.Duration - _shotPrepTime) * 0.5f;

                // Face the net
                _cameraGroup.AddMember(_net, 1, 3);
                player.FacePosition(_net.position, splitTime);
                player.SetAnimation(PlayerAnimation.IdleHold);
                for (float t = 0; t < splitTime; t += Time.deltaTime)
                {
                    _basketball.transform.position = player.BasketballPosition;
                    yield return null;
                }

                // Shoot (hold)
                player.SetAnimation(action.Animation); // Auto transitions to IdleHold in Animator Graph
                for (float t = 0; t < _shotPrepTime; t += Time.deltaTime)
                {
                    _basketball.transform.position = player.BasketballPosition;
                    yield return null;
                }

                // Shoot (arc)
                var startPos = player.BasketballPosition;
                var endPos = _net.position;

                Vector3 displacementXZ = new Vector3(endPos.x - startPos.x, 0, endPos.z - startPos.z);
                Vector3 velocityXZ = displacementXZ / splitTime;
                float displacementY = endPos.y - startPos.y;
                float velocityY = (displacementY / splitTime) + 0.5f * _arcGravity * splitTime;

                for (float t = 0; t < splitTime; t += Time.deltaTime)
                {
                    Vector3 xzPos = startPos + velocityXZ * t;
                    float yPos = startPos.y + velocityY * t - 0.5f * _arcGravity * t * t;
                    _basketball.transform.position = new Vector3(xzPos.x, yPos, xzPos.z);
                    yield return null;
                }
                _basketball.transform.position = endPos;

                // Fall through net
                var groundPos = new Vector3(endPos.x, 0.22f, endPos.z);
                yield return _basketball.transform.DOMove(groundPos, 1f).SetEase(Ease.OutBounce).WaitForCompletion();
                
                if (_allowMultipleShots && i < TimelineActions.Count - 1)
                {
                    _cameraGroup.RemoveMember(player.transform);
                    _cameraGroup.RemoveMember(_net);
                    _cameraGroup.AddMember(_players[2].transform, 1, 2);
                    playerWithBall = _players[2]; // Give ball to Center
                    playerWithBall.SetAnimation(PlayerAnimation.IdleHold);
                    _basketball.transform.position = playerWithBall.BasketballPosition; // TODO: Pickup ball?
                }
                else
                {
                    yield return new WaitForSeconds(2f);
                    break;
                }
            }
            else
            {
                player.SetAnimation(action.Animation);
                float timeScale = 1f / action.Duration;
                for (float t = 0; t < 1f; t += Time.deltaTime * timeScale)
                {
                    _basketball.transform.position = player.BasketballPosition;
                    yield return null;
                }
                player.SetAnimation(PlayerAnimation.Idle);
            }
        }
        OnSequenceCompleted();
    }

    private void OnSequenceCompleted()
    {
        _simulating = false;
        _crowdController.SetPlaying(false);
        TimelineActions.Clear();
        RefreshTimeline?.Invoke();
        _cameraGroup.RemoveMember(_net);
        foreach (var player in _players)
        {
            _cameraGroup.RemoveMember(player.transform);
        }
        foreach (var player in _players)
        {
            _cameraGroup.AddMember(player.transform, 1, 2);
        }
        foreach (var player in _players)
        {
            player.FacePosition(player.transform.position + new Vector3(0, 0, 1f), 1f);
            player.SetAnimation(PlayerAnimation.Idle);
        }
        _basketball.transform.position = new Vector3(0, -10f, 0);
    }
}

public struct GameAction
{
    public Player Player;
    public int ActionIndex;

    public GameAction(Player player, int actionIndex)
    {
        Player = player;
        ActionIndex = actionIndex;
    }
}
