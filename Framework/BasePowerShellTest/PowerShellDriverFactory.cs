//--------------------------------------------------
// <copyright file="PowerShellDriverFactory.cs" company="Magenic">
//  Copyright 2020 Magenic, All rights Reserved
// </copyright>
// <summary>A utility for creating new PowerShellDriver objects</summary>
//-------------------------------------------------
using System;
using System.Management.Automation;
using Magenic.Maqs.Utilities.Logging;

namespace Magenic.Maqs.BasePowerShellTest
{
    /// <summary>
    /// A utility for instantiating new <see cref="PowerShellDriver" />.
    /// </summary>
    public static class PowerShellDriverFactory
    {

        /// <summary>
        /// Returns a default implementation of PowerShell
        /// </summary>
        public static PowerShellDriver BuildPowerShellDriver() 
        {
            var powerShell = PowerShell.Create();

            if (powerShell == null) 
            {
                throw new PowerShellConfigException();
            }

            PowerShellDriver driver = new PowerShellDriver(powerShell, TimeSpan.FromMilliseconds(1000));

            ConfigurePowerShellDriver(driver);

            return driver;
        }

        /// <summary>
        /// Reconfigures a PowerShellDriver to respect the settings set in
        /// the app.config or appconfig.json.
        /// </summary>
        public static void ConfigurePowerShellDriver(PowerShellDriver driver)
        {
            driver.Timeout = PowerShellConfig.GetTimeout();
            driver.ConnectionType = PowerShellConfig.GetConnectionType();
            switch (PowerShellConfig.GetConnectionType()) 
            {
                case ConnectionTypeEnum.Ssh:
                    driver.HostName = PowerShellConfig.GetHostname();
                    driver.KeyFilePath = PowerShellConfig.GetKeyFilePath();
                    break;
                case ConnectionTypeEnum.Wsman:
                    driver.ComputerName = PowerShellConfig.GetComputerName();
                    break;
                default:
                    break;
            }
        }
    }
}