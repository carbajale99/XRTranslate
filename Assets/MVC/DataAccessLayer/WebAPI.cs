using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class WebAPI : MonoBehaviour
{

    private HttpClient client = new HttpClient();

    /*client.BaseAddress = new System.Uri("https://xr-translate-flask-3017423fd510.herokuapp.com");*/

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

    public List<TextItem> ocr(string filePath)
    {
        client.BaseAddress = new System.Uri("https://xr-translate-flask-3017423fd510.herokuapp.com");
        var requestUri = "/ocr";

        // Check if the file exists
        if (!File.Exists(filePath))
        {
            Console.WriteLine("File not found.");
            return null;
        }

        try
        {
            using (var multipartContent = new MultipartFormDataContent())
            {
                // Load the file into a stream
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var fileContent = new StreamContent(fileStream);

                    multipartContent.Add(fileContent, "file", Path.GetFileName(filePath));

                    var response = client.PostAsync(requestUri, multipartContent).Result;
                    response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = response.Content.ReadAsStringAsync().Result;
                        var items = JsonConvert.DeserializeObject<List<TextItem>>(jsonString);
                        return items;
                    }
                    else
                    {
                        Console.WriteLine($"HTTP Error: {response.StatusCode}");
                        return null;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return null;
        }

    }



    /*public var ocr(string imgPath)
    {
        client.BaseAddress = new System.Uri("https://xr-translate-flask-3017423fd510.herokuapp.com");
        string ocrURL = "/ocr";


        var fileName = Path.GetFileName(imgPath);

        var requestContent = new MultipartFormDataContent();
        var fileStream = File.OpenRead(imgPath);

        requestContent.Add(new StreamContent(fileStream), "file", fileName);

        // here it is important that second parameter matches with name given in API.

        var response = client.PostAsync(ocrURL, requestContent).Result;

        if (response.IsSuccessStatusCode) //if translation success display
        {
            var responseContent = response.Content.ReadAsStringAsync().Result;
            responseContent = responseContent.Replace("\"", "");
            //Debug.Log(responseContent);
            return responseContent;

        }
        else //else error
        {
            Debug.Log("Error: " + response.StatusCode);
            return "Error: " + response.StatusCode;
        }

    }*/


    public string translate(string translationURL)
    {
        client.BaseAddress = new System.Uri("https://xr-translate-flask-3017423fd510.herokuapp.com");
        //string translationURL = stringToTranslationPar(phrase);

        var response = client.GetAsync(translationURL).Result; //response from api


        if (response.IsSuccessStatusCode) //if translation success display
        {
            var responseContent = response.Content.ReadAsStringAsync().Result;
            responseContent = responseContent.Replace("\"", "");
            Debug.Log(responseContent);
            return responseContent;

        }
        else //else error
        {
            Debug.Log("Error: " + response.StatusCode);
            return response.StatusCode.ToString();
        }
    }

    private string stringToTranslationPar(string phrase) //function to set up parameter 
    {
        string baseURL = "/translator?phrase="; //base URL

        string finalURL = baseURL + phrase; //baseURL + phrase to translate

        return finalURL;
    }
}
