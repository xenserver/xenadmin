using System;
namespace XenServerHealthCheckTests
{
    class Program
    {
        static void Main(string[] args)
        {
            RequestUploadTaskTests requestUploadTaskTests = new RequestUploadTaskTests();
            requestUploadTaskTests.checkUploadLock();
        }
    }
}
