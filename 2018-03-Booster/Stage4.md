# Booster 2018

## Azure AI


### Cognative Service - Computer Vision

Start by deploying computer Vision resource

Deploy the **Computer Vision API**

|Key|Value|
|---|---|
| Endpoint | https://westeurope.api.cognitive.microsoft.com/vision/v1.0
|Key| 0d5a5b80123456789abcdef9ad07bf08 |


#### .NET Console Application

First, lets make a console application


```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognativeAnalayze
{
   class Program 
   {
      const string skey = "0d5a5b80123456789abcdef9ad07bf08";
      const string uriBase = "https://westeurope.api.cognitive.microsoft.com/vision/v1.0/analyze";

      static void main(string[] args) 
      {
         
         string imageFilePath = @"...";

         MakeAnalysisRequest(imageFilePath);
         Console.ReadLine();
      }

      // Main Function Body
      
      public static async void MakeAnalysisRequest(string imageFilePath) 
      {
         
         // Establish the Paramaters for our Request
         string requestParameters = "visualFeatures=Categories,Description,Color&language=en";
         string uri = uriBase + "?" + requestParamaters;

         string contentString = null;

         // Establish a HTTP Client Object for the request
         using (HttpClient httpClient = new HttpClient())
         {
            
            httpClient.BaseAddress = new (Uri(uri));
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", skey);

            // We need to encode out image into a Byte Stream
            byte[] byteData = getImageAsByteArray(imageFilePath);
            // And then we can use it for posting to the API
            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
               // Post the Image
               content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
               var response = await httpClient.PostAsync(uri, content).Result;
            
               // Check the Results from the API
               contentstring = await response.Content.ReadAsStringAsync();
            
               // Display the JSON response.
               Console.WriteLine($"\nResponse: \n{contentString}");
            }
         }

      } 

      // Method to Encode our image as a Byte Array
      public static byte[] getImageAsByteArray(string imageFilePath)
      {
         FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
         BinaryReader binaryReader = new BinaryReader(fileStream);
         return binaryReader.ReadBytes((int)fileStream.Length);
      }

   }
}
```

#### Function Implemention

Now, lets do it again as an Azure Function. 
The main difference here include

* Function is automatically triggered
* File Location and Name is included in the calling Arguments
* The File Blob is referenced as a Byte Stream ready for out use


```csharp
using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;

const string sKey = "9855e5ee8570123456789abcdefc82c5";
const string uriBase = "https://westeurope.api.cognitive.microsoft.com/vision/v1.0/describe";


public static async Task<HttpResponseMessage> Run(Stream myBlobNew, string name, TraceWriter log)
{
    log.Info($"Processing image Name:{name},  Size: {myBlobNew.Length} bytes");

    // Establish the Paramaters for our Request
    string requestParameters = "visualFeatures=Categories,Description,Color&language=en";
    string uri = uriBase + "?" + requestParameters;

    string contentString = null;

    // Establish a HTTP Client Object for the request
    using (var httpClient = new HttpClient())
    {
        httpClient.BaseAddress = new Uri(uri);
        httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", sKey);

        // Out Image is provided as a Byte String from the Blob
        // To the function, so we can use it directly with the API
        using (HttpContent content = new StreamContent(myBlobNew))
        {
            // Post the Image
            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");
            var response = await httpClient.PostAsync(uri, content);
            
            // Check the Results from the API
            var responseBytes = await response.Content.ReadAsByteArrayAsync();

            // Get the JSON response.
            contentString = await response.Content.ReadAsStringAsync();

            // Display the JSON response.
            log.Info($"Response:{contentString}");
            Console.WriteLine($"\nResponse: \n{contentString}");
        }
    }
    
    return null;
}
```


### Cognative Service - Smart Thumbnails


```csharp
using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;

// Replace the subscriptionKey string value with your valid subscription key.
const string subscriptionKey = "e91d1d3900044636bc8482dd016794e1";

// Replace or verify the region.
//
// You must use the same region in your REST API call as you used to obtain your subscription keys.
// For example, if you obtained your subscription keys from the westus region, replace 
// "westcentralus" in the URI below with "westus".
//
// NOTE: Free trial subscription keys are generated in the westcentralus region, so if you are using
// a free trial subscription key, you should not need to change this region.
const string uriBase = "https://api.projectoxford.ai/vision/v1.0/generateThumbnail";
//const string uriBase = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/analyze";


public static void Run(Stream myBlob, string name, Stream outputBlob, TraceWriter log)
{
    log.Info($"trigger resize of original image\n Name:{name} \n Size: {myBlob.Length} Bytes");
    int width = 320;
    int height = 320;
    bool smartCropping = true;
   
    using (var httpClient = new HttpClient())
    {
        httpClient.BaseAddress = new Uri(uriBase);
        httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
        using (HttpContent content = new StreamContent(myBlob))
        {
            //get response
            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");
            var uri = $"{uriBase}?width={width}&height={height}&smartCropping={smartCropping.ToString()}";
            var response = httpClient.PostAsync(uri, content).Result;
            var responseBytes = response.Content.ReadAsByteArrayAsync().Result;

            //write to output thumb
            outputBlob.Write(responseBytes, 0, responseBytes.Length);
        }
    }
}

```
