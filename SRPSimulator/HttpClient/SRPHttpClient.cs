using SRPConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SRPSimulator

    // Stuff for remote DB operations
{
    public class JsonHttpClient : HttpClient
    {
        public JsonHttpClient() 
            : base()
        {
            DefaultRequestHeaders.Add("Accept", "application/json");
        }
    }

    // The object typename query is used as a path of the REST 

    internal class SRPHttpClient
    {
        private const string urlDefault = "https://localhost:7189/SRPapi/";

        // These 3 properties contain the results of last request
        public HttpStatusCode? ResultStatusCode
        { get; set; } = null;

        public string ErrorMessage
        { get; set; }

        public bool Success
        { get; set; }

        JsonSerializerOptions options = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
        };

        private JsonHttpClient GetClient()
        {
            // singleton
            return SRPServices.GetService(typeof(JsonHttpClient)) as JsonHttpClient;
        }

        private void SetFault(HttpRequestException exception)
        {
            ResultStatusCode = exception.StatusCode;
            ErrorMessage = exception.ToString();
            Success = false;
        }

        // Ping
        public async Task<bool> Ping()
        {
            HttpClient client = GetClient();
            try {
                await client.GetStringAsync(urlDefault);
                return Success = true;
            }
            catch (HttpRequestException exception) {
                SetFault(exception);
            }
            catch (Exception) { 
                Success = false; 
            }
            return false;
        }

        // Requests list of all settings for Object
        public async Task<IEnumerable<ConfigIdentity>> Index(object Object)
        {
            HttpClient client = GetClient();
            try {
                var result = await client.GetStringAsync(urlDefault + Object.GetType().Name);
                Success = true;
                return JsonSerializer.Deserialize<IEnumerable<ConfigIdentity>>(result, options);
            }
            catch (HttpRequestException exception) {
                SetFault(exception);
            }
            catch (Exception) { 
                Success = false; 
            }
            return Enumerable.Empty<ConfigIdentity>();
        }

        // Loads particular config for Object with Id
        public async Task<object> Get(object Object, int Id)
        {
            HttpClient client = GetClient();
            try {
                var uri = urlDefault + Object.GetType().Name + "/" + Id.ToString();
                var result = await client.GetStringAsync(uri);
                Success = true;
                return JsonSerializer.Deserialize(result, Object.GetType(), options);
            }
            catch (HttpRequestException exception) {
                SetFault(exception);
            }
            catch (Exception) {
                Success = false; 
            }
            return null;
        }

        // Adds new config
        public async Task<object> Add(object Object)
        {
            HttpClient client = GetClient();
            try {
                var response = await client.PostAsync(
                    urlDefault + Object.GetType().Name, 
                    new StringContent(
                        JsonSerializer.Serialize(Object, 
                        Object.GetType()), 
                        Encoding.UTF8, 
                        "application/json"));
                if (response.StatusCode != HttpStatusCode.OK) {
                    ResultStatusCode = response.StatusCode;
                    Success = false;
                    return null;
                }
                var result = await response.Content.ReadAsStringAsync();
                Success = true;
                return JsonSerializer.Deserialize(result, Object.GetType(), options);
            }
            catch (HttpRequestException exception) { 
                SetFault(exception); 
            }
            catch (Exception) { 
                Success = false; 
            }
            return null;
        }

        // Updates config with Id for Object
        public async Task<object> Update(object Object, int Id)
        {
            var json = JsonSerializer.Serialize(Object, Object.GetType());
            HttpClient client = GetClient();
            try {
                var response = await client.PutAsync(
                    urlDefault + Object.GetType().Name + "/" + Id, 
                    new StringContent(json, Encoding.UTF8, "application/json"));
                if (response.StatusCode != HttpStatusCode.OK) {
                    ResultStatusCode = response.StatusCode;
                    Success = false;
                    return null;
                }
                var result = await response.Content.ReadAsStringAsync();
                Success = true;
                return JsonSerializer.Deserialize(result, Object.GetType(), options);
            }
            catch (HttpRequestException exception) {
                SetFault(exception);
            }
            catch (Exception) { 
                Success = false; 
            }
            return null;
        }

        // Deletes config with Id for Object
        public async Task<object> Delete(object Object, int Id)
        {
            HttpClient client = GetClient();
            try {
                var response = await client.DeleteAsync(urlDefault + Object.GetType().Name + "/" + Id);
                if (response.StatusCode != HttpStatusCode.OK)
                    return null;
                var result = await response.Content.ReadAsStringAsync();
                Success = true;
                return JsonSerializer.Deserialize(result, Object.GetType(), options);
            }
            catch (HttpRequestException exception) {
                SetFault(exception);
            }
            catch (Exception) { 
                Success = false; 
            }
            return null;
        }
    }
}
