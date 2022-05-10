using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tz_UI_barController : MonoBehaviour
{
    public bool isSetup { get; private set; }
    public bool setupOnAwake = true;

    [Space]
    public Image barImage;
    public Gradient barColorGradient;

    [Space]
    [SerializeField]
    public FillableBar fillableBar;

    private const float presetThreshold = -0.001f;
    private void Awake()
    {
        if (setupOnAwake) Setup();
    }

    public void Setup()
    {
        if (isSetup) return;
        isSetup = true;

        int preset_MaxFill = fillableBar.MAX_FILL > -1 ? fillableBar.MAX_FILL : 100;
        float preset_fillAmount = fillableBar.fillAmount > presetThreshold ? fillableBar.fillAmount : 0;
        float preset_fillRegenAmount = fillableBar.fillRegenAmount > presetThreshold ? fillableBar.fillRegenAmount : 10f;
        float preset_flashThreshold = fillableBar.flashThreshold > presetThreshold ? fillableBar.flashThreshold : 0.3f;

        fillableBar = new FillableBar(preset_MaxFill, preset_fillAmount, preset_fillRegenAmount);

        barImage.fillAmount = fillableBar.GetNormalized();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isSetup) return;

        fillableBar.Update();

        barImage.fillAmount = fillableBar.GetNormalized();

        BarColorUpdate();
    }

    private void BarColorUpdate()
    {
        float currentFill = fillableBar.GetNormalized();
        Color currentBarColor = barColorGradient.Evaluate(currentFill);

        if (currentFill < fillableBar.flashThreshold
        && (int)(currentFill * 100F) % 3 == 0)
        {
            Color barColorFlash = Color.Lerp(currentBarColor, Color.black, 0.25f);
            barImage.color = barColorFlash;
        }
        else
        {
            barImage.color = currentBarColor;
        }
    }

    public void TrySpendFill(int amountToSpend)
    {
        fillableBar.TrySpendFill(amountToSpend);
    }

    public void AdjustMAXFILL(int newMaxAmount)
    {
        fillableBar.AdjustMAXFILL(newMaxAmount);
    }

    public void AllowFillRegen(bool onOff)
    {
        fillableBar.AllowFillRegen(onOff);
    }

    public void AdjustFillRegen(float newRegenAmount)
    {
        fillableBar.AdjustFillRegen(newRegenAmount);
    }

    public void AdjustFlashThreshold(float newFlashThreshold)
    {
        fillableBar.AdjustFlashThreshold(newFlashThreshold);
    }

    public float GetNormalized()
    {
        return fillableBar.GetNormalized();
    }

    private void OnValidate()
    {

        if (Application.isPlaying) return;


        if (fillableBar.fillAmount != -0.001f)
        {
            barImage.fillAmount = fillableBar.fillAmount;
            barImage.color = barColorGradient.Evaluate(barImage.fillAmount);
        }
        else
        {
            barImage.fillAmount = 1;
            barImage.color = barColorGradient.Evaluate(1);
        }

    }
}

[System.Serializable]
public class FillableBar
{
    [Tooltip("Default -1 = 100")]
    [Min(-1f)]
    public int MAX_FILL = -1;

    [Tooltip("Default -0.001 = 0")]
    [Range(-0.001f, 1.0f)]
    public float fillAmount = -0.001f;

    public bool canRegenerate = true;

    [Tooltip("Default -0.001 = 10")]
    [Min(-0.001f)]
    public float fillRegenAmount = -0.001f;

    [Tooltip("Default -0.001 = .3")]
    [Range(-0.001f, 1.0f)]
    public float flashThreshold = -0.001f;


    public FillableBar(
        int starting_MAX_FILL = 100,
        float starting_fillAmount = 0,
        float starting_fillRegenAmount = 10f,
        float starting_flashThreshold = 0.3f,
        bool starting_canRegenerate = true
    )
    {
        MAX_FILL = starting_MAX_FILL;
        fillAmount = starting_fillAmount;
        fillRegenAmount = starting_fillRegenAmount;
        flashThreshold = starting_flashThreshold;

        canRegenerate = starting_canRegenerate;
    }

    public void Update()
    {
        if (canRegenerate) fillAmount += fillRegenAmount * Time.deltaTime;

        fillAmount = Mathf.Clamp(fillAmount, 0f, MAX_FILL);
    }

    public void TrySpendFill(int amountToSpend)
    {
        if (fillAmount >= amountToSpend)
        {
            fillAmount -= amountToSpend;
        }
    }

    public float GetNormalized()
    {
        return fillAmount / MAX_FILL;
    }

    public void AdjustMAXFILL(int newMaxAmount)
    {
        MAX_FILL = newMaxAmount;
    }

    public void AdjustFillRegen(float newRegenAmount)
    {
        fillRegenAmount = newRegenAmount;
    }

    public void AdjustFlashThreshold(float newFlashThreshold)
    {
        flashThreshold = newFlashThreshold;
    }

    public void AllowFillRegen(bool onOff)
    {
        canRegenerate = onOff;
    }
}
