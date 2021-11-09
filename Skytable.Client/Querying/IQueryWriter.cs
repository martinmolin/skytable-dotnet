using System.IO;
using System.Threading.Tasks;

namespace Skytable.Client.Querying
{
    /// <summary>Interface for an object that can be written to a stream.</summary>
    public interface IQueryWriter
    {
        /// <summary>Writes the query to the specified stream.</summary>
        void WriteTo(Stream stream);

        /// <summary>Writes the query to the specified stream asynchronously.</summary>
        Task WriteToAsync(Stream stream);
    }
}
