namespace BIS.Core.Config
{
    public enum ValueType : byte
    {
        Generic, // generic = string
        Float,
        Int,
        Array, //not used?
        Expression,
        NSpecValueType,
        Int64
    }
}