using System.Threading.Tasks;

namespace netcoreCdn
{
    internal interface IWorker
    {
           Task DoWork();
    }
}