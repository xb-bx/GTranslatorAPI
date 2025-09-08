using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GTranslatorAPI
{
    /// <summary>
    /// internal net utils
    /// </summary>
    internal class NetUtil : IDisposable
    {
        /// <summary>
        /// user agent
        /// </summary>
        internal string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";

        /// <summary>
        /// query time out
        /// </summary>
        internal int NetworkQueryTimeout { get; set; } = 2000;

        /// <summary>
        /// build a new instance
        /// </summary>
        internal NetUtil()
        {
            _client = new() { Timeout = TimeSpan.FromMilliseconds(NetworkQueryTimeout) };
            _client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
        }

        private HttpClient _client;

        /// <summary>
        /// build a new instance from settings
        /// </summary>
        /// <param name="settings">network settings</param>
        internal NetUtil(Settings settings)
        {
            NetworkQueryTimeout = settings.NetworkQueryTimeout;
            UserAgent = settings.UserAgent;
            _client = new() { Timeout = TimeSpan.FromMilliseconds(NetworkQueryTimeout) };
            _client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
        }

        /// <summary>
        /// http escape string
        /// </summary>
        /// <param name="text">text to be escaped</param>
        /// <returns>escaped string</returns>
        public static string Escape(string text)
            => Uri.EscapeDataString(text);

        /// <summary>
        /// preform query at url and return result (or null), eventually exception message and object else status description
        /// </summary>
        /// <param name="url">url</param>
        /// <returns>resut|null,status description|error message,null|exception</returns>
        public async Task<(string?, string, Exception?)> GetQueryResponseAsync(string url, CancellationToken token = default)
        {
            try
            {
                var resp = await _client.GetAsync(url, token);
                if (!resp.IsSuccessStatusCode)
                {
                    return (null, resp.StatusCode.ToString(), null);
                }
                var body = await resp.Content.ReadAsStringAsync(token);
                return (body, "OK", null);
            }
            catch (Exception Ex)
            {
                return (null, Ex.Message, Ex);
            }
        }

        public void Dispose()
        {
            ((IDisposable)_client).Dispose();
        }
    }
}
