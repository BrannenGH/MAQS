using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Management.Automation;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PowerShellUnitTests
{
    /// <summary>
    /// A copy of each unit test from <see cref="PowerShellUnitWithDriver"> is
    /// here, just without using MAQS.
    /// </summary> 
    [TestClass]
    [ExcludeFromCodeCoverage] 
    public class PowerShellUnitWithoutDriver
    {
        private static IEnumerable<object[]> CommandStreamMapping => new object[][]
        {
            new object[] 
            {
                "Write-Host",  
                (Func<PowerShell,string>) ((pwsh) => pwsh.Streams.Information.LastOrDefault().ToString())
            },
            new object[] 
            {
                "Write-Information",  
                (Func<PowerShell,string>) ((pwsh) => pwsh.Streams.Information.LastOrDefault().ToString())
            },
            new object[] 
            {
                "Write-Progress",  
                (Func<PowerShell,string>) ((pwsh) => pwsh.Streams.Progress.LastOrDefault().Activity)
            },
            new object[] 
            {
                "Write-Error", 
                (Func<PowerShell,string>) ((pwsh) => pwsh.Streams.Error.LastOrDefault().ToString())
            }
        };


        [DataTestMethod]
        [DynamicData(nameof(CommandStreamMapping), DynamicDataSourceType.Property)]
        public virtual void AllStreamsEnabled(string command, Func<PowerShell, string> getLastMessageFromStream)
        {
            var testString = "I love .NET";
        
            var pwsh = PowerShell.Create();

            pwsh.AddCommand(command);
            
            pwsh.AddArgument(testString);

            pwsh.Invoke();
            
            var message = getLastMessageFromStream(pwsh);

            Assert.AreEqual(testString, message, $"{message ?? "null"} does not match {testString ?? "null"}");
        }

        [TestMethod]
        public virtual void RunCommandReturnsResults()
        {
            var pwsh = PowerShell.Create();

            pwsh.AddCommand("Get-Process");

            var res = pwsh.Invoke();

            Assert.AreNotEqual(0, res.Count, "Get-Process returned no results.");
        }

        [TestMethod]
        public virtual void RunScriptReturnsResults()
        {
            var pwsh = PowerShell.Create();

            pwsh.AddScript(@".\TestPowerShellScript.ps1");

            var res = pwsh.Invoke();

            Assert.AreNotEqual(res.Count, 0, "Script returned no results.");
            Assert.AreEqual(
                "http://localhost./",
                (res.FirstOrDefault()?.ImmediateBaseObject as HttpClient).BaseAddress.ToString(),
                "Script returned incorrect base object."
            );
        }
    }
}