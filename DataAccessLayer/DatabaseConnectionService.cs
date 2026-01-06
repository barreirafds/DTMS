using Microsoft.Data.SqlClient;

namespace DataAcessLayer;

public class DatabaseConnectionService
{
    private readonly string _connectionString;
    private bool? _isConnected;
    private readonly object _lockObject = new object();

    public DatabaseConnectionService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public bool IsDatabaseConnected()
    {
        // Use cached value if available
        if (_isConnected.HasValue)
        {
            return _isConnected.Value;
        }

        lock (_lockObject)
        {
            // Double-check after acquiring lock
            if (_isConnected.HasValue)
            {
                return _isConnected.Value;
            }

            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                _isConnected = true;
                return true;
            }
            catch
            {
                _isConnected = false;
                return false;
            }
        }
    }

    public void ResetConnectionStatus()
    {
        lock (_lockObject)
        {
            _isConnected = null;
        }
    }

    public string ConnectionString => _connectionString;
}





