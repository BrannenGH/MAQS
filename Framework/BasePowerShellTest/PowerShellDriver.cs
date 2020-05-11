//--------------------------------------------------
// <copyright file="PowerShellDriver.cs" company="Magenic">
//  Copyright 2020 Magenic, All rights Reserved
// </copyright>
// <summary>A wrapper for PowerShell</summary>
//-------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace Magenic.Maqs.BasePowerShellTest
{
    /// <summary>
    /// A C# interface for PowerShell to make executing PowerShell
    /// scripts and command from UnitTests easier.
    /// </summary>
    public class PowerShellDriver: IDisposable
    {
        /// <summary>
        /// How long until the PowerShell command times out.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// The remote hostname to connect to if using SSH.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// The remote username to connect as.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The path to the key file to use to initialize
        /// the SSH session. 
        /// </summary>
        public string KeyFilePath { get; set; }

        /// <summary>
        /// The name of the computer to connect to.
        /// </summary>
        public string ComputerName { get; set; }

        /// <summary>
        /// The connection type formed by Inovking a command.
        /// </summary>
        public ConnectionTypeEnum ConnectionType { get; set; }

        private List<PowerShellSession> sessions = new List<PowerShellSession>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PowerShellDriver" /> class.
        /// </summary>
        /// <param name="powerShell">The PowerShell instance to wrap.</param>
        /// <param name="timeout">The timeout value to use per command.</param>
        public PowerShellDriver(PowerShell powerShell, TimeSpan timeout)
        {
            PowerShell = powerShell;
            ConnectionType = ConnectionTypeEnum.Local;
        }

        /// <summary>
        /// Gets the PowerShell instance
        /// </summary>
        public PowerShell PowerShell { get; private set; }

        /// <summary>
        /// Information written to the Information stream (STDOUT).
        /// </summary>
        public PSDataCollection<InformationRecord> Information => PowerShell.Streams.Information;

        /// <summary>
        /// Informaiton written to the Verbose stream.
        /// </summary>
        public PSDataCollection<VerboseRecord> Verbose => PowerShell.Streams.Verbose;

        /// <summary>
        /// Information written to the Warning stream.
        /// </summary>
        public PSDataCollection<WarningRecord> Warning => PowerShell.Streams.Warning;

        /// <summary>
        /// Information written to the Debug stream.
        /// </summary>
        public PSDataCollection<DebugRecord> Debug => PowerShell.Streams.Debug;

        /// <summary>
        /// Information written to the Progress stream.
        /// </summary>
        public PSDataCollection<ProgressRecord> Progress => PowerShell.Streams.Progress;

        /// <summary>
        /// Information written to the Error stream (STDERR).
        /// </summary>
        public PSDataCollection<ErrorRecord> Error => PowerShell.Streams.Error;

        /// <summary>
        /// Wrap the HadErrors PowerShell field.
        /// </summary>
        public bool HadErrors => PowerShell.HadErrors;

        /// <summary>
        /// Invoke a command.
        /// </summary>
        public PSDataCollection<PSObject> Invoke(
            string command, 
            PowerShellSession session = null,
            TimeSpan? timeout = null, 
            bool resetState = true
        ) 
        {
            if (resetState) 
            {
                Reset();
            }

            PowerShell cmd = BuildInvokeCommand(PowerShell, command, session);

            return cmd.Invoke(timeout ?? Timeout);
        }


        /// <summary>
        /// Invoke a command.
        /// </summary>
        public async Task<PSDataCollection<PSObject>> InvokeAsync(
            string command, 
            PowerShellSession session = null,
            TimeSpan? timeout = null, 
            bool resetState = true
        ) 
        {
            if (resetState) 
            {
                Reset();
            }

            PowerShell cmd = BuildInvokeCommand(PowerShell, command, session);

            return await cmd.InvokeAsync(timeout ?? Timeout);
        }

        /// <summary>
        /// Invoke a script that is located on the client machine.
        /// </summary>
        public PSDataCollection<PSObject> InvokeScript(
            string path,
            PowerShellSession session = null,
            TimeSpan? timeout = null, 
            bool resetState = true
        ) 
        {
            if (resetState) 
            {
                Reset();
            }

            PowerShell cmd = BuildInvokeScriptCommand(PowerShell, path, session);

            return cmd.Invoke(timeout ?? Timeout);
        }

        /// <summary>
        /// Invoke a script that is located on the client machine.
        /// </summary>
        public async Task<PSDataCollection<PSObject>> InvokeScriptAsync(
            string path,
            PowerShellSession session = null,
            TimeSpan? timeout = null, 
            bool resetState = true
        ) 
        {
            if (resetState) 
            {
                Reset();
            }

            PowerShell cmd = BuildInvokeScriptCommand(PowerShell, path, session);

            return await cmd.InvokeAsync(timeout ?? Timeout);
        }

        /// <summary>
        /// Starts a new PowerShell session to maintain PowerShell state.
        /// </summary>
        public PowerShellSession StartRemoteSession(
            string sessionName,
            TimeSpan? timeout = null, 
            bool resetState = true
        ) 
        {
            if (resetState) 
            {
                Reset();
            }

            // All PowerShell Interactions are abstracted through Invoke-Command.
            var cmd = PowerShell.AddCommand("New-PSSession")
                // Allows all streams to be read
                .AddParameter("Verbose");

            switch (ConnectionType) 
            {
                case ConnectionTypeEnum.Ssh:
                    cmd = cmd
                        .AddParameter("HostName", HostName)
                        .AddParameter("Username", UserName)
                        .AddParameter("KeyFilePath", KeyFilePath);
                    break;
                    case ConnectionTypeEnum.Wsman:
                        cmd = cmd
                            .AddParameter("ComputerName", ComputerName);
                        break;
                    default:
                        break;
            }

            var session = new PowerShellSession(cmd.Invoke(timeout ?? Timeout)?.LastOrDefault());

            sessions.Add(session);
                
            return session;
        }

        /// <summary>
        /// Ends a PowerShell session.
        /// </summary>
        public void EndRemoteSession(
            PowerShellSession session,
            bool resetState = true
        ) 
        {
            if (resetState) 
            {
                Reset();
            }
            
            var cmd = PowerShell.AddCommand("Remove-PSSession")
                .AddParameter("Session", session.Session);

            cmd.Invoke(Timeout);

            session.IsOpen = false;
        }

        private PowerShell BuildInvokeCommand(PowerShell cmd, string command, PowerShellSession session = null)
        {
            // All PowerShell Interactions are abstracted through Invoke-Command.
            cmd = cmd.AddCommand("Invoke-Command")
                // Allows all streams to be read, Verbose and Debug are silent by default.
                // This allows both to be read.
                .AddParameter("Verbose");

            if (session != null && session.Session != null) 
            {
                cmd = cmd.AddParameter("Session", session.Session);
            }
            else 
            {
                switch (ConnectionType) 
                {
                    case ConnectionTypeEnum.Ssh:
                        cmd = cmd
                            .AddParameter("HostName", HostName)
                            .AddParameter("Username", UserName)
                            .AddParameter("KeyFilePath", KeyFilePath);
                        break;
                    case ConnectionTypeEnum.Wsman:
                        cmd = cmd
                            .AddParameter("ComputerName", ComputerName);
                        break;
                    default:
                        break;
                }
            }
            
            cmd = cmd.AddParameter("ScriptBlock", ScriptBlock.Create(command));

            return cmd;
        }

        private PowerShell BuildInvokeScriptCommand(PowerShell cmd, string scriptPath, PowerShellSession session = null)
        {
            // If it is local, using the FilePath parameter is not supported yet.
            // We run the script in a ScriptBlock instead.
            if (ConnectionType != ConnectionTypeEnum.Local)
            {
                // All PowerShell Interactions are abstracted through Invoke-Command.
                cmd = cmd.AddCommand("Invoke-Command")
                    // Allows all streams to be read
                    .AddParameter("Verbose");

                if (session != null && session.Session != null) 
                {
                    cmd = cmd
                        .AddParameter("Session", session.Session);
                }
                else
                {
                    switch (ConnectionType) 
                    {
                        case ConnectionTypeEnum.Ssh:
                            cmd = cmd
                                .AddParameter("HostName", HostName)
                                .AddParameter("Username", UserName)
                                .AddParameter("KeyFilePath", KeyFilePath);
                            break;
                        case ConnectionTypeEnum.Wsman:
                            cmd = cmd
                                .AddParameter("ComputerName", ComputerName);
                            break;
                        default:
                            break;
                    }               
                }

                return cmd.AddParameter("FilePath", scriptPath);
            }
            else 
            {
                return cmd.AddScript(scriptPath);
            }
        }

        /// <summary>
        /// Test the connection to the remote server by executing a
        /// <c>Write-Host</c> on the remote PowerShell and verifying it
        /// was output to an internal stream.
        /// </summary>
        /// <returns>
        /// A boolean representing if the command executed without errors.
        /// </returns>
        public bool TestConnection() 
        {
            Reset();

            string test = "Testing Connection...";

            Invoke($"Write-Host {test}");

            return !PowerShell.HadErrors && Information.LastOrDefault()?.GetMessageFromRecord() == test;
        }

        /// <summary>
        /// Reset the PowerShell instance to an initial state.
        /// </summary>
        public void Reset()
        {
            PowerShell.Commands.Clear();
            PowerShell.Streams.ClearStreams();
        }

        /// <summary>
        /// Dispose of PowerShell driver.
        /// </summary>
        public void Dispose()
        {
            foreach (var session in sessions ?? Enumerable.Empty<PowerShellSession>())
            {
                if (session.IsOpen) 
                {
                    EndRemoteSession(session);
                }
            }

            if (PowerShell != null) 
            {
                PowerShell.Dispose();
            }
        }
    }
}