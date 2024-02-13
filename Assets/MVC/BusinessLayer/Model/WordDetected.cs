using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordDetected : MonoBehaviour
{
    private float xMin;
    private float yMin;
    private float xMax;
    private float yMax;

    private float height;
    private float width;

    public float getXmin() { return xMin; }
    public float getYmin() {  return yMin; }

    public void setXmin(float x) { xMin = x; }
    public void setYmin(float y) { yMin = y; }
    public void setXmax(float x) { xMax = x; }
    public void setYmax(float y) { yMax = y; }

    public void calculateHeight() {  }

    public void calculateWidth() { }

    public float getHeight() { return height; }

    public float getWidth() { return width; }
}
