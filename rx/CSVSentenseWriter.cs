using System.IO;
using System.Linq;

namespace rx
{
    public class CsvSentenseWriter : ISentenceWriter
    {
        private readonly StreamWriter _writer;
        private int _number = 1;

        public CsvSentenseWriter(Stream outStream)
        {
            _writer = new StreamWriter(outStream);
            //write header
            _writer.WriteLine(", Word 1, Word 2, Word 3, Word 4, Word 5, Word 6, Word 7, Word 8");
        }

        public void Dispose()
        {
            _writer.Flush();
            _writer.Dispose();
        }

        public void WriteSentence(SentenseInfo sentense)
        {
            var resLine = Enumerable.Repeat("Sentence " + _number, 1).Concat(sentense.Words);
            _writer.WriteLine(string.Join(", ", resLine));
            _number++;
        }
    }
}