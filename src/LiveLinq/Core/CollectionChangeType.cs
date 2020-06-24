using System.Runtime.Serialization;

namespace LiveLinq.Core
{
    /// <summary>
    /// Types of changes that can occur in a collection.
    /// </summary>
    [DataContract]
    public enum CollectionChangeType
    {
        Add,
        Remove
    }
}
