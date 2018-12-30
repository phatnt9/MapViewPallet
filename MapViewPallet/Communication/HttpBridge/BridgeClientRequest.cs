using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SelDatUnilever_Ver1._00.Communication.HttpBridge
{
   public class BridgeClientRequest
    {
        public virtual void ReceiveResponseHandler(string msg) { }
        public virtual void ErrorBridgeHandler(int code) { }
       // public event Action<string> ReceiveResponseHandler;
        //public event Action<int> ErrorBridgeHandler;
        public BridgeClientRequest() { }
        public async Task<string> PostCallAPI(string url, object jsonObject)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);
                    if (response != null)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        ReceiveResponseHandler(jsonString);
                        //Console.WriteLine("PostCallAPI:  " + jsonString);
                        return jsonString;
                        //return JsonConvert.DeserializeObject<object>(jsonString);
                    }
                }
            }
            catch
            {
               
            }
            return null;
        }
        public async Task<object> PostCallAPI(string url, string jsonObject)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);
                    if (response != null)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        ReceiveResponseHandler(jsonString);
                        //Console.WriteLine("PostCallAPI:  " + jsonString);
                        return jsonString;
                       // return JsonConvert.DeserializeObject<object>(jsonString);
                    }
                }
            }
            catch 
            {
                Console.WriteLine(url+"  "+ jsonObject);
            }
            return null;
        }
        public async Task<String> GetCallAPI(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    if (response != null)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        ReceiveResponseHandler(jsonString);
                        //Console.WriteLine("GetCallAPI:  " + jsonString);
                        return jsonString;
                    }
                }
            }
            catch
            {

            }
            return null;
        }
    }
}
