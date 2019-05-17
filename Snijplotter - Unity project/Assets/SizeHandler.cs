using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SizeHandler : MonoBehaviour
{
    public Camera camera;
    private VideoPlayer vp;
    public int pixelsToUnityUnits = 100;
    private Transform obj;
    public bool followCamera = true;

    // Start is called before the first frame update
    void Start()
    {
        obj = transform;
        obj.GetComponent<Transform>();
        vp.GetComponent<VideoPlayer>();
        camera = (camera != null) ? camera : Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        float aspectRatio = Screen.width / (float)Screen.height; // in respect to width

        // Guesstimation
        int orthoPixelsY = Mathf.CeilToInt(camera.orthographicSize * 2 * pixelsToUnityUnits);
        int orthoPixelsX = Mathf.CeilToInt(orthoPixelsY * aspectRatio);

        // orthoX and orthoY are obviously not the same as the screen resolution, but we can take advantage of the fact that they're scaled based
        // on unity units and ignore calculating based on screen pixels

        // find the ratio in scales along each respective axis' and assign as the new local scale
        Vector3 newScale = new Vector3(orthoPixelsX / (float)vp.width, orthoPixelsY / (float)vp.height, 1f);
        obj.localScale = newScale;

        if (followCamera)
        {
            Vector3 camPosition = camera.transform.position;
            Vector3 newPosition = new Vector3(camPosition.x, camPosition.y, obj.position.z);
            obj.position = newPosition;
        }
    }
}
