using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Projector.Enums
{
    /// <summary>
    /// Specifies the types of machines or processes used in production operations.
    /// </summary>
    public enum EnumMachineType
    {
        InspectionMachine = 0,
        MachineProcess = 1,
        ManualProcess = 2,
        ManualInspection = 3,
        InscpectionProcess = 4
    }
}
