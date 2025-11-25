using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
namespace PsychoApp.ChatAI
{
    public class OpenRouterChat
    {
        private const string ApiKey = "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";
        private const string ApiUrl = "https://openrouter.ai/api/v1/chat/completions";

        public async Task<string> SendMessageAsync(List<object> messages)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
                client.DefaultRequestHeaders.Add("HTTP-Referer", "psychoapp://wpf");
                client.DefaultRequestHeaders.Add("X-Title", "Psychology App");

                var requestBody = new
                {
                    model = "x-ai/grok-4.1-fast:free",
                    temperature = 0.2,
                    top_p = 0.9,
                    messages = messages
                };

                string json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(ApiUrl, content);
                string responseText = await response.Content.ReadAsStringAsync();

                dynamic result = JsonConvert.DeserializeObject(responseText);

                if (result == null)
                    return "Error: result is null.\nRaw: " + responseText;

                if (result.error != null)
                    return "API Error: " + result.error.message + "\nRaw: " + responseText;

                if (result.choices == null)
                    return "Error: choices is null.\nRaw: " + responseText;

                try
                {
                    return result.choices[0].message.content.ToString();
                }
                catch
                {
                    return "Error: message.content is missing.\nRaw: " + responseText;
                }
            }
        }


    }
}
