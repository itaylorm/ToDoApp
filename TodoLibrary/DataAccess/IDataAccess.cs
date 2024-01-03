namespace TodoLibrary.DataAccess;

public interface IDataAccess
{
    Task<int> AddDataAsync<T>(string storedProcedure, T parameters, string idFieldName, string connectionStringName = "Default");

    Task<IEnumerable<T>?> LoadDataAsync<T, U>(string storedProcedure, U parameters, string connectionStringName = "Default");

    Task SaveDataAsync<T>(string storedProcedure, T parameters, string connectionStringName = "Default");
}
