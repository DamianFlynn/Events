using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognativeThumbnails
{
   class Program 
   {
      const string skey = "";
      const string uriBase = "https://westeurope.api.cognitive.microsoft.com/vision/v1.0/generateThumbnail";

      static void main(string[] args) 
      {
         
         string imageFilePath = @"...";

         GenerateThumbnailRequest(imageFilePath, 80, 80, true);
         Console.ReadLine();
      }

      // Main Function Body
      public static async void GenerateThumbnailRequest(string imageFilePath, int width, int height, bool smart)
      {
         byte[] thumbnail = GetThumbnail(imageFilePath, width, height, smart);

         string thumbnailFullPath = string.Format("{0}\\thumbnail_{1:yyyy-MMM-dd_hh-mm-ss}.jpg",Path.GetDirectoryName(imageFilePath), DateTime.Now);

         using (BinaryWriter bw = new BinaryWriter(new FileStream(thumbnailFullPath, FileMode.OpenOrCreate, FileAccess.Write))) 
         {
            bw.Write(thumbnail);
         }
            
      }

      public static async Task<byte[]> GeThumbnail(string imageFilePath, int width, int height, bool smart) 
      {
         
         // Establish the Paramaters for our Request
         string requestParameters = $"width={width.ToString()}&height={height.ToString()}&smartCropping={smart.ToString().ToLower()}";
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
               var response = await httpClient.PostAsync(uri, content);
            
               // Check the Results from the API
               return await response.Content.ReadAsByteArrayAsync();
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