//--------------------------------------------------
// <copyright file="PowerShellInvocationException.cs" company="Magenic">
//  Copyright 2020 Magenic, All rights Reserved
// </copyright>
// <summary>The exception</summary>
//-------------------------------------------------
using System;

namespace Magenic.Maqs.BasePowerShellTest
{
    /// <summary>
    /// Exception for when PowerShell invocation fails.
    /// </summary>
    public class PowerShellInvocationException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public PowerShellInvocationException(string message, Exception innerException = null) 
            : base(message, innerException)
        {
        }
    }
}