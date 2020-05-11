//--------------------------------------------------
// <copyright file="PowerShellTestObject.cs" company="Magenic">
//  Copyright 2020 Magenic, All rights Reserved
// </copyright>
// <summary>Test object for maintaining PowerShell driver instance</summary>
//------------------------------------------------
using System;
using Magenic.Maqs.BaseTest;
using Magenic.Maqs.Utilities.Logging;

namespace Magenic.Maqs.BasePowerShellTest
{
    /// <summary>
    /// Database test context data
    /// </summary>
    public class PowerShellTestObject : BaseTestObject 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PowerShellTestObject" /> class
        /// </summary>
        /// <param name="powerShell">The function to build a <see cref="PowerShellDriver" /> class</param>
        /// <param name="logger">The test's logger</param>
        /// <param name="softAssert">The test's soft assert</param>
        /// <param name="fullyQualifiedTestName">The test's fully qualified test name</param>
        public PowerShellTestObject(Func<PowerShellDriver> powerShell, Logger logger, SoftAssert softAssert, string fullyQualifiedTestName) : base(logger, softAssert, fullyQualifiedTestName)
        {
            this.ManagerStore.Add(typeof(PowerShellDriverManager).FullName, new PowerShellDriverManager(powerShell, this));
        }

        /// <summary>
        /// Get the PowerShell driver manager.
        /// </summary>
        public PowerShellDriverManager PowerShellManager
        {
            get
            {
                return this.ManagerStore[typeof(PowerShellDriverManager).FullName] as PowerShellDriverManager;
            }
        }

        /// <summary>
        /// Gets the PowerShell driver
        /// </summary>
        public PowerShellDriver PowerShellDriver
        {
            get
            {
                return this.PowerShellManager.GetPowerShellDriver();
            }
        }

        /// <summary>
        /// Provide a new function to use to initalize the <see cref="PowerShellDriver">Power Shell driver</see>.
        /// </summary>
        /// <param name="driver">
        /// The function to get/build the <see cref="PowerShellDriver">Power Shell driver</see>.
        /// </param>
        public void OverridePowerShellDriver(PowerShellDriver driver)
        {
            this.PowerShellManager.OverrideDriver(() => driver);
        }
    }
}