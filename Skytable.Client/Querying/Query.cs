using System.Collections.Generic;
using System.IO;

namespace Skytable.Client.Querying
{
    public class Query
    {
        private ushort _sizeCount;
        private List<byte> _holdingBuffer;

        public Query()
        {
            _sizeCount = 0;
            _holdingBuffer = new List<byte>();
        }

        public void Push(string argument)
        {
            var unicode_argument = System.Text.Encoding.UTF8.GetBytes(argument);
            var header = $"+{unicode_argument.Length}\n";
            var unicode_header = System.Text.Encoding.UTF8.GetBytes(header);
            _holdingBuffer.AddRange(unicode_header);
            _holdingBuffer.AddRange(unicode_argument);
            _holdingBuffer.Add(10);
            _sizeCount++;
        }

        public void WriteTo(Stream stream)
        {
            var header = System.Text.Encoding.UTF8.GetBytes("*1\n");
            stream.Write(header, 0, header.Length);
            var numberOfItemsInDatagroup = System.Text.Encoding.UTF8.GetBytes($"_{_sizeCount}\n");
            stream.Write(numberOfItemsInDatagroup, 0, numberOfItemsInDatagroup.Length);
            stream.Write(_holdingBuffer.ToArray(), 0, _holdingBuffer.Count);
        }
    }
}