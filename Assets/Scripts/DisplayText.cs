using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisplayText
{
    public enum TextColor
    {
        White,
        Fire,
        Water,
        Gold
    }
    private static string getColor(TextColor _color)
    {
        Color rgb;
        switch (_color) {
            default: rgb = Color.white;
                break;
        }
        return "?c"+ColorUtility.ToHtmlStringRGB(rgb);
    }

    public DisplayText()
    {
    }

    private string text = "";

    public void AddText(string _text)
    {
        text += _text;
    }
    public void AddText(TextColor _color, string _text)
    {
        text += getColor(_color) + _text;
    }
    public void NewLine()
    {

    }

    public override string ToString()
    {
        return text;
    }
}
