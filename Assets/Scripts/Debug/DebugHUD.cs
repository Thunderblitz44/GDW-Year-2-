using TMPro;
using UnityEngine;

public class DebugHUD : MonoBehaviour
{
    public static DebugHUD instance;

    [SerializeField] TextMeshProUGUI joinCodetxt;
    [SerializeField] TextMeshProUGUI speedTxt;
    [SerializeField] TextMeshProUGUI airtimeTxt;
    [SerializeField] TextMeshProUGUI messageTxt;

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

    public void SetJoinCode(string value)
    {
        if (!joinCodetxt) return;
        joinCodetxt.text = $"Join Code : {value}";
    }

    void ResetAirtime()
    {
        if (!airtimeTxt) return;
        airtimeTxt.text = $"Air time : 0.000s";
    }

    public void SetMessage(string message)
    {
        if (!messageTxt) return;

        messageTxt.text = message;
    }
}
