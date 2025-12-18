namespace ProdInfoSys.Enums
{
    /// <summary>
    /// Specifies the types of machines or processes used in FFC (Flexible Flat Cable) operations.
    /// </summary>
    /// <remarks>Use this enumeration to distinguish between different machine and process types in FFC
    /// workflows, such as inspection, automated processing, and manual operations. The values can be used for
    /// configuration, logging, or control flow where machine type differentiation is required.</remarks>
    public enum EnumMachineType
    {
        FFCInspectionMachine = 0,
        FFCMachineProcess = 1,
        FFCManualProcess = 2,
        FFCManualInspection = 3,
        FFCInscpectionProcess = 4
    }
}
