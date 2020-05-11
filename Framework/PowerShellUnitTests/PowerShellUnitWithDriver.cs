using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Magenic.Maqs.BasePowerShellTest;
using System.Collections.Generic;
using System;
using System.Management.Automation;
using System.Net.Http;
using System.IO;

namespace PowerShellUnitTests
{
    /// <summary>
    /// Contains unit tests that test the default implementation of PowerShellMaqs.
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage] 
    public class PowerShellUnitWithDriver: BasePowerShellTest
    {
        [TestMethod]
        public virtual void DriverConfiguredProperly()
        {
        }

        private static IEnumerable<object[]> CommandStreamMapping => new object[][]
        {
            new object[] 
            {
                "Write-Host",  
                (Func<PowerShellDriver,string>) ((driver) => driver.Information.LastOrDefault()?.GetMessageFromRecord())
            },
            new object[] 
            {
                "Write-Information",  
                (Func<PowerShellDriver,string>) ((driver) => driver.Information.LastOrDefault()?.GetMessageFromRecord())
            },
            new object[] 
            {
                "Write-Progress",  
                (Func<PowerShellDriver,string>) ((driver) => driver.Progress.LastOrDefault()?.GetMessageFromRecord())
            },
            new object[] 
            {
                "$VerbosePreference = \"Continue\"; Write-Verbose",  
                (Func<PowerShellDriver,string>) ((driver) => driver.Verbose.LastOrDefault()?.GetMessageFromRecord())
            },
            new object[] 
            {
                "Write-Error", 
                (Func<PowerShellDriver,string>) ((driver) => driver.Error.LastOrDefault()?.GetMessageFromRecord())
            },
            new object[] 
            {
                "$DebugPreference = \"Continue\"; Write-Debug", 
                (Func<PowerShellDriver,string>) ((driver) => driver.Debug.LastOrDefault()?.GetMessageFromRecord())
            }
        };

        [TestMethod]
        public virtual void DebugStreamConfiguredProperly() 
        {
            Assert.IsTrue(PowerShellDriver.TestConnection(), "Could not connect to PowerShell.");

            PowerShellDriver.Invoke("$DebugPreference = \"Continue\"; Write-Host $DebugPreference");

            Assert.AreEqual("Continue", PowerShellDriver.Information.LastOrDefault()?.GetMessageFromRecord());
        }

        [TestMethod]
        public virtual void VerboseStreamConfiguredProperly() 
        {
            Assert.IsTrue(PowerShellDriver.TestConnection(), "Could not connect to PowerShell.");

            PowerShellDriver.Invoke("$VerbosePreference = \"Continue\"; Write-Host $VerbosePreference");

            Assert.AreEqual("Continue", PowerShellDriver.Information.LastOrDefault()?.GetMessageFromRecord());
        }

        [DataTestMethod]
        [DynamicData(nameof(CommandStreamMapping), DynamicDataSourceType.Property)]
        public virtual void AllStreamsEnabled(string command, Func<PowerShellDriver, string> getLastMessageFromStream)
        {
            Assert.IsTrue(PowerShellDriver.TestConnection(), "Could not connect to PowerShell.");

            var testString = "I love .NET";

            PowerShellDriver.Invoke(command + " \"" + testString + "\"");

            var message = getLastMessageFromStream(PowerShellDriver);

            Assert.AreEqual(testString, message, $"Failed to get stream for command {command}");
        }

        [TestMethod]
        public virtual void RunCommandReturnsResults()
        {
            Assert.IsTrue(PowerShellDriver.TestConnection(), "Could not connect to PowerShell.");

            var res = PowerShellDriver.Invoke("Get-Process");

            Assert.AreNotEqual(0, res.Count, "Get-Process returned no results.");
        }

        [TestMethod]
        // This test won't run by default
        [Ignore]
        public virtual void CanRunMultipleCommandsInSession()
        {
            Assert.IsTrue(PowerShellDriver.TestConnection(), "Could not connect to PowerShell.");

            Guid filename = Guid.NewGuid();

            var session = PowerShellDriver.StartRemoteSession("test");
            string homePath = PowerShellDriver.Invoke("(Get-Variable Home).Value").LastOrDefault()?.ToString();
            PowerShellDriver.Invoke($"New-Item -ItemType Directory {Path.Join(homePath, filename.ToString())}", session);
            PowerShellDriver.Invoke($"Set-Location {Path.Join(homePath, filename.ToString())}", session);
            PSDataCollection<PSObject> result = PowerShellDriver.Invoke($"Get-Location", session);

            Assert.AreEqual(
                // Windows to UNIX style paths
                Path.Join(homePath, filename.ToString()).Replace("\\", "/"), 
                result.LastOrDefault()?.ToString(),
                "The sequence of commands did not execute successfully."
            );
        }

        [TestMethod]
        public virtual void TestTimeoutThrowsException()
        {
            Assert.IsTrue(PowerShellDriver.TestConnection(), "Could not connect to PowerShell.");

            Assert.ThrowsException<PowerShellInvocationException>(
                () => PowerShellDriver.Invoke("Start-Sleep -Seconds 100000"),
                "Timeout did not throw an exception."
            );
        }

        [TestMethod]
        public virtual void RunScriptReturnsResults()
        {
            Assert.IsTrue(PowerShellDriver.TestConnection(), "Could not connect to PowerShell.");

            var res = PowerShellDriver.InvokeScript(@".\TestPowerShellScript.ps1");

            Assert.AreNotEqual(res.Count, 0, "Script returned no results.");
            Assert.AreEqual(
                "http://localhost./",
                (res.FirstOrDefault()?.Properties["BaseAddress"].Value).ToString(),
                "Script returned incorrect base object."
            );
        }

        [TestMethod]
        public virtual void RunCommandsReturnsResults()
        {
            Assert.IsTrue(PowerShellDriver.TestConnection(), "Could not connect to PowerShell.");

            var res = PowerShellDriver.Invoke("$test = New-Object -TypeName System.Net.Http.HttpClient; $test.BaseAddress = \"http://localhost.\"; $test");

            Assert.AreNotEqual(res.Count, 0, "Script returned no results.");
            Assert.AreEqual(
                "http://localhost./",
                (res.FirstOrDefault()?.Properties["BaseAddress"].Value).ToString(),
                "Script returned incorrect base object."
            );
        }
    }
}