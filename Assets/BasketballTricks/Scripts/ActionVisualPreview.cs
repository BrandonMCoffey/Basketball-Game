using System.Collections.Generic;
using UnityEngine;

public class ActionVisualPreview : MonoBehaviour
{
    [SerializeField] private GameObject _trickVisuals;

    [SerializeField] private GameObject _passVisuals;
    [SerializeField] private SpriteRenderer _passSpriteRenderer;

    [SerializeField] private GameObject _shotVisuals;
    [SerializeField] private LineRenderer _shotLineRenderer;
    [SerializeField] private int _lineSegments = 20;

    public void ShowType(int type)
    {
        gameObject.SetActive(true);
        _trickVisuals.SetActive(type == 0);
        _passVisuals.SetActive(type == 1);
        _shotVisuals.SetActive(type == 2);
    }

    public void ShowTrick(Vector3 playerPos)
    {
        ShowType(0);
        playerPos.y = 0f;
        transform.SetPositionAndRotation(playerPos, Quaternion.identity);
    }

    public void ShowPass(Vector3 fromPlayer, Vector3 toPlayer, Color color, bool extraCost)
    {
        ShowType(1);
        fromPlayer.y = toPlayer.y = 0f;
        transform.position = fromPlayer;
        transform.LookAt(toPlayer);
        float dist = Vector3.Distance(fromPlayer, toPlayer);
        _passSpriteRenderer.size = new Vector2(dist, _passSpriteRenderer.size.y);
        _passSpriteRenderer.transform.localPosition = new Vector3(0f, _passSpriteRenderer.transform.localPosition.y, dist * 0.5f);
        _passSpriteRenderer.color = color;
    }

    public void ShowShot(Vector3 fromPlayer, Vector3 toNet, Color color)
    {
        ShowType(2);
        transform.position = fromPlayer;
        transform.LookAt(toNet);
        var positions = new List<Vector3>(_lineSegments);
        for (int i = 0; i < _lineSegments; i++)
        {
            float delta = (float)(i + 1) / (_lineSegments + 1);
            var pos = Vector3.Lerp(fromPlayer, toNet, delta);
            pos.y = fromPlayer.y + (toNet.y - fromPlayer.y) * ShotCurve(delta);
            positions.Add(pos);
        }
        _shotLineRenderer.positionCount = positions.Count;
        _shotLineRenderer.SetPositions(positions.ToArray());
        var gradient = new Gradient();
        gradient.colorKeys = new GradientColorKey[] { new GradientColorKey(color, 0) };
        _shotLineRenderer.colorGradient = gradient;
    }

    private float ShotCurve(float x)
    {
        return 1.33333f * (1 - Mathf.Pow(1 - x * 1.5f, 2));
    }
}
