//--------------------------------------------------
// <copyright file="PowerShellConfigException.cs" company="Magenic">
//  Copyright 2020 Magenic, All rights Reserved
// </copyright>
// <summary>This is the base PowerShell test class</summary>
//-------------------------------------------------
using System;

namespace Magenic.Maqs.BasePowerShellTest
{
    /// <summary>
    /// An exception for when PowerShell cannot be instantiated.
    /// </summary>
    public class PowerShellConfigException: Exception
    {
        private const string defaultMessage = "Could not start PowerShell. " +
            "Could not find a referenced PowerShell implementation. " +
            "Please reference a DLL containing a PowerShell implementation.";

        /// <summary>
        /// Constructor
        /// </summary>
        public PowerShellConfigException(string message = null): base(message ?? defaultMessage)
        {
        }
    }
}