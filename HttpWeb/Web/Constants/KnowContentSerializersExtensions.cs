namespace Dna
{
    /// <summary>
    /// Extension methods for <see cref="KnowContentSerializersExtensions"/>
    /// </summary>
    public static class KnowContentSerializersExtensions
    {
        /// <summary>
        /// Converts <see cref="KnownContentSerializers"/> to mime string
        /// </summary>
        /// <param name="serializer"><see cref="KnownContentSerializers"/></param>
        /// <returns></returns>
        public static string ToMimeString(this KnownContentSerializers serializer)
        {
            switch (serializer)
            {
                case KnownContentSerializers.Json:
                    return "application/json";
                case KnownContentSerializers.Xml:
                    return "application/xml";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
