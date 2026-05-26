using CrossCuttingConcerns.Serilog.ConfigurationModels;
using CrossCuttingConcerns.Serilog.Messages;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace CrossCuttingConcerns.Serilog.Loggers;

public class MssqlLogger : LoggerServiceBase
{

    public MssqlLogger(IConfiguration configuration)
    {
        MssqlConfiguration mssqlConfiguration = configuration.GetSection("SeriLogConfigurations:MssqlLogConfiguration").Get<MssqlConfiguration>() ?? throw new InvalidOperationException(SerilogMessages.NullOptionsMessage);

        MSSqlServerSinkOptions mssqlServerSinkOptions = new MSSqlServerSinkOptions()
        {
            TableName = mssqlConfiguration.TableName,
            AutoCreateSqlTable = mssqlConfiguration.AutoCreateSqlTable
        };

        ColumnOptions columnOptions = new ColumnOptions();

        Logger = new LoggerConfiguration()
            .WriteTo.MSSqlServer(
                connectionString: mssqlConfiguration.ConnectionString,
                sinkOptions: mssqlServerSinkOptions,
                columnOptions: columnOptions
            ).CreateLogger();
    }
}
