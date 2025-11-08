using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class GammaSettingBehavior : MonoBehaviour
{

    public Slider gammaSlider;

    public PostProcessProfile brightness;
    public PostProcessLayer layer;

    AutoExposure exposure;
    void Start()
    {
        brightness.TryGetSettings(out exposure);
        AdjustBrightness(gammaSlider.value);
    }

    public void AdjustBrightness(float value)
    {
        if (value >= 0.05f)
        {
            exposure.keyValue.value = value;
        }
        else
        {
            exposure.keyValue.value = .05f;
        }
    }
}

/*
    SETUP:

        Camera: Add Post Processing Layer Component
        Create new object layer: Post processing
        Set Post Processing Layer to Post Processing

        Create new Brightness Object
        Set layer of object to post processing
        Add Post-Processing Volume
        Click new next to profile (On Volume Component)
        Add new effect > Unity > Auto Exposure (On Volume Component)
        
        Added this script to brightness
        Assign Slider to slider
        Assign your post process profile to profile (Should be in scenes folder under Scenename_profiles)
        Assign Layer to be your Main camera

        On slider:
        On slider change, run gammaSettingBehavior > AdjustBrightness (Under Dyanmic Float)

        Lastly on your post process profile, set progressive > fix, so it adjusts instantly

    -- Extra: (For Brightness on UI) --

        Create new camera, Name it "UI Camera" and place it under camera
        Settings:
        Clear Flags: Depth Only
        Culling Mask: Nothing, only UI checked
        Depth: 1 (Or +1 above Main Camera's Depth)
        Components:
        Add Post-Processing Layer
        Set Layer to Post Processing
        On MAIN camera, remove UI from culling mask
        On you canvas set Remder mode to screen space - Camera,
        then set your render camera to UI Camera

        Video Tutorial: https://www.youtube.com/watch?v=XiJ-kb-NvV4

*/
