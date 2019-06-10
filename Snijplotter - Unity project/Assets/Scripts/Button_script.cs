using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System.Threading;
using System.IO;

public class Button_script : MonoBehaviour , IPointerEnterHandler
{
    public VideoClip inladen;
    public VideoClip uitsnijden;
    public VideoClip error;
    //public AudioSource audio_inladen, audio_uitsnijden, audio_error;
    public Texture2D inladenThumbnail, uitsnijdenThumbnail, errorThumbnail, firstFrame;
    public VideoPlayer vp;
    public GameObject playButton;
    private Image playButtonRend;

    public GameObject[] panels; // [0] = main menu, [1] = videoplayer


    //WEBGL FIX
    public VideoPlayer_script vp_script;
    public UnityWebRequest request;
    public GameObject progressBar;
    public Text videoDownloadProgressText;
    public Slider videoDownloadProgressSlider;
    private bool downloadAborted = false;
    private bool downloadError = false;
    private string temp_filePath, inladen_filePath, starten_filePath, error_filePath, sourcePath;

    public enum Animations
    {
        inladen,
        starten,
        error,
    }

    private void Start()
    {
        playButtonRend = playButton.GetComponent<Image>();

        //Check if application is running as a WEBGL build
        inladen_filePath = WebGLOverrideCheck(inladen.originalPath, inladen.name);
        starten_filePath = WebGLOverrideCheck(uitsnijden.originalPath, uitsnijden.name);
        error_filePath = WebGLOverrideCheck(error.originalPath, error.name);

        //download animation "inladen starten"
        //StartCoroutine(DownloadVideo(sourcePath, inladen.name));
    }
    public void closeApp()
    {
        Debug.Log("attempted to quit");
        Application.Quit();
    }

    public string WebGLOverrideCheck(string url, string filename) //returns the streamingAssets path for the given file when application is running as WEBGL build
    {
        Debug.Log("Checking WEBGL url override");
        if (string.IsNullOrEmpty(url) || !File.Exists(url))
        {
            Debug.Log("DETECTED WEBGL BUILD: Overriding url");
            url = System.IO.Path.Combine(Application.streamingAssetsPath, filename +".mp4");
            Debug.Log(url);
        }

        return url;
        
        //videoPlayer.SetTargetAudioSource(0, audioSource);
    }

    public void OnClick(Button b)
    {
        bool playbool = true;

        if (playbool) //activate panels and get vp_script to call WEBGLVideoOverride.
        {
            panels[0].SetActive(false);
            panels[1].SetActive(true);
            vp_script = GameObject.FindGameObjectWithTag("VideoplayerScriptHolder").GetComponent<VideoPlayer_script>();
        }

        switch (b.name)
        {
            case "Button inladen":

                vp.url = inladen_filePath;
                firstFrame = inladenThumbnail;
                //Debug.Log("name: " + inladen.name + "path: " + inladen.originalPath);
                //vp_script.WebGLVideoOverride(inladen.originalPath, inladen.name);
                break;
            case "Button uitsnijden":
                vp.url = starten_filePath;
                firstFrame = uitsnijdenThumbnail;
                break;
            case "Button error":
                vp.url = error_filePath;
                firstFrame = errorThumbnail;
                break;
            case "Back":
                playButtonRend.color = new Color(1, 1, 1, 1);
                playbool = false;
                panels[0].SetActive(true);
                panels[1].SetActive(false);
                break;
            default:
                Debug.Log("Undefined button clicked");
                break;
        }
        
        if (playbool)
        {
            Debug.Log("playbool= true");
            vp.targetTexture.DiscardContents();
            Graphics.Blit(firstFrame, vp.targetTexture);
            vp.waitForFirstFrame = true;
            vp.loopPointReached += EndReached;
            vp.Pause();
            vp.frame = 0;
        }
    }

    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        // Reset video to first frame
        vp.frame = 0;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log(eventData.pointerCurrentRaycast.gameObject.name);
        string bName = "";
        if (eventData.pointerCurrentRaycast.gameObject.tag == "videobutton")
        {
            bName = (eventData.pointerCurrentRaycast.gameObject.name);
            //Debug.Log("name:" + bName);
        }
    }


    public IEnumerator DownloadVideo(string videoURL, string videoName) //Coroutine to download video
    {
        downloadError = false;
        downloadAborted = false;
        request = new UnityWebRequest(videoURL, UnityWebRequest.kHttpVerbGET);
        //uwr.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
        inladen_filePath = Application.persistentDataPath + "/Videos/" + videoName + ".mp4";
        print("Downloading: " + inladen_filePath);
        request.downloadHandler = new DownloadHandlerFile(inladen_filePath);
        StartCoroutine(ShowDownloadProgress(request));
        //stopButton.SetActive(true);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError(request.error);
            downloadError = true;
        }
        else
            Debug.Log("File successfully downloaded and saved to " + inladen_filePath);
    }

    IEnumerator ShowDownloadProgress(UnityWebRequest uwr) //progressbar for downloading videos
    {
        while (!uwr.isDone)
        {
            progressBar.SetActive(true);
            Debug.Log(string.Format("Downloaded {0:P1}", uwr.downloadProgress));
            videoDownloadProgressText.text = string.Format("Downloading... {0:P1}", uwr.downloadProgress);
            videoDownloadProgressSlider.value = uwr.downloadProgress;
            yield return new WaitForSeconds(.1f);
        }
        if (uwr.isDone && downloadAborted && !downloadError)
        {
            print("Download progress = " + uwr.downloadProgress + ". Setting isDownloaded to true");
            videoDownloadProgressText.text = "Download complete.";
            Debug.Log("Done");
        }
        else
        {
            print("Download was aborted.");
            videoDownloadProgressText.text = "Download canceled.";
        }
    }
}
