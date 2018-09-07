using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace rx
{
    public class XmlSentenseWriter : ISentenceWriter
    {
        private readonly XmlTextWriter _writer;

        public XmlSentenseWriter(Stream outStream, bool pretty)
        {
            _writer = new XmlTextWriter(outStream, Encoding.UTF8);
            _writer.Formatting = pretty ? Formatting.Indented : Formatting.None;
            //write header
            _writer.WriteStartDocument();
            _writer.WriteStartElement("text");
        }

        public void Dispose()
        {
            _writer.WriteEndElement();
            _writer.WriteEndDocument();
            _writer.Flush();
            _writer.Dispose();
        }

        public void WriteSentence(SentenseInfo sentense)
        {
            _writer.WriteStartElement("sentence");
            foreach (var word in sentense.Words)
            {
                _writer.WriteElementString("word", word);
            }
            _writer.WriteEndElement();
        }
    }
}