using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;

public class Quad : MonoBehaviour
{
    private SpriteRenderer _childImage;
    private SpriteRenderer _myImage;

    private bool _isTrueQuad = false;
    private bool _isTouchable = true;
    public UnityEvent onEndBounceScaleAnimation_e;
    public UnityEvent onMouseDown_e;


    public SpriteRenderer ChildImage => _childImage;
    public bool IsTouchable => _isTouchable;
    public bool IsTrueQuad => _isTrueQuad;

    public void Awake()
    {
        _myImage = GetComponent<SpriteRenderer>();
        _childImage = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public void SetTrueQuad(bool value) => _isTrueQuad = value;

    public void SetTouchableQuad(bool value) => _isTouchable = value;

    public void SetData(Color color, Sprite image)
    {
        //set color
        _myImage.color = color;

        //set image
        _childImage.sprite = image;
    }

    private void ShakeAnimation()
    {
        _childImage.transform.DOShakePosition(1, new Vector3(0.2f, 0, 0), 10, 0, false, true);
    }

    public void BounceScaleAnimation(bool setCallback = false)
    {
       var t = transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.6f, 2, 5f);
        if (setCallback)
        {
            _isTouchable = false;
            t.onComplete += () =>
            {
                onEndBounceScaleAnimation_e?.Invoke();
            };
        }
    }

    public void ClearEvents()
    {
        onMouseDown_e.RemoveAllListeners();
        onEndBounceScaleAnimation_e.RemoveAllListeners();
    }

    private void OnMouseDown()
    {
        if (!_isTouchable)
            return;
        Debug.Log("I was clicked");

        onMouseDown_e?.Invoke();

        if (_isTrueQuad)
            BounceScaleAnimation(true);
        else
            ShakeAnimation();
    }


    private void OnDestroy()
    {
        ClearEvents();
    }
}
