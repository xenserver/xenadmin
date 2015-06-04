using System;
namespace XenServerHealthCheckTests
{
    class Program
    {
        static void Main(string[] args)
        {
            RequestUploadTaskTests.checkUploadLock();
        }
    }
}
