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
    [SerializeField] private ActionVisualPreview _actionVisualPreview;

    [Header("Drag & Drop Placement")]
    [SerializeField] private RectTransform _minimumMouseXShow;
    [SerializeField] private RectTransform _minimumMouseXPlace;
    [SerializeField, Range(-0.5f, 0.5f)] private float _dragPlayerYOffset = -0.1f;
    [SerializeField, Range(0f, 2f)] private float _dragPlayerXMult = 0.9f;
    [SerializeField] private LayerMask _floorMask = 1;
    [SerializeField] private Vector2 _spacingBetweenPlayersXZ = new Vector2(2f, 3f);
    [SerializeField] private Vector3 _outOfBoundsPlayerPos = new Vector3(10f, 0f, 0f);
    [SerializeField] private List<RandomPlayerArtData> _randomPlayerArt;

    private List<ActionVisualPreview> _actionVisualPreviews;

    private const float _tossPrepTime = 0.5f;
    private const float _catchAnimationLeadTime = 0.5f;
    private const float _catchHoldTime = 0.5f;
    private const float _shotPrepTime = 0.5f;

    public List<GameAction> TimelineActions { get; private set; } = new List<GameAction>();
    public static event System.Action RefreshTimeline = delegate { };
    public static event System.Action RefreshPlayers = delegate { };
    public static event System.Action<float> UpdateHype = delegate { };
    public static event System.Action<float, float> UpdateEnergy = delegate { };
    public static event System.Action UpdateEffectNextStack = delegate { };
    public List<EffectNext> EffectNextStack { get; private set; } = new List<EffectNext>();

    public float Hype { get; private set; }
    private float _maxEnergy = 5;

    private bool _simulating;
    private Player _placingPlayer;
    private float _energyRemaining;

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
        UpdateEnergy?.Invoke(0, _maxEnergy);
        if (_randomPlayerArt.Count > 0)
        {
            foreach (var player in _players)
            {
                player.PlayerArt.SetPlayerArt(_randomPlayerArt[Random.Range(0, _randomPlayerArt.Count)].GetData());
            }
        }
        _trickshotCamera.SetNormalCamera();
        _actionVisualPreviews = new List<ActionVisualPreview>(5);
        for (int i = 0; i < 5; i++)
        {
            var preview = Instantiate(_actionVisualPreview, transform);
            preview.gameObject.SetActive(false);
            _actionVisualPreviews.Add(preview);
        }
        _energyRemaining = _maxEnergy;
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

    public void PreviewSequence(List<GameAction> playedActions, List<ActionCard> cards)
    {
        if (_simulating) return;
        TimelineActions = new List<GameAction>(playedActions);
        RefreshTimeline?.Invoke();

        float sequenceCost = 0;
        int j = 0;
        int prevActionIndex = 0;
        Player prevPlayer = null;
        Player player = null;
        var tempEffectNextStack = new List<EffectNext>();
        List<int> skipActualStackIndexes = new List<int>();
        int count = TimelineActions.Count + cards.Count;
        for (int i = 0; i < count; i++)
        {
            bool played = i < TimelineActions.Count;

            // Extra Pass Check
            float adjustCost = 0;
            float adjustHype = 0;
            if (played && _actionVisualPreviews.Count <= j) _actionVisualPreviews.Add(Instantiate(_actionVisualPreview, transform));

            player = played ? TimelineActions[i].Player : cards[i - TimelineActions.Count].Action.Player;
            int actionIndex = played ? TimelineActions[i].ActionIndex : cards[i - TimelineActions.Count].Action.ActionIndex;
            var action = player.CardData.GetAction(actionIndex);

            if (prevPlayer != null && prevPlayer != player)
            {
                bool passCostsExtra = !(action.Type == ActionType.Pass || prevPlayer.CardData.GetAction(prevActionIndex).Type == ActionType.Pass);

                if (played)
                {
                    _actionVisualPreviews[j++].ShowPass(prevPlayer.transform.position, player.transform.position, Color.black, passCostsExtra);
                    if (_actionVisualPreviews.Count <= j) _actionVisualPreviews.Add(Instantiate(_actionVisualPreview, transform));
                }
                if (passCostsExtra)
                {
                    //Debug.Log($"Adjust {action.Name} by basic pass: 1");
                    adjustCost++;
                }
            }

            if (played)
            {
                sequenceCost += player.CardData.GetAction(actionIndex).Effects.Cost;
                switch (action.Type)
                {
                    case ActionType.Trick:
                        _actionVisualPreviews[j++].ShowTrick(player.transform.position);
                        break;
                    case ActionType.Pass:
                        // Handled above
                        break;
                    case ActionType.Shot:
                        _actionVisualPreviews[j++].ShowShot(player.transform.position + Vector3.up * 1f, _goal.NetTarget.position + Vector3.up * 0.1f, player.PositionColor);
                        player = Players[2]; // Center picks up
                        break;
                }
            }

            int tempCount = tempEffectNextStack.Count;
            int stackCount = EffectNextStack.Count;
            var removeTempIndexes = new List<int>();
            for (int k = 0; k < tempCount + stackCount; k++)
            {
                if (k >= tempCount && skipActualStackIndexes.Contains(k - tempCount)) continue;
                // TODO: UIHHHH FIOX PLSZ
                EffectNext nextEffect = k < tempCount ? tempEffectNextStack[k] : EffectNextStack[k - tempCount];
                if (nextEffect.AppliesTo == NextEffectAppliesTo.NextCardDrawn) continue;
                if (nextEffect.RequiredType == action.Type && nextEffect.RequiredPosition.HasFlag(player.Position))
                {
                    var effects = nextEffect.GetEffects();
                    //Debug.Log($"Adjust {action.Name} by effectNextStack: {effects.Cost} | {effects.HypeGain}");
                    adjustCost += effects.Cost;
                    adjustHype += effects.HypeGain;
                    if (played && k < tempCount) removeTempIndexes.Add(k);
                    else if (played) skipActualStackIndexes.Add(k - tempCount);
                }
                else if (nextEffect.AppliesTo == NextEffectAppliesTo.NextCardPlayed)
                {
                    if (played && k < tempCount) removeTempIndexes.Add(k);
                }
            }
            removeTempIndexes.Sort();
            for (int index = removeTempIndexes.Count - 1; index >= 0; index--)
            {
                tempEffectNextStack.RemoveAt(removeTempIndexes[index]);
            }

            if (action.HasEffectIfPrevious && i > 0 && prevPlayer != null)
            {
                var previousAction = prevPlayer.CardData.GetAction(prevActionIndex);
                if (action.UseEffectIfPrevious(previousAction.Type, prevPlayer.Position, out var effectIfPrevious))
                {
                    //Debug.Log($"Adjust {action.Name} by effectIfPrevious: {effectIfPrevious.Cost} | {effectIfPrevious.HypeGain}");
                    adjustCost += effectIfPrevious.Cost;
                    adjustHype += effectIfPrevious.HypeGain;
                }
            }

            if (action.HasEffectIfSequence)
            {
                int seqIfCount = action.EffectIfSequence.Requirements switch
                {
                    SequenceRequirements.First => i == 0 ? 1 : 0,
                    SequenceRequirements.Last => i == TimelineActions.Count - 1 ? 1 : 0,
                    SequenceRequirements.NoTypePlayed => !TimelineActions.Any(o => o.Player.CardData.GetAction(o.ActionIndex).Type == action.EffectIfSequence.OfType) ? 1 : 0,
                    SequenceRequirements.ForEachOfType => TimelineActions.Count(o => o.Player.CardData.GetAction(o.ActionIndex).Type == action.EffectIfSequence.OfType),
                    _ => 0
                };
                if (seqIfCount > 0)
                {
                    var effectIfSequence = action.EffectIfSequence.Effects.GetEffects(action.ActionLevel);
                    for (int k = 0; k < seqIfCount; k++)
                    {
                        //Debug.Log($"Adjust {action.Name} by effectIfSequence: {effectIfSequence.Cost} | {effectIfSequence.HypeGain}");
                        adjustCost += effectIfSequence.Cost;
                        adjustHype += effectIfSequence.HypeGain;
                    }
                }
            }
            if (played && action.HasNextEffect)
            {
                tempEffectNextStack.Add(action.NextEffect);
            }

            if (played)
            {
                sequenceCost += adjustCost;
                prevPlayer = player;
                prevActionIndex = actionIndex;
            }
            else
            {
                // TODO: Highlight cards
                //Debug.Log($"For {action.Name}: {sequenceCost} + {player.CardData.GetAction(actionIndex).Effects.Cost} + {adjustCost} out of {_maxEnergy}");
                bool locked = _maxEnergy < sequenceCost + player.CardData.GetAction(actionIndex).Effects.Cost + adjustCost;
                cards[i - TimelineActions.Count].SetLocked(locked);
                if (!locked)
                {
                    cards[i - TimelineActions.Count].RefreshVisuals(adjustCost, adjustHype);
                }
            }
        }
        for (; j < _actionVisualPreviews.Count; j++)
        {
            _actionVisualPreviews[j].gameObject.SetActive(false);
        }

        _energyRemaining = _maxEnergy - sequenceCost;
        UpdateEnergy?.Invoke(sequenceCost, _maxEnergy);
    }

    public bool RunSimulation()
    {
        if (_simulating/* || !IsSequenceValid()*/) return false;
        _simulating = true;
        foreach (var preview in _actionVisualPreviews)
        {
            preview.gameObject.SetActive(false);
        }
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
            ApplyActionEffects(playerWithBall, action, i);
            if (action.Type == ActionType.Pass)
            {
                var passToPlayer = (i + 1) < TimelineActions.Count ? TimelineActions[i + 1].Player : playerWithBall;
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

    private void ApplyActionEffects(Player player, ActionData action, int timelineIndex)
    {
        Debug.Log($"Play Action: {action.ActionSummary}");
        var effects = action.Effects;

        for (int i = EffectNextStack.Count - 1; i >= 0; i--)
        {
            EffectNext nextEffect = EffectNextStack[i];
            // TODO: Check and apply and delete any matching next effects
            if (nextEffect.AppliesTo == NextEffectAppliesTo.NextCardDrawn) continue;
            if (nextEffect.RequiredType == action.Type && nextEffect.RequiredPosition.HasFlag(player.Position))
            {
                Debug.Log("Bonus: Next Effect applies to played card!");
                effects += nextEffect.GetEffects();
                EffectNextStack.RemoveAt(i);
                UpdateEffectNextStack?.Invoke();
            }
            else if (nextEffect.AppliesTo == NextEffectAppliesTo.NextCardPlayed)
            {
                Debug.Log("Next effect did not match. Removing from stack.");
                EffectNextStack.RemoveAt(i);
                UpdateEffectNextStack?.Invoke();
            }
        }

        if (action.HasEffectIfPrevious && timelineIndex > 0)
        {
            var previous = TimelineActions[timelineIndex - 1];
            var previousAction = previous.Player.CardData.GetAction(previous.ActionIndex);
            if (action.UseEffectIfPrevious(previousAction.Type, previous.Player.Position, out var effectIfPrevious))
            {
                Debug.Log("Bonus: Effect if Previous is Applied!");
                effects += effectIfPrevious;
            }
        }

        if (action.HasEffectIfSequence)
        {
            int count = action.EffectIfSequence.Requirements switch
            {
                SequenceRequirements.First => timelineIndex == 0 ? 1 : 0,
                SequenceRequirements.Last => timelineIndex == TimelineActions.Count - 1 ? 1 : 0,
                SequenceRequirements.NoTypePlayed => !TimelineActions.Any(o => o.Player.CardData.GetAction(o.ActionIndex).Type == action.EffectIfSequence.OfType) ? 1 : 0,
                SequenceRequirements.ForEachOfType => TimelineActions.Count(o => o.Player.CardData.GetAction(o.ActionIndex).Type == action.EffectIfSequence.OfType),
                _ => 0
            };
            if (count > 0)
            {
                Debug.Log($"Bonus: Effect if sequence is applied{(count > 1 ? count + " times!" : "!")}");
                var effectIfSequence = action.EffectIfSequence.Effects.GetEffects(action.ActionLevel);
                for (int i = 0; i < count; i++)
                {
                    effects += effectIfSequence;
                }
            }
        }

        Hype += effects.HypeGain;
        // TODO: Apply other effects

        if (action.HasNextEffect)
        {
            EffectNextStack.Add(action.NextEffect);
            UpdateEffectNextStack?.Invoke();
        }

        // Action visual effects
        player.SetActionText(action.Name, action.Duration);

        // Update visuals
        UpdateHype?.Invoke(Hype);
        _crowdController.SetHype(Hype / 100f);
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
        // TODO: Update costs
        _energyRemaining = _maxEnergy;
        UpdateEnergy?.Invoke(0, _maxEnergy);
        _trickshotCamera.SetNormalCamera();
        foreach (var player in _players)
        {
            player.FacePosition(player.transform.position + new Vector3(0, 0, 1f), 1f);
            player.SetAnimation(PlayerAnimation.Idle);
        }
        for (int i = EffectNextStack.Count - 1; i >= 0; i--)
        {
            switch (EffectNextStack[i].AppliesTo)
            {
                case NextEffectAppliesTo.NextMatchingCardThisGame:
                    // Save for next round
                    break;
                case NextEffectAppliesTo.NextCardDrawn:
                    // TODO: Send to action deck manager
                    EffectNextStack.RemoveAt(i);
                    break;
                default:
                    EffectNextStack.RemoveAt(i);
                    break;
            }
        }
        UpdateEffectNextStack?.Invoke();
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
