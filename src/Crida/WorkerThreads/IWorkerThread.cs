using System.Threading.Tasks;

namespace Crida.WorkerThreads
{
    public interface IWorkerThread
    {
        // todo public void Init(Client client);

        public Task Run();
    }
}
