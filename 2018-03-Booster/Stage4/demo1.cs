using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognativeAnalayze
{
   class Program 
   {
      const string skey = "";
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