using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Manager.Core.Infrastracture
{
    public abstract class RepositoryBase
    {
        private readonly IDbConnection _dbConnection;
        protected RepositoryBase(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        protected virtual async Task<T> FromResult<T>(T type)
        {
            return await Task.FromResult(type);
        }

        protected  virtual CommandParameter CreateCommandParameter(string name, object value, DbType type)
        {
            return new CommandParameter()
            {
                Name = name,
                Value = value,
                Type = type
            };
        }

        protected virtual IDbCommand CreateCommand(string query, CommandType commandType, IList<CommandParameter> parameters = null)
        {
            var command = _dbConnection.CreateCommand();
            command.CommandText = query;

            if (parameters!=null && parameters.Any())
            {
                foreach (var item in parameters)
                {
                    var parameter = command.CreateParameter();
                    parameter.Value = item.Value;
                    parameter.DbType = item.Type;
                    parameter.ParameterName = item.Name;
                    command.Parameters.Add(parameter);
                }
            }

           return command;
        }

        public virtual async Task<DataTable> CreateDataTableVersion<T>(IList<T> data, CancellationToken cancellationToken = default)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));

            DataTable table = new DataTable();

            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            object[] values = new object[props.Count];

            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }

            return await FromResult(table);
        }
    }
}
