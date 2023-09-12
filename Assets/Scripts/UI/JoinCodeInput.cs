using TMPro;
using UnityEngine;

public class JoinCodeInput : MonoBehaviour
{
    [SerializeField] Relay relay;
    [SerializeField] TMP_InputField input;

    private void Start()
    {
        input.onEndEdit.AddListener((string s) => { relay.JoinRelay(input.text); });
    }
}
