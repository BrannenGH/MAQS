using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Management.Automation;
using Magenic.Maqs.BasePowerShellTest;
using Magenic.Maqs.Utilities.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PowerShellUnitTests
{
    /* 
    * While PowerShell can be installed from a NuGet package
    * the use of OpenSSH requires SSH be installed on the machine,
    * as it is opened by PowerShell as a seperate process. 
    * See https://github.com/PowerShell/PowerShell/blob/b1e998046e12ebe5da9dee479f20d479aa2256d7/src/System.Management.Automation/engine/remoting/common/RunspaceConnectionInfo.cs#L2416
    */

    /// <summary>
    /// Executes all the tests in <see cref="PowerShellUnitWithDriver" />,
    /// just in a remote SSH instance.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PowerShellUnitWithDriverSshRemote: PowerShellUnitWithDriver
    {
        [TestMethod]
        [TestCategory(TestCategories.PowerShell)]
        public override void DriverConfiguredProperly()
        {
            Assert.AreEqual(ConnectionTypeEnum.Ssh, PowerShellDriver.ConnectionType, "Driver is not configured properly.");
        }

        [TestMethod]
        [TestCategory(TestCategories.PowerShell)]
        public override void CanRunMultipleCommandsInSession()
        {
            base.CanRunMultipleCommandsInSession();
        }

        protected override PowerShellDriver GetPowerShellDriver() 
        {
            var driver = PowerShellDriverFactory.BuildPowerShellDriver();

            driver.ConnectionType = ConnectionTypeEnum.Ssh;
            driver.UserName = "root";
            driver.HostName = "localhost:2200";
            driver.KeyFilePath = "docker_linux.rsa";

            return driver;
        } 
    }
}