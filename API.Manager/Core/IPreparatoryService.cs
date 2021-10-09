using System.Threading;
using System.Threading.Tasks;

namespace API.Manager.Core
{
    public interface IPreparatoryService
    {
        Task PrepareServiceTablesAsync(CancellationToken cancellationToken = default);
        Task PrepareServiceData(CancellationToken cancellationToken=default);
    }
}
