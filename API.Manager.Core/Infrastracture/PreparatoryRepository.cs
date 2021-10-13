using API.Manager.Core.Options;
using API.Manager.Core.Resources;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace API.Manager.Core.Infrastracture
{
    public class PreparatoryRepository : RepositoryBase, IPreparatoryRepository
    {
        private readonly ApiManagerOptions _options;
        private readonly IDbConnection _dbConnection;
        public PreparatoryRepository(IDbConnection dbConnection, ApiManagerOptions options) : base(dbConnection)
        {
            _dbConnection = dbConnection;
            _options = options;
        }

        public async Task PrepareServiceTablesAsync(CancellationToken cancellationToken = default)
        {
            if (!_options.CreateTableIfNeccassary.Value)
                await FromResult(Task.CompletedTask);

            var installScriptQuery = Resource.ApiManagerInstallScript;
            installScriptQuery = string.Format(installScriptQuery, _options.Schema);

            installScriptQuery = installScriptQuery.Replace(Environment.NewLine," ");

            var command = CreateCommand(installScriptQuery, CommandType.Text);

            try
            {
                _dbConnection.Open();
                command.Transaction = _dbConnection.BeginTransaction();
                command.ExecuteNonQuery();
                command.Transaction.Commit();

            }
            catch (Exception ex)
            {
                command.Transaction.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                _dbConnection.Close();
            }

            await FromResult(Task.CompletedTask);
        }
    }
}
