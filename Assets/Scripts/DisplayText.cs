using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//class for storing text that gets displayed in ui
public class DisplayText
{
    public DisplayText()
    {
    }

    private string text = "";

    //method to add text to displaytext
    public void AddText(string _text)
    {
        text += _text;
    }

    //method to add new line to displaytext
    public void NewLine()
    {
        text += "\n";
    }


    //method to return text as string
    public override string ToString()
    {
        return text;
    }
}
