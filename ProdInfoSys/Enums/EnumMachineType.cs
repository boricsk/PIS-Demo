namespace ProdInfoSys.Enums
{
    /// <summary>
    /// Specifies the types of machines or processes used in production operations.
    /// </summary>
    /// <remarks>Use this enumeration to distinguish between different machine and process types in FFC
    /// workflows, such as inspection, automated processing, and manual operations. The values can be used for
    /// configuration, logging, or control flow where machine type differentiation is required.</remarks>
    public enum EnumMachineType
    {
        InspectionMachine = 0,
        MachineProcess = 1,
        ManualProcess = 2,
        ManualInspection = 3,
        InscpectionProcess = 4
    }
}
