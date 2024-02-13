using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface WebAPI_IF
{
    string imageToText(string imgPath);
    string translate(string phrase);
}
