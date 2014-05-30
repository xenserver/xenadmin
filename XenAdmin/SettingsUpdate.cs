using System;
using System.Reflection;
using XenAdmin;
using System.IO;

public static class SettingsUpdate
{
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    // The path of the user.config files looks something like this:
    // <Profile Directory>\<Company Name>\<App Name>_<Evidence Type>_<Evidence Hash>\<Version>\user.config

    /// <summary>
    /// Looks for a config file from a previous installation of the application and updates the settings from it.
    /// </summary>
    public static void Update()
    {
        try
        {
            Assembly a = Assembly.GetExecutingAssembly();
            Version appVersion = a.GetName().Version;

            // get previous config file by enumerating through all the folders in <Profile Directory>\<Company Name> 
            // to find a previous user.config file

            var currentConfigFolder = new DirectoryInfo(Settings.GetUserConfigPath()).Parent;

            if (currentConfigFolder == null)
                return;

            var appDomainName = AppDomain.CurrentDomain.FriendlyName;
            var companyFolder = currentConfigFolder.Parent != null ? currentConfigFolder.Parent.Parent : null;

            if (companyFolder == null)
                return;

            FileInfo previousConfig = null;
            Version previousVersion = null;

            foreach (var subDir in companyFolder.GetDirectories("*" + appDomainName + "*", SearchOption.AllDirectories))
            {
                foreach (var file in subDir.GetFiles("user.config", SearchOption.AllDirectories))
                {
                    var configFolderName = Path.GetFileName(Path.GetDirectoryName(file.FullName));
                    if (configFolderName != null)
                    {
                        var configVersion = new Version(configFolderName);

                        if ((configVersion <= appVersion) && (previousVersion == null || configVersion > previousVersion))
                        {
                            previousVersion = configVersion;
                            previousConfig = file;
                        }
                    }
                }
            }
            
            if (previousConfig != null)
            {
                // copy previous config file to current config location
                var destinationFile = Path.GetDirectoryName(currentConfigFolder.FullName);

                destinationFile = Path.Combine(destinationFile, previousVersion.ToString());

                if (!Directory.Exists(destinationFile))
                    Directory.CreateDirectory(destinationFile);

                destinationFile = Path.Combine(destinationFile, previousConfig.Name);

                File.Copy(previousConfig.FullName, destinationFile);

                // upgrade settings
                XenAdmin.Properties.Settings.Default.Upgrade();
            }
        }
        catch (Exception ex)
        {
            log.DebugFormat("Exception while updating settings: {0}", ex.Message);
        }
    }
}
