using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;

public class VideoPlayer_script : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField]
    private VideoPlayer vp;
    public  GameObject playImg, pauseImg;
    private Image playImgRend, pauseImgRend; 

    public SVGImage handle;
    
    private Image progress;
    Vector2 localPoint;
    private bool pauseOrPlayBool = false, showPause = false, showPlay = false;
    static float t = 0.0f;

    AudioSource audioSource;
    private bool videoStarted;

    // Start is called before the first frame update
    void Start()
    {
        vp.Pause();
        progress = GetComponent<Image>();
        playImgRend = playImg.GetComponent<Image>();
        pauseImgRend = pauseImg.GetComponent<Image>();
        vp.skipOnDrop = true;
        pauseOrPlayBool = true;
        

        /*
        //<AUDIO FIX:>
        // We want to play from a URL.
        // Set the source mode FIRST, before changing audio settings;
        // as setting the source mode will reset those.
        vp.source = VideoSource.Url;

        // Set mode to Audio Source.
        vp.audioOutputMode = VideoAudioOutputMode.AudioSource;

        // We want to control one audio track with the video player
        vp.controlledAudioTrackCount = 1;

        // We enable the first track, which has the id zero
        vp.EnableAudioTrack(0, true);

        // ...and we set the audio source for this track
        vp.SetTargetAudioSource(0, audioSource);

        // now set an url to play
        vp.url = "...some url...";
        //<\AUDIO FIX>*/
    }

    void OnEnable()
    {
        pauseImgRend = pauseImg.GetComponent<Image>();
        pauseImgRend.color = new Color(1, 1, 1, 0);
        videoStarted = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!videoStarted)
        {
            vp.Play();
            videoStarted = true;
        }


        if (vp.frameCount > 0)
        {
            progress.fillAmount = (float)vp.frame / (float)vp.frameCount;
            handle.rectTransform.anchoredPosition = new Vector2((-progress.rectTransform.rect.width) / 2 + ((float)vp.frame / (float)vp.frameCount) * progress.rectTransform.rect.width, handle.rectTransform.anchoredPosition.y);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Playbutton();
        }

        if (pauseOrPlayBool)
        {
            if (showPause)
            {
                t = 1f;
                showPause = false;
            }

            if (vp.isPaused && vp.frame > 5)
            {
                playImgRend.color = new Color(1, 1, 1, 0);
                pauseImgRend.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, t));
                if (t > 0)
                {
                    t -= 2f * Time.deltaTime;
                }
            }

            if (vp.isPlaying)
            {
                pauseImgRend.color = new Color(1, 1, 1, 0);
                playImgRend.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, t));
                if (t > 0)
                {
                    t -= 2f * Time.deltaTime;
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("DRAGGING SCRUBBER");
        TrySkip(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("CLICKING SCRUBBER");
        TrySkip(eventData);
    }

    private void TrySkip(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(progress.rectTransform, eventData.position, null, out localPoint))
        {
            Debug.Log("rectTransform: " + progress.rectTransform.position + "eventdata: " + eventData.position);

            float pct = Mathf.InverseLerp(0, Screen.width, eventData.position.x);
            Debug.Log("Skipping to " + pct*100f + "%");
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
