using UnityEngine;
using UnityEngine.XR;
public class switcher : MonoBehaviour
{
    [SerializeField] private GameObject XRplayer;
    [SerializeField] private GameObject FPSplayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Hmd + " + XRSettings.loadedDeviceName);
        if (XRSettings.isDeviceActive || XRSettings.loadedDeviceName == "OpenXR Display")
        {
            Debug.Log("Using Device XR Player with HMD + " +XRSettings.loadedDeviceName);
            XRplayer.SetActive(true);
            FPSplayer.SetActive(false);
        }
        else
        {
            Debug.Log("NO HMD DETECTED USING FPS PLAYER+ ");
            XRplayer.SetActive(false);
            FPSplayer.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
