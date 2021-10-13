using System.Threading;
using System.Threading.Tasks;

namespace API.Manager.Core.Infrastracture
{
    public interface IPreparatoryRepository
    {
        Task PrepareServiceTablesAsync(CancellationToken cancellationToken = default);
    }
}
