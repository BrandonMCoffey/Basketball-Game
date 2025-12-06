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
    [SerializeField] private BasketballGoal _goal;
    [SerializeField] private CrowdController _crowdController;
    [SerializeField] private PlayerUIManager _playerUIManager;
    [SerializeField] private SlideInPanel _cardCatalogPanel;
    [SerializeField] private TrickshotCamera _trickshotCamera;

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
        Hype = 0;
        UpdateHype?.Invoke(Hype);
        if (_randomPlayerArt.Count > 0)
        {
            foreach (var player in _players)
            {
                player.PlayerArt.SetPlayerArt(_randomPlayerArt[Random.Range(0, _randomPlayerArt.Count)].GetData());
            }
        }
        _trickshotCamera.SetNormalCamera();
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
        StartCoroutine(TrickshotRoutine());
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

    private IEnumerator TrickshotRoutine()
    {
        _crowdController.SetPlaying(true);
        Player playerWithBall = TimelineActions[0].Player;
        _trickshotCamera.SetTrickCamera(playerWithBall.CameraTarget);
        yield return StartCoroutine(HoldBallRoutine(playerWithBall, 1f, PlayerAnimation.IdleHold));
        for (int i = 0; i < TimelineActions.Count; i++)
        {
            yield return StartCoroutine(HoldBallRoutine(playerWithBall, 0.3f, PlayerAnimation.IdleHold));

            var timelinePlayer = TimelineActions[i].Player;
            if (playerWithBall != timelinePlayer)
            {
                yield return StartCoroutine(PassRoutine(playerWithBall, timelinePlayer));
                playerWithBall = timelinePlayer;
                yield return StartCoroutine(HoldBallRoutine(playerWithBall, 0.3f, PlayerAnimation.IdleHold));
            }

            var action = playerWithBall.CardData.GetAction(TimelineActions[i].ActionIndex);
            ApplyActionEffects(playerWithBall, action);
            if (action.Type == ActionType.Pass)
            {
                var passToPlayer = TimelineActions[i + 1].Player;
                yield return StartCoroutine(PassRoutine(playerWithBall, passToPlayer));
                playerWithBall = passToPlayer;
            }
            else if (action.Type == ActionType.Shot)
            {
                yield return StartCoroutine(ShotRoutine(playerWithBall));

                if (_allowMultipleShots && i < TimelineActions.Count - 1)
                {
                    playerWithBall = _players[2]; // Give ball to center
                    _trickshotCamera.SetTrickFocus(playerWithBall.CameraTarget);
                }
                else
                {
                    yield return new WaitForSeconds(2f);
                    break;
                }
            }
            else // Trick
            {
                yield return StartCoroutine(HoldBallRoutine(playerWithBall, action.Duration, action.Animation));
            }
        }
        OnSequenceCompleted();
    }

    private IEnumerator HoldBallRoutine(Player player, float duration, PlayerAnimation? animation)
    {
        if (animation.HasValue) player.SetAnimation(animation.Value);
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            _basketball.transform.position = player.BasketballPosition;
            yield return null;
        }
    }

    private void ApplyActionEffects(Player player, ActionData action)
    {
        // TODO: Use data to calculate and apply effects
        Debug.Log($"Play Action: {action.Name} for {action.Duration} seconds. Get {action.HypeGain} points.");

        Hype += action.HypeGain.GetValue(action.ActionLevel);
        _crowdController.SetHype(Hype / 100f);
        UpdateHype?.Invoke(Hype);
        player.SetActionText(action.Name, action.Duration);
        player.EmitParticles();
    }

    private IEnumerator PassRoutine(Player fromPlayer, Player toPlayer)
    {
        float splitTime = 0.5f;// Mathf.Max(0f, action.Duration - _tossPrepTime - _catchHoldTime) * 0.5f;

        // Face each other
        _trickshotCamera.SetTrickFocus(fromPlayer.CameraTarget, toPlayer.CameraTarget);
        fromPlayer.FaceOtherPlayer(toPlayer, splitTime);
        yield return StartCoroutine(HoldBallRoutine(fromPlayer, splitTime, PlayerAnimation.IdleHold));

        // Pass the ball (toss hold)
        yield return StartCoroutine(HoldBallRoutine(fromPlayer, _tossPrepTime, PlayerAnimation.Pass));

        // Pass the ball (arc)
        Vector3 startPos = fromPlayer.BasketballPosition;
        Vector3 endPos = toPlayer.HeadPosition; // Assumes the player catches near the head (TODO: Make this better)

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
            _basketball.transform.position = Vector3.Lerp(new Vector3(xzPos.x, yPos, xzPos.z), toPlayer.BasketballPosition, catchLerpDelta);

            if (!catchTriggered && t >= catchT)
            {
                catchTriggered = true;
                toPlayer.SetAnimation(PlayerAnimation.Catch);
                _trickshotCamera.SetTrickFocus(toPlayer.CameraTarget);
            }
            yield return null;
        }
        fromPlayer.SetAnimation(PlayerAnimation.Idle);

        // Pass the ball (catch hold)
        yield return StartCoroutine(HoldBallRoutine(toPlayer, _catchHoldTime, null));
    }

    private IEnumerator ShotRoutine(Player player)
    {
        float splitTime = 0.5f;

        // Face the net
        _trickshotCamera.SetTrickFocus(player.CameraTarget, _goal.NetTarget);
        player.FacePosition(_goal.NetTarget.position, splitTime);
        yield return StartCoroutine(HoldBallRoutine(player, splitTime, PlayerAnimation.IdleHold));

        // Shoot (hold)
        yield return StartCoroutine(HoldBallRoutine(player, _shotPrepTime, PlayerAnimation.Shoot));

        // Shoot (arc)
        var startPos = player.BasketballPosition;
        var endPos = _goal.NetTarget.position + Vector3.up * 0.25f;

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

        player.SetAnimation(PlayerAnimation.Idle); // Victory dance?

        // Fall through net
        var groundPos = new Vector3(endPos.x, 0.22f, endPos.z);
        yield return _basketball.transform.DOMove(groundPos, 1f).SetEase(Ease.OutBounce).WaitForCompletion();
    }

    private void OnSequenceCompleted()
    {
        _simulating = false;
        _crowdController.SetPlaying(false);
        TimelineActions.Clear();
        RefreshTimeline?.Invoke();
        _trickshotCamera.SetNormalCamera();
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
