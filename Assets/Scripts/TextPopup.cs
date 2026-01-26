using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;

//class for text popup that dies out after some time
public class TextPopup : MonoBehaviour
{


    private float curAge = 0;
    private float maxAge = 0;
    public float Lifespan = 10;
    public string Text = "GRAH";
    public Color Color = Color.red;
    private TextMeshProUGUI textMeshPro;
    private void Start()
    {
        curAge = 0;
        maxAge = Lifespan;
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.SetText(Text);
        textMeshPro.color = Color;
    }

    private void Update()
    {
        curAge += Time.deltaTime;
        Color modified = Color; 
        modified.a = Mathf.Pow(Mathf.Lerp(Color.a,0,curAge/maxAge),0.4f);
        textMeshPro.color = modified;
        if (curAge > maxAge)
        {
            Destroy(gameObject);
        }
    }


}
