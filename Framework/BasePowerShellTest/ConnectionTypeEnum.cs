//--------------------------------------------------
// <copyright file="ConnectionTypeEnum.cs" company="Magenic">
//  Copyright 2020 Magenic, All rights Reserved
// </copyright>
// <summary>The PowerShell connection types supported by MAQS</summary>
//-------------------------------------------------
namespace Magenic.Maqs.BasePowerShellTest
{
    /// <summary>
    /// The type of connection to form with PowerShell
    /// </summary>
    public enum ConnectionTypeEnum
    {
        /// <summary>
        /// Run all PowerShell commands within the context of the current system.
        /// </summary>
        Local,
        /// <summary>
        /// Run all PowerShell commands within the context of a remote SSH connection.
        /// </summary>
        Ssh,
        /// <summary>
        /// Run all PowerShell commands within the context of a remote WSMAN connection.
        /// </summary>
        Wsman
    }
}