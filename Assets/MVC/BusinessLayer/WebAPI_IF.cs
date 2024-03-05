using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Vertex
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class TextItem
    {
        public string Text { get; set; } = string.Empty;

        public List<Vertex> Vertices { get; set; } = new List<Vertex>();
    }

public interface WebAPI_IF
{
    List<TextItem> ocr(string filePath);
    string translate(string phrase);
}
