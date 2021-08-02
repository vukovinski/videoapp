using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace videoapp.api.Services
{
    public static class SimpleGetter
    {
        public static async Task<TData> RetrieveAsync<TData>(string url, ILogger logger = null) where TData : class
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var client = new WebClient();
                    var data = client.DownloadString(url);
                    if (data != null)
                    {
                        return (TData)JsonSerializer.Deserialize(data, typeof(TData));
                    }
                    return null;
                }
                catch (Exception e)
                {
                    logger?.LogError(e, $"error while getting object instance of type {typeof(TData).Name} from {url}!");
                    return null;
                }
            });
        }
    }
}
