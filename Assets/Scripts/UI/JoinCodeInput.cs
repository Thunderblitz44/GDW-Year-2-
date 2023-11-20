using TMPro;
using UnityEngine;

public class JoinCodeInput : MonoBehaviour
{
    //[SerializeField] Relay relay;

    private void Start()
    {
        TMP_InputField input = GetComponent<TMP_InputField>();
        //input.onEndEdit.AddListener((string s) => { relay.JoinRelay(input.text); });
    }
}
