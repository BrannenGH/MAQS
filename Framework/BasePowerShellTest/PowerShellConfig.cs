//--------------------------------------------------
// <copyright file="ConnectionTypeEnum.cs" company="Magenic">
//  Copyright 2020 Magenic, All rights Reserved
// </copyright>
// <summary>The PowerShell configuration abstraction</summary>
//-------------------------------------------------
using System;
using System.Collections.Generic;
using Magenic.Maqs.Utilities.Helper;

namespace Magenic.Maqs.BasePowerShellTest
{
    /// <summary>
    /// Utility that wraps the IConfiguration PowerShell implementation.
    /// </summary>
    public static class PowerShellConfig
    {
        /// <summary>
        /// Loads when class is loaded
        /// </summary>
        static PowerShellConfig()
        {
            CheckConfig();
        }

        /// <summary>
        /// Ensure required fields are in the config
        /// </summary>
        private static void CheckConfig()
        {
            var validator = new ConfigValidation();

            switch (GetConnectionType()) 
            {
                case ConnectionTypeEnum.Ssh:
                    validator.RequiredFields = new List<string>
                    {
                        // Only hostname is required, because one can
                        // write a unix style SSH hostname — root@127.0.0.1:2020
                        "HostName",
                        "KeyFilePath"
                    };
                    break;
                case ConnectionTypeEnum.Wsman:
                    validator.RequiredFields = new List<string>
                    {
                        "ComputerName"
                    };
                    break;
                default:
                    validator.RequiredFields = new List<string>
                    {
                    };
                    break;
            }

            Config.Validate(ConfigSection.PowerShellMaqs, validator);
        }

        /// <summary>
        /// Static name for the PowerShell configuration section.
        /// </summary>
        private const string POWERSHELLMAQS = "PowerShellMaqs";

        /// <summary>
        /// Get the connection type.
        /// </summary>
        public static ConnectionTypeEnum GetConnectionType()
        {
            switch (Config.GetValueForSection(POWERSHELLMAQS, "ConnectionType"))
            {
                case "SSH":
                    return ConnectionTypeEnum.Ssh;
                case "WSMAN":
                    return ConnectionTypeEnum.Wsman;
                default:
                    return ConnectionTypeEnum.Local;
            }
        }

        /// <summary>
        /// Get the remote user to use for the SSH Connection.
        /// </summary>
        public static string GetUser()
        {
            return Config.GetValueForSection(POWERSHELLMAQS, "User");
        }

        /// <summary>
        /// Get the Timeout from the configuration.
        /// </summary>
        public static TimeSpan GetTimeout()
        {
            ulong timeout;
            if (ulong.TryParse(Config.GetValueForSection(POWERSHELLMAQS, "Timeout", "60000"), out timeout))
            {
                return TimeSpan.FromMilliseconds(timeout);
            }
            else 
            {
                return TimeSpan.FromMilliseconds(60000);
            }
        }

        /// <summary>
        /// Get the Hostname for an SSH connection.
        /// </summary>
        public static string GetHostname()
        {
            return Config.GetValueForSection(POWERSHELLMAQS, "Hostname");
        }

        /// <summary>
        /// Get the Username for an SSH connection.
        /// </summary>
        public static string GetUsername()
        {
            return Config.GetValueForSection(POWERSHELLMAQS, "Username");
        }

        /// <summary>
        /// Get the ComputerName for a WSMAN connection.
        /// </summary>
        public static string GetComputerName()
        {
            return Config.GetValueForSection(POWERSHELLMAQS, "ComputerName");
        }

        /// <summary>
        /// Get the path to the keyfile for an SSH connection.
        /// </summary>
        /// <returns>The path to the keyfile, or null if unspecified.</returns>
        public static string GetKeyFilePath() 
        {
            return Config.GetValueForSection(POWERSHELLMAQS, "KeyFilePath");
        }
    }
}