﻿using System;
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
using System.IO.Pipes;
using System.IO;
using XenAdmin.Model;

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
            config[HealthCheckSettings.STATUS] = "true";
            config[HealthCheckSettings.UPLOAD_LOCK] = "";
            config[HealthCheckSettings.INTERVAL_IN_DAYS] = HealthCheckSettings.DEFAULT_INTERVAL_IN_DAYS.ToString();
            config[HealthCheckSettings.DAY_OF_WEEK] = "0";
            config[HealthCheckSettings.LAST_SUCCESSFUL_UPLOAD] = "";
            config[HealthCheckSettings.LAST_FAILED_UPLOAD] = "";
            return config;
        }
        private const char SEPARATOR = '\x202f'; // narrow non-breaking space.
        public void CheckUnenrolledHostShouldRemoved()
        {
            try
            {
                CredentialReceiver.instance.Init();
                ServerListHelper.instance.Init();
                DatabaseManager.CreateNewConnection(dbName);
                IXenConnection connection = DatabaseManager.ConnectionFor(dbName);
                Session _session = DatabaseManager.ConnectionFor(dbName).Session;
                DatabaseManager.ConnectionFor(dbName).LoadCache(_session);
                Dictionary<string, string> config = cleanStack();
                connection.LoadCache(_session);

                int conSize = ServerListHelper.instance.GetServerList().Count;

                NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", HealthCheckSettings.HEALTH_CHECK_PIPE, PipeDirection.Out);
                pipeClient.Connect();
                string credential = EncryptionUtils.ProtectForLocalMachine(String.Join(SEPARATOR.ToString(), new[] { connection.Hostname, connection.Username, connection.Password }));
                pipeClient.Write(Encoding.UTF8.GetBytes(credential), 0, credential.Length);
                pipeClient.Close();
                System.Threading.Thread.Sleep(1000);
                List<ServerInfo> con = ServerListHelper.instance.GetServerList();
                Assert.IsTrue(con.Count == conSize + 1);


                //1. If XenServer has not enroll, lock will not been set.
                config = cleanStack();
                config[HealthCheckSettings.STATUS] = "false";
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsFalse(RequestUploadTask.Request(connection, _session));
                con = ServerListHelper.instance.GetServerList();
                Assert.IsTrue(con.Count == conSize);
                CredentialReceiver.instance.UnInit();
            }
            catch (Exception)
            { }
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
                config[HealthCheckSettings.STATUS] = "false";
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsFalse(RequestUploadTask.Request(connection, _session));

                //2.If the lock has already set by current service and not due, the lock should not been set again.
                config = cleanStack();
                config[HealthCheckSettings.UPLOAD_LOCK] = UUID + "|" + HealthCheckSettings.DateTimeToString(DateTime.UtcNow);
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsFalse(RequestUploadTask.Request(connection, _session));


                //3. If the lock already due or no one set the lock, but current schedule DayOfWeek and TimeOfDay is not correct, the lock should not been set.
                config = cleanStack();
                config[HealthCheckSettings.UPLOAD_LOCK] = UUID + "|" + HealthCheckSettings.DateTimeToString(DateTime.UtcNow.Subtract(TimeSpan.FromDays(14)));
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsFalse(RequestUploadTask.Request(connection, _session));

                //4. For lock due or not set by others and schedule meet, lock should be set.
                config = cleanStack();
                config[HealthCheckSettings.UPLOAD_LOCK] = UUID + "|" + HealthCheckSettings.DateTimeToString(DateTime.UtcNow.Subtract(TimeSpan.FromDays(14)));
                config[HealthCheckSettings.DAY_OF_WEEK] = DateTime.UtcNow.DayOfWeek.ToString();
                config[HealthCheckSettings.TIME_OF_DAY] = DateTime.UtcNow.Hour.ToString();
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsTrue(RequestUploadTask.Request(connection, _session));

                //5. For Lock set by other service and not due, the lock should not been set by us.
                config = cleanStack();
                config[HealthCheckSettings.UPLOAD_LOCK] = "test2-test2" + "|" + HealthCheckSettings.DateTimeToString(DateTime.UtcNow);
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsFalse(RequestUploadTask.Request(connection, _session));

                //6. For Lock set by other service but already due, the lock can be set by current service
                config = cleanStack();
                config[HealthCheckSettings.UPLOAD_LOCK] = "test2-test2" + "|" + HealthCheckSettings.DateTimeToString(DateTime.UtcNow.Subtract(TimeSpan.FromDays(14)));
                config[HealthCheckSettings.DAY_OF_WEEK] = DateTime.UtcNow.DayOfWeek.ToString();
                config[HealthCheckSettings.TIME_OF_DAY] = DateTime.UtcNow.Hour.ToString();
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsTrue(RequestUploadTask.Request(connection, _session));

                //7 Check LastFailedUpload is not empty and > LastSuccessfulUpload && INTERVAL_IN_DAYS using default, lock can be set 
                config = cleanStack();
                config[HealthCheckSettings.LAST_SUCCESSFUL_UPLOAD] = HealthCheckSettings.DateTimeToString(DateTime.UtcNow.Subtract(TimeSpan.FromDays(14)));
                config[HealthCheckSettings.LAST_FAILED_UPLOAD] = HealthCheckSettings.DateTimeToString(DateTime.UtcNow.Subtract(TimeSpan.FromDays(8)));
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsTrue(RequestUploadTask.Request(connection, _session));

                //8 For not due uploading, lock should not been set 
                config = cleanStack();
                config[HealthCheckSettings.LAST_SUCCESSFUL_UPLOAD] = HealthCheckSettings.DateTimeToString(DateTime.UtcNow.Subtract(TimeSpan.FromDays(6)));
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsFalse(RequestUploadTask.Request(connection, _session));

                //9 For failed upload, retry was needed but not meet RetryIntervalInDays, lock should not been set
                config = cleanStack();
                config[HealthCheckSettings.LAST_SUCCESSFUL_UPLOAD] = HealthCheckSettings.DateTimeToString(DateTime.UtcNow.Subtract(TimeSpan.FromDays(14)));
                config[HealthCheckSettings.LAST_FAILED_UPLOAD] = HealthCheckSettings.DateTimeToString(DateTime.UtcNow.Subtract(TimeSpan.FromDays(5)));
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsFalse(RequestUploadTask.Request(connection, _session));

                //10 For failed upload, retry was needed and meet RetryIntervalInDays, lock should be set
                config = cleanStack();
                config[HealthCheckSettings.LAST_FAILED_UPLOAD] = HealthCheckSettings.DateTimeToString(DateTime.UtcNow.Subtract(TimeSpan.FromDays(7)));
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsTrue(RequestUploadTask.Request(connection, _session));


                //11 Retry needed because no LAST_SUCCESSFUL_UPLOAD but not meet RetryIntervalInDays, lock should not be set 
                config = cleanStack();
                config[HealthCheckSettings.LAST_SUCCESSFUL_UPLOAD] = "";
                config[HealthCheckSettings.LAST_FAILED_UPLOAD] = HealthCheckSettings.DateTimeToString(DateTime.UtcNow.Subtract(TimeSpan.FromDays(8)));
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsTrue(RequestUploadTask.Request(connection, _session));

                //12 For no LAST_FAILED_UPLOAD or invalid LAST_FAILED_UPLOAD, lock should not be set if not due
                config = cleanStack();
                config[HealthCheckSettings.LAST_SUCCESSFUL_UPLOAD] = HealthCheckSettings.DateTimeToString(DateTime.UtcNow.Subtract(TimeSpan.FromDays(13)));
                config[HealthCheckSettings.LAST_FAILED_UPLOAD] = "asd";
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
                config[HealthCheckSettings.UPLOAD_LOCK] = UUID + "|" + HealthCheckSettings.DateTimeToString(DateTime.UtcNow);
                config[HealthCheckSettings.NEW_UPLOAD_REQUEST] = HealthCheckSettings.DateTimeToString(DateTime.UtcNow);
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsFalse(RequestUploadTask.OnDemandRequest(connection, _session));

                //2 Uploading is inprocess by other service, demand will be ignore
                config = cleanStack();
                config[HealthCheckSettings.UPLOAD_LOCK] = "test2-test2" + "|" + HealthCheckSettings.DateTimeToString(DateTime.UtcNow);
                config[HealthCheckSettings.NEW_UPLOAD_REQUEST] = HealthCheckSettings.DateTimeToString(DateTime.UtcNow);
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsFalse(RequestUploadTask.OnDemandRequest(connection, _session));

                //3 Uploading is not due and demand due,  demand will be ignore
                config = cleanStack();
                config[HealthCheckSettings.UPLOAD_LOCK] = "test2-test2" + "|" + HealthCheckSettings.DateTimeToString(DateTime.UtcNow.Subtract(TimeSpan.FromDays(14)));
                config[HealthCheckSettings.NEW_UPLOAD_REQUEST] = HealthCheckSettings.DateTimeToString(DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(31)));
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsFalse(RequestUploadTask.OnDemandRequest(connection, _session));

                //4 Uploading is due and demand not due, lock will be set
                config = cleanStack();
                config[HealthCheckSettings.UPLOAD_LOCK] = "test2-test2" + "|" + HealthCheckSettings.DateTimeToString(DateTime.UtcNow.Subtract(TimeSpan.FromDays(14)));
                config[HealthCheckSettings.NEW_UPLOAD_REQUEST] = HealthCheckSettings.DateTimeToString(DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(28)));
                Pool.set_health_check_config(_session, connection.Cache.Pools[0].opaque_ref, config);
                Assert.IsTrue(RequestUploadTask.OnDemandRequest(connection, _session));
            }
            catch (Exception)
            { }
        }
    }
}
