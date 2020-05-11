//--------------------------------------------------
// <copyright file="PowerShellExtensions.cs" company="Magenic">
//  Copyright 2020 Magenic, All rights Reserved
// </copyright>
// <summary></summary>
//------------------------------------------------
using System;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.Timeout;

namespace Magenic.Maqs.BasePowerShellTest
{
    /// <summary>
    /// Extension methods for PowerShell Objects.
    /// </summary>
    public static class PowerShellExtensions
    {
        /// <summary>
        /// Invoke PowerShell cmdlet with a timeout.
        /// </summary>
        /// <param name="powerShell">The PowerShell instance to invoke on.</param>
        /// <param name="timeout">The period to throw a timeout exception if the command is not finished yet.</param>
        public static PSDataCollection<PSObject> Invoke(this PowerShell powerShell, TimeSpan timeout)
        {
            var timeoutPolicy = Policy.Timeout(timeout, TimeoutStrategy.Pessimistic);

            try {
                return timeoutPolicy.Execute(
                    () => new PSDataCollection<PSObject>(powerShell.Invoke())
                );
            } 
            catch (TimeoutRejectedException e) 
            {
                throw new PowerShellInvocationException("Invocation timed out.", e);
            }
            catch (Exception e)
            {
                throw new PowerShellInvocationException("Invocation failed.", e);
            }
        } 
        
        /// <summary>
        /// Invoke PowerShell cmdlet with a timeout.
        /// </summary>
        /// <param name="powerShell">The PowerShell instance to invoke on.</param>
        /// <param name="timeout">The period to throw a timeout exception if the command is not finished yet.</param>
        public static async Task<PSDataCollection<PSObject>> InvokeAsync(this PowerShell powerShell, TimeSpan timeout) 
        {
            /*
             * While certain versions of PowerShell would support directly
             * using cancelation tokens and modern async/await, older versions of PowerShell
             * won't necesarilly support that interface.
             */
            var timeoutPolicy = Policy.TimeoutAsync(timeout, TimeoutStrategy.Pessimistic);

            try {
                return await timeoutPolicy.ExecuteAsync(
                    () => Task<PSDataCollection<PSObject>>.Factory.FromAsync(
                            powerShell.BeginInvoke(), 
                            powerShell.EndInvoke 
                    )
                );
            } 
            catch (TimeoutRejectedException e) 
            {
                throw new PowerShellInvocationException("Invocation timed out.", e);
            }
            catch (Exception e)
            {
                throw new PowerShellInvocationException("Invocation failed.", e);
            }
        }

        /// <summary>
        /// Returns a string representing the message displayed to the console for the
        /// respective record.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// If the record parameter is not a PowerShell Record type (i.e. ErrorRecord, InformationRecord),
        /// an exception is thrown.
        /// </exception>
        /// <param name="record">
        /// The PowerShell record to get the message from.
        /// </param>
        public static string GetMessageFromRecord(this object record) 
        {
            Type t = record.GetType();

            switch (record) {
                case InformationRecord info:
                    return info.ToString();
                case ErrorRecord error:
                    return error.ToString();
                case ProgressRecord progress:
                    return progress.Activity;
                case VerboseRecord verbose:
                    return verbose.ToString();
                case DebugRecord debug:
                    return debug.ToString();
                default:
                    throw new ArgumentException("Object is not a Record.");
            }
        }
    }
}