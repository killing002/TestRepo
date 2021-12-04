using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasProcces : MonoBehaviour
{
    [SerializeField] private Image _aimImage;
    [SerializeField] private Text _findText;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Image _backGroundImage;

    public Button RestartButton => _restartButton;
    public Image BackGroundImage => _backGroundImage;
    public Image AimImage => _aimImage;
    public Text FindText => _findText;

    public void FadeOut(Image image)
    {
        image.DOFade(1, 1);
    }

    public void FadeOut(Text text)
    {
        text.DOFade(1, 1);
    }

    public void SetAimSprite(Sprite value)
    {
        if (value == null)
            return;

        _aimImage.sprite = value;
    }

    public void OnRestartClick()
    {
        SceneManager.LoadScene(0);
    }
}
