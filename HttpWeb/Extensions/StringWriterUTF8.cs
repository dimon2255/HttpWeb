using System.IO;
using System.Text;

namespace Dna
{
    /// <summary>
    /// String Writer as UTF8
    /// </summary>
    public class StringWriterUTF8 : StringWriter
    {
        /// <summary>
        /// Set the Encoding to UTF8
        /// </summary>
        public override Encoding Encoding => Encoding.UTF8;
    }
}
