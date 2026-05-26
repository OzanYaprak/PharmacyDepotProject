using CrossCuttingConcerns.Serilog.ConfigurationModels;
using CrossCuttingConcerns.Serilog.Messages;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Json;

namespace CrossCuttingConcerns.Serilog.Loggers;

public class FileLogger : LoggerServiceBase
{
    public FileLogger(IConfiguration configuration)
    {
        FileLogConfiguration fileLogConfiguration = configuration.GetSection("SeriLogConfigurations:FileLogConfiguration").Get<FileLogConfiguration>() ?? throw new InvalidOperationException(SerilogMessages.NullOptionsMessage);

        string logFolder = Path.Combine(Directory.GetCurrentDirectory(), fileLogConfiguration.FolderPath);
        string txtLogFilePath = Path.Combine(logFolder, "Logs.txt");
        string jsonLogFilePath = Path.Combine(logFolder, "Logs.json");

        Logger = new LoggerConfiguration()
            .WriteTo.File(
                path: txtLogFilePath,
                rollingInterval: RollingInterval.Hour,
                retainedFileCountLimit: null,
                fileSizeLimitBytes: 5000000,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
            .WriteTo.File(
                formatter: new JsonFormatter(renderMessage: true),
                path: jsonLogFilePath,
                rollingInterval: RollingInterval.Hour,
                retainedFileCountLimit: null,
                fileSizeLimitBytes: 5000000)
            .CreateLogger();
    }
}
