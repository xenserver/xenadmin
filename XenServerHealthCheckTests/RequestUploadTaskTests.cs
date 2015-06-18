using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XenAdmin;
using XenServerHealthCheck;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Network;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XenAdminTests;

namespace XenServerHealthCheckTests
{

    class RequestUploadTaskTests: DatabaseTester_TestFixture
    {
        private const string dbName = "state1.xml";
        public RequestUploadTaskTests() : base(dbName) { }
        private static string UUID = "test-test";

        public Dictionary<string, string> cleanStack()
        {
            Dictionary<string, string> config = new Dictionary<string, string>();
            config[CallHomeSettings.STATUS] = "true";
            config[CallHomeSettings.UPLOAD_LOCK] = "";
            config[CallHomeSettings.INTERVAL_IN_DAYS] = CallHomeSettings.intervalInDaysDefault.ToString();
            config[CallHomeSettings.DAY_OF_WEEK] = "0";
            config[CallHomeSettings.LAST_SUCCESSFUL_UPLOAD] = "";
            config[CallHomeSettings.LAST_FAILED_UPLOAD] = "";
            return config;
        }

        public void checkUploadLock()
        {
            DatabaseManager.CreateNewConnection(dbName);
            IXenConnection connection = DatabaseManager.ConnectionFor(dbName);
            Session _session = DatabaseManager.ConnectionFor(dbName).Session;
            DatabaseManager.ConnectionFor(dbName).LoadCache(_session);
            try
            {
                Dictionary<string, string> config = cleanStack();
                connection.LoadCache(_session);

                //1. If XenServer has not enroll, lock will not been set.
                config = cleanStack();
                config[CallHomeSettings.STATUS] = "false";
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsFalse(RequestUploadTask.Request(connection, _session));

                //2.If the lock has already set by current service and not due, the lock should not been set again.
                config = cleanStack();
                config[CallHomeSettings.UPLOAD_LOCK] = UUID + "|" + DateTime.UtcNow.ToString();
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsFalse(RequestUploadTask.Request(connection, _session));


                //3. If the lock already due or no one set the lock, but current schedule DayOfWeek and TimeOfDay is not correct, the lock should not been set.
                config = cleanStack();
                config[CallHomeSettings.UPLOAD_LOCK] = UUID + "|" + DateTime.UtcNow.Subtract(TimeSpan.FromDays(14)).ToString();
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsFalse(RequestUploadTask.Request(connection, _session));

                //4. For lock due or not set by others and schedule meet, lock should be set.
                config = cleanStack();
                config[CallHomeSettings.UPLOAD_LOCK] = UUID + "|" + DateTime.UtcNow.Subtract(TimeSpan.FromDays(14)).ToString();
                config[CallHomeSettings.DAY_OF_WEEK] = DateTime.UtcNow.DayOfWeek.ToString();
                config[CallHomeSettings.TIME_OF_DAY] = DateTime.UtcNow.Hour.ToString();
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsTrue(RequestUploadTask.Request(connection, _session));

                //5. For Lock set by other service and not due, the lock should not been set by us.
                config = cleanStack();
                config[CallHomeSettings.UPLOAD_LOCK] = "test2-test2" + "|" + DateTime.UtcNow.ToString();
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsFalse(RequestUploadTask.Request(connection, _session));

                //6. For Lock set by other service but already due, the lock can be set by current service
                config = cleanStack();
                config[CallHomeSettings.UPLOAD_LOCK] = "test2-test2" + "|" + DateTime.UtcNow.Subtract(TimeSpan.FromDays(14)).ToString();
                config[CallHomeSettings.DAY_OF_WEEK] = DateTime.UtcNow.DayOfWeek.ToString();
                config[CallHomeSettings.TIME_OF_DAY] = DateTime.UtcNow.Hour.ToString();
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsTrue(RequestUploadTask.Request(connection, _session));

                //7 Check LastFailedUpload is not empty and > LastSuccessfulUpload && INTERVAL_IN_DAYS using default, lock can be set 
                config = cleanStack();
                config[CallHomeSettings.LAST_SUCCESSFUL_UPLOAD] = DateTime.UtcNow.Subtract(TimeSpan.FromDays(14)).ToString();
                config[CallHomeSettings.LAST_FAILED_UPLOAD] = DateTime.UtcNow.Subtract(TimeSpan.FromDays(8)).ToString();
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsTrue(RequestUploadTask.Request(connection, _session));

                //8 For not due uploading, lock should not been set 
                config = cleanStack();
                config[CallHomeSettings.LAST_SUCCESSFUL_UPLOAD] = DateTime.UtcNow.Subtract(TimeSpan.FromDays(6)).ToString();
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsFalse(RequestUploadTask.Request(connection, _session));

                //9 For failed upload, retry was needed but not meet RetryIntervalInDays, lock should not been set
                config = cleanStack();
                config[CallHomeSettings.LAST_SUCCESSFUL_UPLOAD] = DateTime.UtcNow.Subtract(TimeSpan.FromDays(14)).ToString();
                config[CallHomeSettings.LAST_FAILED_UPLOAD] = DateTime.UtcNow.Subtract(TimeSpan.FromDays(5)).ToString();
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsFalse(RequestUploadTask.Request(connection, _session));

                //10 For failed upload, retry was needed and meet RetryIntervalInDays, lock should be set
                config = cleanStack();
                config[CallHomeSettings.LAST_FAILED_UPLOAD] = DateTime.UtcNow.Subtract(TimeSpan.FromDays(7)).ToString();
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsTrue(RequestUploadTask.Request(connection, _session));


                //11 Retry needed because no LAST_SUCCESSFUL_UPLOAD but not meet RetryIntervalInDays, lock should not be set 
                config = cleanStack();
                config[CallHomeSettings.LAST_SUCCESSFUL_UPLOAD] = "";
                config[CallHomeSettings.LAST_FAILED_UPLOAD] = DateTime.UtcNow.Subtract(TimeSpan.FromDays(8)).ToString();
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsTrue(RequestUploadTask.Request(connection, _session));

                //12 For no LAST_FAILED_UPLOAD or invalid LAST_FAILED_UPLOAD, lock should not be set if not due
                config = cleanStack();
                config[CallHomeSettings.LAST_SUCCESSFUL_UPLOAD] = DateTime.UtcNow.Subtract(TimeSpan.FromDays(13)).ToString();
                config[CallHomeSettings.LAST_FAILED_UPLOAD] = "asd";
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsFalse (RequestUploadTask.Request(connection, _session));
            }
            catch (Exception)
            {}
        }

        public void checkDemandLock()
        {
            DatabaseManager.CreateNewConnection(dbName);
            IXenConnection connection = DatabaseManager.ConnectionFor(dbName);
            Session _session = DatabaseManager.ConnectionFor(dbName).Session;
            DatabaseManager.ConnectionFor(dbName).LoadCache(_session);
            try
            {
                Dictionary<string, string> config = cleanStack();
                connection.LoadCache(_session);
                //1 Uploading is inprocess by current service, demand will be ignore
                config = cleanStack();
                config[CallHomeSettings.UPLOAD_LOCK] = UUID + "|" + DateTime.UtcNow.ToString();
                config[CallHomeSettings.NEW_UPLOAD_REQUEST] = DateTime.UtcNow.ToString();
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsFalse(RequestUploadTask.OnDemandRequest(connection, _session));

                //2 Uploading is inprocess by other service, demand will be ignore
                config = cleanStack();
                config[CallHomeSettings.UPLOAD_LOCK] = "test2-test2" + "|" + DateTime.UtcNow.ToString();
                config[CallHomeSettings.NEW_UPLOAD_REQUEST] = DateTime.UtcNow.ToString();
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsFalse(RequestUploadTask.OnDemandRequest(connection, _session));

                //3 Uploading is not due and demand due,  demand will be ignore
                config = cleanStack();
                config[CallHomeSettings.UPLOAD_LOCK] = "test2-test2" + "|" + DateTime.UtcNow.Subtract(TimeSpan.FromDays(14)).ToString();
                config[CallHomeSettings.NEW_UPLOAD_REQUEST] = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(31)).ToString();
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsFalse(RequestUploadTask.OnDemandRequest(connection, _session));

                //4 Uploading is due and demand not due, lock will be set
                config = cleanStack();
                config[CallHomeSettings.UPLOAD_LOCK] = "test2-test2" + "|" + DateTime.UtcNow.Subtract(TimeSpan.FromDays(14)).ToString();
                config[CallHomeSettings.NEW_UPLOAD_REQUEST] = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(28)).ToString();
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsTrue(RequestUploadTask.OnDemandRequest(connection, _session));
            }
            catch (Exception)
            { }
        }
    }
}
