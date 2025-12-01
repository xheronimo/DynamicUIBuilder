//DynamicUI.Logging.Targets / NetworkLogTarget.cs (opcional — ejemplo simple)
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DynamicUI.Logging.Targets
{
    /// <summary>
    /// Ejemplo de target que envía logs a un endpoint HTTP.
    /// NOTA: Implementación simplificada; en producción adaptar tiempo de espera y política de reintento.
    /// </summary>
    public class NetworkLogTarget : ILogTarget, IDisposable
    {
        private readonly HttpClient _client;
        private readonly string _endpoint;

        public NetworkLogTarget(string endpoint, HttpClient? client = null)
        {
            _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            _client = client ?? new HttpClient();
        }

        public void Write(LogEntry entry)
        {
            // Fire-and-forget; evitar bloqueos largos
            _ = PostAsync(entry);
        }

        private async Task PostAsync(LogEntry entry)
        {
            try
            {
                var payload = new
                {
                    timestamp = entry.Timestamp,
                    level = entry.Level.ToString(),
                    source = entry.Source,
                    message = entry.Message,
                    exception = entry.Exception?.ToString()
                };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await _client.PostAsync(_endpoint, content).ConfigureAwait(false);
            }
            catch
            {
                // swallow
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
