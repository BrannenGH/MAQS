using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Magenic.Maqs.BasePowerShellTest;
using Magenic.Maqs.Utilities.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PowerShellUnitTests
{    
    /// <summary>
    /// Executes all the tests in <see cref="PowerShellUntWithDriver" />,
    /// just in a remote WSMAN instance.
    /// </summary>
    [TestClass]
    [Ignore]
    [ExcludeFromCodeCoverage]
    public class PowerShellUnitWithDriverWsMan: PowerShellUnitWithDriver
    {
        [TestMethod]
        [TestCategory(TestCategories.PowerShell)]
        public override void DriverConfiguredProperly()
        {
            Assert.AreEqual(ConnectionTypeEnum.Wsman, PowerShellDriver.ConnectionType, "Driver is not configured properly.");
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

            driver.ConnectionType = ConnectionTypeEnum.Wsman;
            driver.ComputerName = "localhost";

            return driver;
        } 
    }
}