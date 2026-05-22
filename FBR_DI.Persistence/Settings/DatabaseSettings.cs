using FBR_DI.Persistence.Enums;

namespace FBR_DI.Persistence.Settings;

public class DatabaseSettings
{
    public const string SectionName = "Database";

    public string Provider { get; set; } = "SqlServer";
    public string ConnectionString { get; set; } = string.Empty;

    public DatabaseProvider GetProvider() => Provider.Trim().ToLower() switch
    {
        "sqlserver" or "mssql"                          => DatabaseProvider.SqlServer,
        "mysql"     or "mariadb"                        => DatabaseProvider.MySQL,
        "postgresql" or "postgres" or "pgsql"           => DatabaseProvider.PostgreSQL,
        "sqlite"                                        => DatabaseProvider.SQLite,
        _                                               => DatabaseProvider.SqlServer
    };
}
