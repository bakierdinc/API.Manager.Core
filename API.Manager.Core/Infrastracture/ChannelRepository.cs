using API.Manager.Core.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace API.Manager.Core.Infrastracture
{
    public class ChannelRepository : RepositoryBase, IChannelRepository
    {
        private readonly ApiManagerOptions _options;
        private readonly IDbConnection _dbConnection;

        public ChannelRepository(IDbConnection dbConnection, ApiManagerOptions options) : base(dbConnection)
        {
            _options = options;
            _dbConnection = dbConnection;
        }

        public async Task<IList<string>> GetAsync(CancellationToken cancellationToken = default)
        {
            IList<string> channels = new List<string>();
            string query = string.Format("SELECT * FROM {0}.Channel", _options.Schema);

            var command = CreateCommand(query, CommandType.Text);
            IDataReader reader = null;

            try
            {
                _dbConnection.Open();
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    channels.Add(reader.GetString(0));
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

            return await FromResult(channels);
        }

        public async Task AddAsync(string channel, CancellationToken cancellationToken = default)
        {
            string query = string.Format("INSERT INTO {0}.Channel (Channel) VALUES(@channel)", _options.Schema);

            IList<CommandParameter> parameters = new List<CommandParameter>();

            parameters.Add(CreateCommandParameter(string.Concat("@", nameof(channel)), channel, DbType.String));

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

        public async Task DeleteAsync(string channel, CancellationToken cancellationToken = default)
        {
            string serviceQuery = string.Format("DELETE {0}.Service  WHERE Channel=@channel", _options.Schema);
            string channelQuery = string.Format("DELETE {0}.Channel  WHERE Channel=@channel", _options.Schema);

            IList<CommandParameter> parameters = new List<CommandParameter>();

            parameters.Add(CreateCommandParameter(string.Concat("@", nameof(channel)), channel, DbType.String));

            var command = CreateCommand(serviceQuery, CommandType.Text, parameters);

            try
            {
                _dbConnection.Open();
                command.Transaction = _dbConnection.BeginTransaction();
                command.ExecuteNonQuery();
                command.Transaction.Commit();

                command = CreateCommand(channelQuery, CommandType.Text, parameters);
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
