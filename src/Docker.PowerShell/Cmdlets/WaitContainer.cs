﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Management.Automation;
using Docker.DotNet;
using Docker.PowerShell.Objects;

namespace Docker.PowerShell.Cmdlets
{
    [Cmdlet("Wait", "Container",
            DefaultParameterSetName = CommonParameterSetNames.Default)]
    public class WaitContainer : ContainerOperationCmdlet
    {
        #region Parameters

        /// <summary>
        /// If specified, the resulting container object will be output after the operation has
        /// finished.
        /// </summary>
        [Parameter]
        public virtual SwitchParameter PassThru { get; set; }

        #endregion

        #region Overrides

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            foreach (var entry in ParameterResolvers.GetContainerIdMap(Container, Id, HostAddress))
            {
                HostAddress = entry.Value;

                var waitResponse = DkrClient.Containers.WaitContainerAsync(
                    entry.Key,
                    CancelSignal.Token).AwaitResult();

                if (PassThru.ToBool())
                {
                    WriteVerbose("Status Code: " + waitResponse.StatusCode.ToString());

                    WriteObject(ContainerOperations.GetContainerById(entry.Key, DkrClient));

                }
                else
                {
                    WriteObject(waitResponse.StatusCode);
                }
            }
        }

        #endregion
    }
}