//--------------------------------------------------
// <copyright file="PowerShellSession.cs" company="Magenic">
//  Copyright 2020 Magenic, All rights Reserved
// </copyright>
// <summary>An abstraction over PowerShell sessions.</summary>
//-------------------------------------------------
using System;
using System.Management.Automation;

namespace Magenic.Maqs.BasePowerShellTest
{
    /// <summary>
    /// An abstraction that wraps a PowerShell session.
    /// </summary>
    public class PowerShellSession
    {
        /// <summary>
        /// A PSObject containing a PowerShell session.
        /// </summary>
        public PSObject Session { get; set; }

        /// <summary>
        /// Keep track of session state.
        /// </summary>
        /* TODO: This should be programatically generated from the Session PSObject. */
        public bool IsOpen { get; set; }

        /// <summary>
        /// Instantiate a new <see cref="PowerShellSession" />.
        /// </summary>
        public PowerShellSession(PSObject session)
        {
            Session = session;
            IsOpen = true;
        }
    }
}