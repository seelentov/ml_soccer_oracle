using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using System.Reflection;
using System.Text.Json;
using WebApplication2.Models;
using WebApplication2.Models.ML;
using WebApplication2.Services;

namespace WebApplication2.Workers
{
    public class MLWorker: IHostedService
    {
        private readonly ILogger<MLWorker> _logger;
        private readonly Soccer365Parser _soccer365parser;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TelegramService _telegramService;
        private readonly MLContext _mlContext;
        private readonly string _resultKey;
        private readonly InputOutputColumnPair[] _categoriesKeys;
        private readonly string[] _featureKeys;
        private readonly string _modelFilePath;


        public MLWorker(Soccer365Parser soccer365parser, ILogger<MLWorker> logger, IServiceScopeFactory scopeFactory, TelegramService telegramService)
        {
            var resultKey = "Result";

            var categoriesKeys = new List<string>()
            {
                "LeagueName",
                "MatchDate",
                "Day",
                "Month",
                "Year",
                "Team1Name",
                "Team2Name",
            };

            _modelFilePath = "model.zip";

            var serializedModel = JsonSerializer.Serialize(new MLGame());
            var deserialize =  JsonSerializer.Deserialize<Dictionary<string, object>>(serializedModel);

            var featureKeys = deserialize.Keys.ToList();

            foreach (var categoriesKey in categoriesKeys)
            {
                featureKeys.Remove(categoriesKey);
            }
            featureKeys.Remove(resultKey);

            _featureKeys = featureKeys.ToArray();
            _categoriesKeys = categoriesKeys.Select(c => new InputOutputColumnPair(c)).ToArray();
            _resultKey = resultKey;

            _logger = logger;
            _soccer365parser = soccer365parser;
            _logger = logger;
            _scopeFactory = scopeFactory;
            _mlContext = new MLContext();
            _telegramService = telegramService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _ = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        try
                        {
                            _logger.LogInformation("Start ML Cycle", LogLevel.Information);

                            var gamesService = scope.ServiceProvider.GetRequiredService<GamesService>();
                            var leaguesService = scope.ServiceProvider.GetRequiredService<LeaguesService>();
                            var optionsService = scope.ServiceProvider.GetRequiredService<OptionsService>();

                            var leaguesCount = (await leaguesService.GetAll()).Count();
                            var leaguesParsedCount = (await leaguesService.GetRange(l => l.Parsed)).Count();


                            var games = (await gamesService.GetAllML()).ToList();

                            if (games.Count < 1)
                            {
                                _logger.LogInformation("Wait 1 hour ", LogLevel.Information);
                                await Task.Delay(TimeSpan.FromHours(1));
                                continue;
                            }

                            _logger.LogInformation("Get Games. Count: " + games.Count, LogLevel.Information);

                            var data = _mlContext.Data.LoadFromEnumerable<MLGame>(games);
                            _logger.LogInformation("Create data", LogLevel.Information);

                            var pipeline = _mlContext.Transforms.Categorical.OneHotEncoding(_categoriesKeys)
                            .Append(_mlContext.Transforms.Concatenate("Features", _featureKeys))
                            .Append(_mlContext.Transforms.NormalizeMinMax("Features"))
                            .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: _resultKey, maximumNumberOfIterations: 100));

                            _logger.LogInformation("Create pipeline", LogLevel.Information);

                            var trainTestData = _mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

                            var trainData = trainTestData.TrainSet;
                            var testData = trainTestData.TestSet;

                            var model = pipeline.Fit(trainData);

                            _logger.LogInformation("TrainedModel pipeline", LogLevel.Information);

                            var predictions = model.Transform(testData);

                            var metrics = _mlContext.Regression.Evaluate(predictions, labelColumnName: _resultKey);

                            _logger.LogInformation($"R^2: {metrics.RSquared:0.##}", LogLevel.Information);
                            _logger.LogInformation($"RMS error: {metrics.RootMeanSquaredError:0.##}", LogLevel.Information);
                            _logger.LogInformation($"MS error: {metrics.MeanSquaredError:0.##}", LogLevel.Information);

                            await optionsService.UpdateOrAdd(new Option() { Key = "ML_R^2", Value = $"{metrics.RSquared:0.##}" });
                            await optionsService.UpdateOrAdd(new Option() { Key = "ML_RMS error", Value = $"{metrics.RootMeanSquaredError:0.##}" });
                            await optionsService.UpdateOrAdd(new Option() { Key = "ML_MS error", Value = $"{metrics.MeanSquaredError:0.##}" });

                            await _telegramService.SendMessage($"R^2: {metrics.RSquared:0.##}\nRMS error: {metrics.RootMeanSquaredError:0.##}\nMS error: {metrics.MeanSquaredError:0.##}\nGames count: {games.Count}\nLeagues: {leaguesParsedCount} / {leaguesCount}", "MLWorkerStat");

                            _mlContext.Model.Save(model, data.Schema, _modelFilePath);

                            await Task.Delay(TimeSpan.FromHours(4));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInformation(ex, ex.Message, LogLevel.Information);
                            await _telegramService.SendMessage(ex.Message, "MLWorkerError");

                        }
                    }

                }
            }, cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

    }
}
