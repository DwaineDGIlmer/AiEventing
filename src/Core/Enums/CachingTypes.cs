namespace Core.Enums
{
    /// <summary>
    /// Specifies the types of caching mechanisms available for use.
    /// </summary>
    /// <remarks>This enumeration provides options for different caching strategies, such as in-memory or file
    /// system storage. It can be used to configure caching behavior in applications.</remarks>
    public enum CachingTypes
    {
        /// <summary>
        /// Represents a state where no specific option or flag is set.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents an in-memory storage option.
        /// </summary>
        InMemory = 1,

        /// <summary>
        /// Represents a file system storage type.
        /// </summary>
        /// <remarks>This enumeration value is used to specify that the storage type is a file
        /// system.</remarks>
        FileSystem = 2,
    }
}
