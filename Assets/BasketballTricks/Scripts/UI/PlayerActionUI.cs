using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionUI : MonoBehaviour
{
    [SerializeField] private SlideInPanel _playerActionPanel;
    [SerializeField] private PlayerCard _playerCardVisual;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private List<PlayerActionVisuals> _actionVisuals;
    [SerializeField] RectTransform _playerIconTransform;

    [SerializeField] RectTransform _glareEffect;

    private Player _player;
    private GameCard _playerData;
    
    private void Start()
    {
        PlayerShowAnims(GetComponent<SlideInPanel>().IsShown);
    }

    public void ShowPlayer(Player player)
    {
        _playerActionPanel.SetShown(true);
        _player = player;
        _playerData = player != null ? player.CardData : null;
        PlayerShowAnims(true);
        UpdateData();
    }

    public void Hide()
    {
        PlayerShowAnims(false);
        _playerActionPanel.SetShown(false);
    }

    private void UpdateData()
    {
        if (_playerCardVisual != null) _playerCardVisual.SetData(_playerData);
        for (int i = 0; i < _actionVisuals.Count; i++)
        {
            _actionVisuals[i].SetData(_playerData != null ? _playerData.GetAction(i) : new ActionData());
        }
        if (_backgroundImage != null && _player != null)
        {
            Color.RGBToHSV(_backgroundImage.color, out float h, out float s, out float v);
            Color.RGBToHSV(_player.PositionColor, out float ph, out float ps, out float pv);
            var color = Color.HSVToRGB(ph, s, v);
            color.a = _backgroundImage.color.a;
            _backgroundImage.color = color;
        }
    }

    public void SelectAction(int index)
    {
        // Check action if it is pass to make the user select who to pass to
        //PlayerManager.Instance.AddAction(_player, index);
    }

    Coroutine glareRoutine;
    public void PlayerShowAnims(bool enabled)
    {
        if (enabled)
        {
            _playerIconTransform.DOPunchScale(Vector3.one * 0.1f, 0.3f);
            if (glareRoutine != null) StopCoroutine(glareRoutine);
            glareRoutine = StartCoroutine(GlareRoutine());
        }
        else
        {
            if (glareRoutine != null) StopCoroutine(glareRoutine);
            _glareEffect.gameObject.SetActive(false);
        }
    }

    IEnumerator GlareRoutine()
    {
        _glareEffect.gameObject.SetActive(true);
        while (true)
        {
            _glareEffect.anchoredPosition = new Vector2(-Screen.width - 1024, 0);
            _glareEffect.DOAnchorPos(new Vector2(Screen.width + 1024, 0), 3f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(5f);
            _glareEffect.anchoredPosition = new Vector2(-Screen.width - 1024, 0);
        }
    }
}
