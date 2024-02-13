using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Translation : MonoBehaviour
{
    private string phrase;
    private string translation;

    public string getPhrase() { return phrase; }
    public void setPhrase(string p) { phrase = p; }
    public string getTranslation() { return translation; }
    public void setTranslation(string t) { translation = t; }
}
