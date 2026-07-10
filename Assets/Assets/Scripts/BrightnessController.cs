using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class BrightnessController : MonoBehaviour
{
    [Header("Volume Reference")]
    [SerializeField] private Volume globalVolume;

    [Header("UI Slider References")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Slider saturationSlider;
    [SerializeField] private Slider gammaSlider;

    private ColorAdjustments colorAdjustments;
    private LiftGammaGain liftGammaGain;

    private void Start()
    {
        if (globalVolume == null)
        {
            Debug.LogError("Global Volume not assigned in BrightnessController.");
            return;
        }

        if (globalVolume.profile == null)
        {
            Debug.LogError("No Volume Profile assigned on the Volume.");
            return;
        }

        if (!globalVolume.profile.TryGet(out colorAdjustments))
        {
            Debug.LogError("No ColorAdjustments override found in Volume Profile.");
            return;
        }

        if (!globalVolume.profile.TryGet(out liftGammaGain))
        {
            Debug.LogError("No LiftGammaGain override found in Volume Profile. Add 'Lift, Gamma, Gain' to the Volume Profile.");
            return;
        }

        if (brightnessSlider != null)
        {
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
            brightnessSlider.value = colorAdjustments.postExposure.value;
        }

        if (saturationSlider != null)
        {
            saturationSlider.onValueChanged.AddListener(SetSaturation);
            saturationSlider.value = colorAdjustments.saturation.value;
        }

        if (gammaSlider != null)
        {
            gammaSlider.onValueChanged.AddListener(SetGamma);
            gammaSlider.value = liftGammaGain.gamma.value.w;
        }
    }

    public void SetBrightness(float value)
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.value = value;
        }
    }

    public void SetSaturation(float value)
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = value;
        }
    }

    public void SetGamma(float value)
    {
        if (liftGammaGain != null)
        {
            Vector4 gammaValue = liftGammaGain.gamma.value;
            gammaValue.w = value;
            liftGammaGain.gamma.value = gammaValue;
        }
    }
}