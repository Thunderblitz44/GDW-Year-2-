using TMPro;
using UnityEngine;

public class DebugHUD : MonoBehaviour
{
    public static DebugHUD instance;

    [SerializeField] TextMeshProUGUI speedTxt;
    [SerializeField] TextMeshProUGUI airtimeTxt;
    [SerializeField] TextMeshProUGUI timerTxt;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public void SetSpeed(float value)
    {
        if (!speedTxt) return;
        speedTxt.text = $"Speed : {value.ToString("0.00")}u/s";
    }

    public void SetAirTime(float value)
    {
        if (!airtimeTxt) return;
        airtimeTxt.text = $"Air time : {value.ToString("0.000")}s";
        CancelInvoke(nameof(ResetAirtime));
        Invoke(nameof(ResetAirtime), 2f);
    }

    void ResetAirtime()
    {
        if (!airtimeTxt) return;
        airtimeTxt.text = $"Air time : 0.000s";
    }

    public void SetTimer(float value)
    {
        if (!timerTxt) return;
        timerTxt.text = $"Timer : {value.ToString("0.000")}s";
    }
}
