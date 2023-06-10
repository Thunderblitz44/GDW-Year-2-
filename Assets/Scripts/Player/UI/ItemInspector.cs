using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInspector : MonoBehaviour
{
    GridLayoutGroup glg;
    RectTransform rt;

    private void Awake()
    {
        glg = GetComponent<GridLayoutGroup>();
        rt = transform as RectTransform;
    }

    public void ResizePanel()
    {
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, glg.cellSize.y * (transform.childCount / 2) + glg.padding.top * 2);
    }

    private void OnDisable()
    {
        foreach (Transform item in gameObject.transform)
        {
            Destroy(item.gameObject);
        }
    }
}
