namespace Dna
{
    /// <summary>
    /// Known types of content that can be serialized and sent to a receiver
    /// </summary>
    public enum KnownContentSerializers
    {
        /// <summary>
        /// Data should be serialized as JSON
        /// </summary>
        Json = 1,

        /// <summary>
        /// Data should be serialized as XML
        /// </summary>
        Xml = 2
    }
}
