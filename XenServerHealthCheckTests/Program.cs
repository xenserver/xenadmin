using System;
using XenServerHealthCheck;

namespace XenServerHealthCheckTests
{
    class Program
    {
        static void Main(string[] args)
        {
            CredentialTests.CredentialReceiverTests();

            RequestUploadTaskTests requestUploadTaskTests = new RequestUploadTaskTests();
            requestUploadTaskTests.CheckUnenrolledHostShouldRemoved();
            requestUploadTaskTests.checkUploadLock();
            requestUploadTaskTests.checkDemandLock();
        }
    }
}
