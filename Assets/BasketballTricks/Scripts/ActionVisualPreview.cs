using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionVisualPreview : MonoBehaviour
{
    [SerializeField] private GameObject _trickVisuals;

    [SerializeField] private GameObject _passVisuals;
    [SerializeField] private SpriteRenderer _passSpriteRenderer;

    [SerializeField] private GameObject _shotVisuals;
    [SerializeField] private SpriteRenderer _shotSpriteRenderer;

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

    public void ShowPass(Vector3 fromPlayer, Vector3 toPlayer, Color color)
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

    public void ShowShot(Vector3 fromPlayer, Vector3 toNet)
    {
        ShowType(2);
        fromPlayer.y = toNet.y = 0f;
        transform.position = fromPlayer;
        transform.LookAt(toNet);
        float dist = Vector3.Distance(fromPlayer, toNet);
        _shotSpriteRenderer.size = new Vector2(dist, _shotSpriteRenderer.size.y);
        _shotSpriteRenderer.transform.localPosition = new Vector3(0f, _shotSpriteRenderer.transform.localPosition.y, dist * 0.5f);
    }
}
