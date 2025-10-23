using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool InTransition { get; private set; }
    public static event System.Action UpdateActivePlayers = delegate { };

    [SerializeField] private List<PlayerData> _players;
    [SerializeField] private CanvasGroup _sceneTransition;

    private int _selectedPlayerIndex = -1;

    public List<PlayerData> Players => _players;
    public PlayerData ActivePlayer => (_selectedPlayerIndex >= 0 && _selectedPlayerIndex < _players.Count) ? _players[_selectedPlayerIndex] : null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(TransitionToSceneRoutine("", true));
    }

    public void StartGame()
    {
        TransitionToScene("TrickPlays");
    }

    public void LoadCatalog()
    {
        TransitionToScene("CardCatalog");
    }

    public void StartDribblePractice(int playerIndex)
    {
        Debug.Log($"Starting dribble practice with {_players[playerIndex].PlayerName}");
        _selectedPlayerIndex = playerIndex;
        TransitionToScene("DribbleGame");
    }

    public void StartShootingPractice(int playerIndex)
    {
        Debug.Log($"Starting shooting practice with {_players[playerIndex].PlayerName}");
        _selectedPlayerIndex = playerIndex;
        TransitionToScene("ShootingGame");
    }

    private void TransitionToScene(string newScene)
    {
        if (!InTransition)
        {
            StartCoroutine(TransitionToSceneRoutine(newScene));
        }
        else
        {
            Debug.LogWarning($"Scene transition already in progress. Cannot load {newScene}");
        }
    }
    private IEnumerator TransitionToSceneRoutine(string newScene, bool startup = false)
    {
        InTransition = true;
        if (!startup)
        {
            for (float t = 0; t < 1f; t += Time.deltaTime * 2f)
            {
                _sceneTransition.alpha = EaseInOutQuart(t);
                yield return null;
            }
            SceneManager.LoadScene(newScene);
        }
        for (float t = 0; t < 1f; t += Time.deltaTime * 2f)
        {
            _sceneTransition.alpha = 1 - EaseInOutQuart(t);
            yield return null;
        }
        InTransition = false;
    }
    public static float EaseInOutQuart(float x)
    {
        return x < 0.5 ? 8 * x * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 4) / 2;
    }
}