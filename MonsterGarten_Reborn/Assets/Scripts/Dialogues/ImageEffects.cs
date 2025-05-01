using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageEffects : MonoBehaviour
{
    Color fadedColor;
    float t;
    [Range(0, 3)] [SerializeField] float _TimeFadeOut = 1f;
    [Range(0, 3)] [SerializeField] float _TimeFadeIn = 2f;
    private void Update()
    {
        if (t < 1) t += Time.deltaTime;

    }
    public void SwitchPosition(Image _img1, Image _img2)
    {
        fadedColor = _img2.color;
        Sprite HeadSprite = _img1.sprite;
        Sprite FollowerSprite = _img2.sprite;
        StartCoroutine(Move(_img2, _img1, HeadSprite, FollowerSprite, _TimeFadeOut , _TimeFadeIn));


    }

    void SynthFloats(Image _imageToTranspose, float _time, bool _Out)
    {
        float r = _imageToTranspose.color.r;
        float g = _imageToTranspose.color.g;
        float b = _imageToTranspose.color.b;
        float a = _imageToTranspose.color.a;
        if (_Out)
        {
            float r1 = Mathf.Lerp(r, 0f, _time);
            float g1 = Mathf.Lerp(g, 0f, _time);
            float b1 = Mathf.Lerp(b, 0f, _time);
            float a1 = Mathf.Lerp(a, 0f, _time);
            _imageToTranspose.color = new Vector4(r1, g1, b1, a1);
        }
        if (!_Out)
        {
            float r1 = Mathf.Lerp(r, fadedColor.r, _time);
            float g1 = Mathf.Lerp(g, fadedColor.g, _time);
            float b1 = Mathf.Lerp(b, fadedColor.b, _time);
            float a1 = Mathf.Lerp(a, fadedColor.a, _time);

            float r2 = Mathf.Lerp(0, 255, _time);
            float g2 = Mathf.Lerp(0, 255, _time);
            float b2 = Mathf.Lerp(0, 255, _time);
            float a2 = Mathf.Lerp(0, 255, _time);
            _imageToTranspose.color = new Vector4(r2, g2, b2, a2);
        }
    }
    IEnumerator Move(Image _imageFollower, Image _imageLead, Sprite Lead, Sprite Follower, float timeFadeOut , float TimeFadeIn)
    {
        _imageLead.color = new Vector4(0, 0, 0, 0);
        float qt = 0;
        // disparition
        while (qt < timeFadeOut)
        {
            qt += Time.deltaTime;
            #region Follower
            SynthFloats(_imageFollower, qt, true);
            #endregion

            #region Lead
            SynthFloats(_imageLead, qt, true);
            #endregion

            yield return null;
        }

        _imageFollower.sprite = Lead;
        _imageLead.sprite = Follower;


        // apparition
        float qi = 0;
        while (qi < TimeFadeIn)
        {
            qi += Time.deltaTime;
            #region Follower
            SynthFloats(_imageFollower, qi, false);
            #endregion

            #region Lead
            SynthFloats(_imageLead, qi, false);
            #endregion
            yield return null;
        }
  
    }
}
