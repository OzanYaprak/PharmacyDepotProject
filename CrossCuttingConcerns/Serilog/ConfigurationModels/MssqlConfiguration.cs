namespace CrossCuttingConcerns.Serilog.ConfigurationModels;

public class MssqlConfiguration
{
    public MssqlConfiguration()
    {
        ConnectionString = string.Empty;
        TableName = string.Empty;
    }
    public MssqlConfiguration(string connectionString, string tableName, bool autoCreateSqlTable)
    {
        ConnectionString = connectionString;
        TableName = tableName;
        AutoCreateSqlTable = autoCreateSqlTable;
    }

    public string ConnectionString { get; set; }
    public string TableName { get; set; }
    public bool AutoCreateSqlTable { get; set; }

}
