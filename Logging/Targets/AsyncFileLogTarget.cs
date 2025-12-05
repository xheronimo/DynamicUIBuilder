//AsyncFileLogTarget.cs
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DynamicUI.Logging.Targets
{
    public class AsyncFileLogTarget : ILogTarget, IDisposable
    {
        private readonly ConcurrentQueue<string> _queue = new();
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _workerTask;
        private readonly string _filePath;
        private readonly int _maxBatchSize = 100;
        private readonly TimeSpan _flushInterval = TimeSpan.FromSeconds(1);

        public AsyncFileLogTarget(string filePath)
        {
            _filePath = filePath;
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            _workerTask = Task.Run(ProcessQueueAsync);
        }

        public void Write(LogEntry entry)
        {
            _queue.Enqueue(entry.ToString());
        }

        private async Task ProcessQueueAsync()
        {
            var batch = new List<string>(_maxBatchSize);

            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_flushInterval, _cts.Token);

                    while (batch.Count < _maxBatchSize && _queue.TryDequeue(out var line))
                    {
                        batch.Add(line);
                    }

                    if (batch.Count > 0)
                    {
                        await File.AppendAllLinesAsync(_filePath, batch, _cts.Token);
                        batch.Clear();
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception)
                {
                    // swallow: nunca romper el hilo de logging
                }
            }

            // Flush final
            while (_queue.TryDequeue(out var line)) batch.Add(line);
            if (batch.Count > 0) File.AppendAllLines(_filePath, batch);
        }

        public void Dispose()
        {
            _cts.Cancel();
            _workerTask.Wait();
        }
    }
}
