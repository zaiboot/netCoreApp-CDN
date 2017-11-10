using System.Threading.Tasks;

namespace netcoreCdn
{
    class Program
    {
        static void Main() => Task.Run(async () =>
                                {
                                    var worker = new UploadFilesWorker();
                                    // Do any async anything you need here without worry
                                    await worker.DoWork();
                                }).GetAwaiter().GetResult();
    }
}