using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeContent : MonoBehaviour
{
    public GameObject scrollbar;
    public float scroll_pos = 0;
    float[] pos;

    // Update is called once per frame
    void Update()
    {
        pos = new float[transform.childCount];
        float panelWidth = 1f / (pos.Length - 1f);
        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = panelWidth * i;
        }

        if (Input.GetMouseButton(0))
        {
            scroll_pos = scrollbar.GetComponent<Scrollbar>().value;
            if (scroll_pos < 0)
                scroll_pos = 0;
        }
        else
        {
            for (int i = 0; i < pos.Length; i++)
            {
                if (scroll_pos < pos[i] + (panelWidth / 2f) && scroll_pos > pos[i] - (panelWidth / 2f))
                {
                    scrollbar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollbar.GetComponent<Scrollbar>().value, pos[i], 0.1f);
                }
            }
        }
    }
}