using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadGeneratorApp
{
    // klase atsako uz gijos ir generavima 
    public class ThreadManager
    {
        // Kiekvienai gija atitinkantis CancellationTokenSource 
        private List<CancellationTokenSource> _cancellationTokenSources;

        // Task, kurios veikia kaip threads ir vykdo generavimo ciklą.
        private List<Task> _tasks;

        // irasymas i db
        private DatabaseService _databaseService;

        // random genravimas
        private Random _random;

        
        public event EventHandler<ThreadResult> DataGenerated;

        public ThreadManager()
        {
            // inicializacija kolekcijas ir servisus.
            _cancellationTokenSources = new List<CancellationTokenSource>();
            _tasks = new List<Task>();
            _databaseService = new DatabaseService();
            _random = new Random();
        }

        public async Task StartAsync(int threadCount)
        {
        
            Stop(); // Stop any existing threads

            // Sukuriame nurodytą kiekį gijų, numeruojam nuo 1.
            for (int i = 1; i <= threadCount; i++)
            {
                var cts = new CancellationTokenSource();
                _cancellationTokenSources.Add(cts);

                int threadId = i;
                var task = Task.Run(async () =>
                {
                    // Kiekviena gija sukasi cikle iki kol gaus stop signalą.
                    while (!cts.Token.IsCancellationRequested)
                    {
                        try
                        {
                            // Atsitiktinai parenkame generuojamos eilutės ilgį (5-10 simbolių).
                            int stringLength = _random.Next(5, 11);
                            string generatedString = GenerateRandomString(stringLength);
                            
                            // Sukuriame rezultatą su gijos ID ir generavimo laiku.
                            var result = new ThreadResult
                            {
                                ThreadId = threadId,
                                GeneratedString = generatedString,
                                Timestamp = DateTime.Now
                            };

                            // Įrašome į DB kiekvieną sugeneruotą įrašą
                            await _databaseService.SaveResultAsync(result);

                            // ListView atnaujinimas
                            DataGenerated?.Invoke(this, result);

                            //pauzė tarp generavimų
                            int delayMs = _random.Next(500, 2001);
                            await Task.Delay(delayMs, cts.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            //rankiniu budu išeiname iš ciklo.
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Thread {threadId} error: {ex.Message}");
                        }
                    }
                }, cts.Token);
                _tasks.Add(task);
            }

            await Task.WhenAll(_tasks);
        }

        public void Stop()
        {
            // Siunčiame cancel signalą visoms gijoms.
            foreach (var cts in _cancellationTokenSources)
            {
                cts.Cancel();
                cts.Dispose();
            }

            // Išvalome kolekcijas, kad būtų galima StartAsync kviesti iš naujo.
            _cancellationTokenSources.Clear();
            _tasks.Clear();
        }

        private string GenerateRandomString(int length)
        {
            // Galimi simboliai
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var sb = new StringBuilder(length);
            
            // Sudedame atsitiktinius simbolius.
            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[_random.Next(chars.Length)]);
            }
            
            return sb.ToString();
        }
    }
}
