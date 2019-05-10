using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using System.Threading;

public class Button_script : MonoBehaviour //, IPointerEnterHandler
{
    public VideoClip inladen;
    public VideoClip uitsnijden;
    public VideoClip error;
    public Texture2D firstFrame;
    public VideoPlayer vp;

    public GameObject[] panels; // [0] = main menu, [1] = videoplayer
    
    public void OnClick(Button b)
    {
        bool playbool = true;
        switch (b.name)
        {
            case "Button inladen":
                vp.clip = inladen;
                break;
            case "Button uitsnijden":
                vp.clip = uitsnijden;
                break;
            case "Button error":
                vp.clip = error;
                break;
            case "Back":
                playbool = false;
                panels[0].SetActive(true);
                panels[1].SetActive(false);
                break;
            default:
                break;
        }
        
        if (playbool)
        {
            Debug.Log("playbool= true");
            
            vp.loopPointReached += EndReached;
            panels[0].SetActive(false);
            panels[1].SetActive(true);
            vp.Pause();
            vp.frame = 0;
        }
        
        
    }

    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        // Reset video to first frame
        vp.frame = 0;
    }

    /*public void OnPointerEnter(PointerEventData eventData)
    {
        string bName = "";
        if (bName == (eventData.pointerEnter.GetComponentInParent<Button>().name))
        {
            bName = (eventData.pointerEnter.GetComponentInParent<Button>().name);
            Debug.Log(bName);
           

            ;
        }
    }*/
}
