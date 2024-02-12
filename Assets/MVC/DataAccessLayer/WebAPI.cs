using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine;

public class WebAPI : MonoBehaviour
{
    private HttpClient client = new HttpClient();


    // Start is called before the first frame update
    void Start()
    {
        //client = new HttpClient();
        //client.BaseAddress = new System.Uri("https://xr-translate-flask-3017423fd510.herokuapp.com");
    }

    public string imageToText(string imgPath)
    {
        client.BaseAddress = new System.Uri("https://xr-translate-flask-3017423fd510.herokuapp.com");
        string ocrURL = "/ocr";


        var fileName = Path.GetFileName(imgPath);
        //using var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(imgPath));
        //fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        //using var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(imgPath));
        //fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

        var requestContent = new MultipartFormDataContent();
        // here it is important that second parameter matches with name given in API.

        // here it is important that second parameter matches with name given in API.
        //form.Add(fileContent, "file", Path.GetFileName(imgPath));

        // here it is important that second parameter matches with name given in API.
        //form.Add(fileContent, "file", Path.GetFileName(imgPath));

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


        //var result = await response.Content.ReadAsStringAsync();
        //Debug.Log(result);



    }


    public string translate(string phrase)
    {
        client.BaseAddress = new System.Uri("https://xr-translate-flask-3017423fd510.herokuapp.com");
        string translationURL = stringToTranslationPar(phrase);

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
