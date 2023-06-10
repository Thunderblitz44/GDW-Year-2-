using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] Image cursor;
    [SerializeField] GameObject[] textAreas;
    [SerializeField] GameObject exampleTextBox;

    public enum TextRegion
    {
        crosshair
    }

    public enum TextArea
    {
        /// <summary>
        /// Name of interactable
        /// </summary>
        textAboveCrosshair,
        /// <summary>
        /// Actions for interactable
        /// </summary>
        textBelowCrosshair,
        /// <summary>
        /// Inspection of interactable
        /// </summary>
        textRightOfCrosshair
    }


    private void Start()
    {
        SetTextRegionVisibility(TextRegion.crosshair, false);
    }


    // for simple text areas
    public void DisplayText(TextArea area, string text)
    {
        if (textAreas == null) return;

        TextMeshProUGUI textMesh;
        if (textAreas[(int)area].TryGetComponent(out textMesh))
        {
            textMesh.richText = true;
            textMesh.text = text;
        }
    }

    // for complex text areas. aka inspector panel
    public void DisplayText(TextArea area, Dictionary<string, string> data)
    {
        if (textAreas == null) return;

        //re building the inspection panel
        
        TextMeshProUGUI[] textMeshProUGUIs = new TextMeshProUGUI[data.Count * 2];

        for (int i = 0, n = 0; i < data.Count; i++, n += 2)
        {
            // label
            textMeshProUGUIs[n] = Instantiate(exampleTextBox, textAreas[(int)area].transform).GetComponent<TextMeshProUGUI>();
            textMeshProUGUIs[n].text = data.ElementAt(i).Key;

            // value
            textMeshProUGUIs[n+1] = Instantiate(exampleTextBox, textAreas[(int)area].transform).GetComponent<TextMeshProUGUI>();
            textMeshProUGUIs[n+1].text = data.ElementAt(i).Value;
        }

        // resize inspector window if valid
        ItemInspector itemInspector;
        if (textAreas[(int)area].TryGetComponent(out itemInspector))
        {
            itemInspector.ResizePanel();
        }
    }

    public void SetTextRegionVisibility(TextRegion region, bool isVisible)
    {
        if (textAreas == null) return;

        if (region == TextRegion.crosshair)
        {
            for (int i = 2; i >= 0; i--)
            {
                textAreas[i].SetActive(isVisible);
            }
        }
    }

    public void SetTextAreaVisibility(TextArea area, bool isVisible)
    {
        if (textAreas == null) return;

        textAreas[(int)area].SetActive(isVisible);
    }
}
