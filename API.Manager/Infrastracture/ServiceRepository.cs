using API.Manager.Core.Models;
using API.Manager.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace API.Manager.Infrastracture
{
    public class ServiceRepository : RepositoryBase, IServiceRepository
    {
        private readonly ApiManagerOptions _options;
        private readonly IDbConnection _dbConnection;

        public ServiceRepository(IDbConnection dbConnection, ApiManagerOptions options) : base(dbConnection)
        {
            _options = options;
            _dbConnection = dbConnection;
        }

        public async Task<IList<string>> GetProjectsAsync(CancellationToken cancellationToken = default)
        {
            IList<string> projects = new List<string>();

            string query = string.Format("SELECT DISTINCT Project FROM {0}.Service", _options.Schema);

            var command = CreateCommand(query, CommandType.Text);
            IDataReader reader = null;

            try
            {
                _dbConnection.Open();
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    projects.Add(reader.GetString(0));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                reader.Close();
                _dbConnection.Close();
            }

            return await FromResult(projects);
        }

        public async Task<IList<string>> GetControllersByProjectNameAsync(string projectName, CancellationToken cancellationToken = default)
        {
            IList<string> controllers = new List<string>();

            string query = string.Format("SELECT DISTINCT Controller FROM {0}.Service WHERE Project=@projectName", _options.Schema);

            IList<CommandParameter> commandParameters = new List<CommandParameter>();
            commandParameters.Add(CreateCommandParameter(string.Concat("@", nameof(projectName)), projectName, DbType.String));

            var command = CreateCommand(query, CommandType.Text, commandParameters);

            IDataReader reader = null;

            try
            {
                _dbConnection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    controllers.Add(reader.GetString(0));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                reader.Close();
                _dbConnection.Close();
            }

            return await FromResult(controllers);
        }

        public async Task<IList<Service>> GetMethodsByControllerNameAsync(string controllerName, CancellationToken cancellationToken = default)
        {
            IList<Service> methods = new List<Service>();

            string query = string.Format("SELECT * FROM {0}.Service WHERE Controller=@controllerName", _options.Schema);

            IList<CommandParameter> commandParameters = new List<CommandParameter>();

            commandParameters.Add(CreateCommandParameter(string.Concat("@", nameof(controllerName)), controllerName, DbType.String));

            var command = CreateCommand(query, CommandType.Text,commandParameters);

            IDataReader reader = null;

            try
            {
                _dbConnection.Open();
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Service service = new Service();
                    service.Id = reader.GetInt32(0);
                    service.Channel = reader.GetString(1);
                    service.Project = reader.GetString(2);
                    service.Controller = reader.GetString(3);
                    service.Method = reader.GetString(4);
                    service.MethodType = reader.GetString(5);
                    service.IsServiceable = reader.GetBoolean(6);

                    methods.Add(service);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                reader.Close();
                _dbConnection.Close();
            }

            return await FromResult(methods);
        }

        public async Task UpdateServiceStatusByIdAsync(int id, bool isServiceable, CancellationToken cancellationToken = default)
        {
            string query = string.Format("UPDATE {0}.Service  SET IsServiceable = @isServiceable WHERE Id=@id", _options.Schema);

            IList<CommandParameter> parameters = new List<CommandParameter>();

            parameters.Add(CreateCommandParameter(string.Concat("@", nameof(id)), id, DbType.String));
            parameters.Add(CreateCommandParameter(string.Concat("@", nameof(isServiceable)), isServiceable, DbType.String));

            var command = CreateCommand(query, CommandType.Text, parameters);

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

        public async Task AddBulkServiceAsync(IList<Service> services, string[] channels, CancellationToken cancellationToken = default)
        {
            IList<DataTable> dataTables = new List<DataTable>();

            var dataTable = await CreateDataTableVersion(services);

            _dbConnection.Open();
            var transaction = _dbConnection.BeginTransaction();
            try
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(_dbConnection.ConnectionString))
                {
                    bulkCopy.DestinationTableName = string.Format("{0}.Service", _options.Schema);
                    await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                _dbConnection.Close();
            }
        }

        public async Task<IList<Service>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IList<Service> methods = new List<Service>();

            string query = string.Format("SELECT * FROM {0}.Service", _options.Schema);
            var command = CreateCommand(query, CommandType.Text);

            IDataReader reader = null;

            try
            {
                _dbConnection.Open();
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Service service = new Service();
                    service.Id = reader.GetInt32(0);
                    service.Channel = reader.GetString(1);
                    service.Project = reader.GetString(2);
                    service.Controller = reader.GetString(3);
                    service.Method = reader.GetString(4);
                    service.MethodType = reader.GetString(5);
                    service.IsServiceable = reader.GetBoolean(6);

                    methods.Add(service);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                reader.Close();
                _dbConnection.Close();
            }

            return await FromResult(methods);
        }

        public async Task UpdateServiceStatusByChannel(string channel, bool isServiceable, CancellationToken cancellationToken = default)
        {
            string query = string.Format("UPDATE {0}.Service  SET IsServiceable = @isServiceable WHERE Channel=@channel", _options.Schema);

            IList<CommandParameter> parameters = new List<CommandParameter>();

            parameters.Add(CreateCommandParameter(string.Concat("@", nameof(channel)), channel, DbType.String));
            parameters.Add(CreateCommandParameter(string.Concat("@", nameof(isServiceable)), isServiceable, DbType.String));

            var command = CreateCommand(query, CommandType.Text, parameters);

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

        public async Task UpdateServiceStatusByProject(string project, bool isServiceable, CancellationToken cancellationToken = default)
        {
            string query = string.Format("UPDATE {0}.Service  SET IsServiceable = @isServiceable WHERE Project=@project", _options.Schema);

            IList<CommandParameter> parameters = new List<CommandParameter>();

            parameters.Add(CreateCommandParameter(string.Concat("@", nameof(project)), project, DbType.String));
            parameters.Add(CreateCommandParameter(string.Concat("@", nameof(isServiceable)), isServiceable, DbType.String));

            var command = CreateCommand(query, CommandType.Text, parameters);

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

        public async Task UpdateServiceStatusByController(string controller, bool isServiceable, CancellationToken cancellationToken = default)
        {
            string query = string.Format("UPDATE {0}.Service  SET IsServiceable = @isServiceable WHERE Controller=@controller", _options.Schema);

            IList<CommandParameter> parameters = new List<CommandParameter>();

            parameters.Add(CreateCommandParameter(string.Concat("@", nameof(controller)), controller, DbType.String));
            parameters.Add(CreateCommandParameter(string.Concat("@", nameof(isServiceable)), isServiceable, DbType.String));

            var command = CreateCommand(query, CommandType.Text, parameters);

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

        public async Task ClearDeletedServicesFromDbAsync(IList<int> serviceIds, CancellationToken cancellationToken = default)
        {
            string query = string.Format("DELETE FROM {0}.Service WHERE Id IN(@id)", _options.Schema);
            foreach (var id in serviceIds)
            {
                IList<CommandParameter> parameters = new List<CommandParameter>();

                parameters.Add(CreateCommandParameter("@id", id, DbType.Int32));
                var command = CreateCommand(query, CommandType.Text, parameters);

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
            }

            await FromResult(Task.CompletedTask);
        }

        public async Task<bool> IsServiceableAsync(Service service, CancellationToken cancellationToken = default)
        {
            bool result = true;
            string query = string.Format("SELECT IsServiceable FROM {0}.Service WHERE Channel=@channel AND Project=@project AND Controller=@controller AND Method=@method AND MethodType=@methodType", _options.Schema);

            IList<CommandParameter> parameters = new List<CommandParameter>();

            parameters.Add(CreateCommandParameter("@channel", service.Channel, DbType.String));
            parameters.Add(CreateCommandParameter("@controller", service.Controller, DbType.String));
            parameters.Add(CreateCommandParameter("@project", service.Project, DbType.String));
            parameters.Add(CreateCommandParameter("@method", service.Method, DbType.String));
            parameters.Add(CreateCommandParameter("@methodType", service.MethodType, DbType.String));

            var command = CreateCommand(query, CommandType.Text, parameters);


            IDataReader reader = null;

            try
            {
                _dbConnection.Open();
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = reader.GetBoolean(0);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                reader.Close();
                _dbConnection.Close();
            }

            return await FromResult(result);
        }
    }
}
