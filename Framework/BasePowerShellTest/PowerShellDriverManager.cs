//--------------------------------------------------
// <copyright file="PowerShellDriverManager.cs" company="Magenic">
//  Copyright 2020 Magenic, All rights Reserved
// </copyright>
// <summary>An object for managing PowerShell drivers</summary>
//------------------------------------------------
using System;
using System.Management.Automation;
using Magenic.Maqs.BaseTest;
using Magenic.Maqs.Utilities.Logging;

namespace Magenic.Maqs.BasePowerShellTest
{
    /// <summary>
    /// PowerShellDriver manager.
    /// </summary>
    public class PowerShellDriverManager : DriverManager
    {
        /// <summary>
        /// Cached copy of the connection driver.
        /// </summary>
        private PowerShellDriver driver;

        /// <summary>
        /// Instantiates a new PowerShellDriverManager.
        /// </summary>
        /// <param name="funcToRun">Function to get/build the PowerShellDriver.</param>
        /// <param name="testObject">The test object.</param>
        public PowerShellDriverManager(Func<PowerShellDriver> funcToRun, BaseTestObject testObject) : base(funcToRun, testObject)
        {
        }

        /// <summary>
        /// Get the PowerShell driver
        /// </summary>
        public override object Get()
        {
            return this.GetPowerShellDriver();
        }

        /// <summary>
        /// Get the PowerShell driver
        /// </summary>
        /// <returns>The PowerShell driver</returns>
        public PowerShellDriver GetPowerShellDriver()
        {
            PowerShellDriver driver = GetBase() as PowerShellDriver;

            if (this.driver == null)
            {
                this.driver = driver;
                this.MapEvents(this.driver);
            }

            return this.driver;
        }

        /// <summary>
        /// Dispose of PowerShellDriverManager.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /// <summary>
        /// Explicitly exposes the driver if it is not null.
        /// </summary>
        protected override void DriverDispose()
        {
            if (this.driver != null)
            {
                this.driver.Dispose();
            }

            this.driver = null;
        }

        /// <summary>
        /// Map PowerShell events to logger.
        /// </summary>
        /// <param name="driver">The driver that we want mapped.</param>
        private void MapEvents(PowerShellDriver driver)
        {
            driver.Error.DataAdding += new EventHandler<DataAddingEventArgs>(OnLogFactory(MessageType.ERROR));
            driver.Information.DataAdding += new EventHandler<DataAddingEventArgs>(OnLogFactory(MessageType.INFORMATION));
            driver.Progress.DataAdding += new EventHandler<DataAddingEventArgs>(OnLogFactory(MessageType.VERBOSE));
            driver.Verbose.DataAdding += new EventHandler<DataAddingEventArgs>(OnLogFactory(MessageType.VERBOSE));
            driver.Warning.DataAdding += new EventHandler<DataAddingEventArgs>(OnLogFactory(MessageType.WARNING));
            driver.Debug.DataAdding += new EventHandler<DataAddingEventArgs>(OnLogFactory(MessageType.VERBOSE));
        }

        /// <summary>
        /// Add stream to be logged.
        /// </summary>
        private Action<object, DataAddingEventArgs> OnLogFactory(MessageType type) 
        {
            return (sender, e) => 
            {
                this.Log.LogMessage(type, e.ItemAdded.GetMessageFromRecord());
            };
        }

        /// <summary>
        /// Override the PowerShell driver.
        /// </summary>
        /// <param name="driver">The function to get/build a new PowerShellDriver.</param>
        public void OverrideDriver(Func<PowerShellDriver> driver)
        {
            this.driver = null;
            this.OverrideDriverGet(driver);
        }
    }
}