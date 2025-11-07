namespace NHQueryBuilder.Enums
{
    /// <summary>
    /// Represents the various query condition types supported by NHQueryBuilder.
    /// This enum is marked with the [Flags] attribute to allow bitwise combinations.
    /// </summary>
    [Flags]
    public enum ConditionType : long
    {
        None = 0x00000000,

        // Basic comparison operations
        IsNull = 0x00000001,
        IsNotNull = 0x00000002,
        Equal = 0x00000004,
        NotEqual = 0x00000008,
        In = 0x00000010,
        NotIn = 0x00000020,
        Like = 0x00000040,
        NotLike = 0x00000080,
        Between = 0x00000100,
        GreaterThan = 0x00000200,
        GreaterThanOrEqual = 0x00000400,
        LessThan = 0x00000800,
        LessThanOrEqual = 0x00001000,

        // Query structure and sorting
        Fetch = 0x00002000,
        OrderByAsc = 0x00004000,
        OrderByDesc = 0x00008000,
        Select = 0x00010000,
        Take = 0x00020000,
        Skip = 0x00040000
    }
}
