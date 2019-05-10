using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayer_script : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField]
    private VideoPlayer vp;
    public  GameObject playImg, pauseImg;
    private SpriteRenderer playImgRend, pauseImgRend; 

    public SVGImage handle;
    
    private Image progress;
    Vector2 localPoint;
    private bool pauseOrPlayBool = false, showPause = false, showPlay = false;
    static float t = 0.0f;
    //PointerEventData ped;
    
    // Start is called before the first frame update
    void Start()
    {
        progress = GetComponent<Image>();
        vp.Pause();
        playImgRend = playImg.GetComponent<SpriteRenderer>();
        pauseImgRend = pauseImg.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (vp.frameCount > 0)
        {
            progress.fillAmount = (float)vp.frame / (float)vp.frameCount;
            handle.rectTransform.anchoredPosition = new Vector2((-progress.rectTransform.rect.width) / 2 + ((float)vp.frame / (float)vp.frameCount) * progress.rectTransform.rect.width, handle.rectTransform.anchoredPosition.y);
        }

        if (pauseOrPlayBool)
        {
            if (showPause)
            {
                t = 1f;
                showPause = false;
            }
            if (vp.isPaused)
            {
                playImgRend.color = new Color(1, 1, 1, 0);
                pauseImgRend.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, t));
                if (t > 0)
                {
                    t -= 0.95f * Time.deltaTime;
                }
            }

            if (vp.isPlaying)
            {
                pauseImgRend.color = new Color(1, 1, 1, 0);
                playImgRend.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, t));
                if (t > 0)
                {
                    t -= 0.95f * Time.deltaTime;
                }
            }

        }

        

    }

    public void OnDrag(PointerEventData eventData)
    {
        TrySkip(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        TrySkip(eventData);
    }

    private void TrySkip(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(progress.rectTransform,eventData.position, null, out localPoint))
        {
            float pct = Mathf.InverseLerp(progress.rectTransform.rect.xMin, progress.rectTransform.rect.xMax, localPoint.x);
            SkipToPercent(pct);
        }
    }

    private void SkipToPercent(float pct)
    {
        var frame = vp.frameCount * pct;
        vp.frame = (long)frame;
    }

    public void Playbutton()
    {
        if (vp.isPlaying)
        {
            vp.Pause();
        }
        else
        {
            vp.Play();
        }
        pauseOrPlayBool = true;
        showPause = true;
    }


}
