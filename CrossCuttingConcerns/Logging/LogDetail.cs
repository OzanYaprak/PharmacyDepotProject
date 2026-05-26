namespace CrossCuttingConcerns.Logging;

public class LogDetail
{
    public LogDetail()
    {
        Fullname = string.Empty;
        MethodName = string.Empty;
        User = string.Empty;
        Parameters = new List<LogParameter>();
    }

    public LogDetail(string fullname, string methodName, string user, List<LogParameter> parameters)
    {
        Fullname = fullname;
        MethodName = methodName;
        User = user;
        Parameters = parameters;
    }

    public string? Fullname { get; set; }
    public string? MethodName { get; set; }
    public string? User { get; set; }
    public List<LogParameter>? Parameters { get; set; }
    public double ExecutionTimeMs { get; set; }
}


