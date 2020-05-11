//--------------------------------------------------
// <copyright file="BasePowerShellTest.cs" company="Magenic">
//  Copyright 2020 Magenic, All rights Reserved
// </copyright>
// <summary>This is the base PowerShell test class</summary>
//-------------------------------------------------
using Magenic.Maqs.BaseTest;
using Magenic.Maqs.Utilities.Logging;

namespace Magenic.Maqs.BasePowerShellTest
{
    /// <summary>
    /// Generic base PowerShell test
    /// </summary>
    public class BasePowerShellTest: BaseExtendableTest<PowerShellTestObject>
    {

        /// <summary>
        /// Gets or sets the PowerShell driver
        /// </summary>
        public PowerShellDriver PowerShellDriver
        {
            get
            {
                return this.TestObject.PowerShellDriver;
            }

            set
            {
                this.TestObject.OverridePowerShellDriver(value);
            }
        }

        /// <summary>
        /// Create a new PowerShellTestObject.
        /// </summary>
        protected override void CreateNewTestObject() 
        {
            Logger newLogger = this.CreateLogger();
            this.TestObject = new PowerShellTestObject(
                () => this.GetPowerShellDriver(), 
                newLogger, 
                new SoftAssert(newLogger), 
                this.GetFullyQualifiedTestClassName()
            );
        }

        /// <summary>
        /// Builds a new PowerShellDriver
        /// </summary>
        protected virtual PowerShellDriver GetPowerShellDriver() 
        {
            return PowerShellDriverFactory.BuildPowerShellDriver();
        }
    }
}
