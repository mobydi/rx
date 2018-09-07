using System;


namespace rx
{
    static class Program
    {
        static void Main(string[] args)
        {
            ISentenceWriter writer;
            if (args.Length >= 1 && string.Compare(args[0], "-xml", StringComparison.InvariantCultureIgnoreCase) == 0)
                writer = new XmlSentenseWriter(Console.OpenStandardOutput(), true);
            else
                writer = new CsvSentenseWriter(Console.OpenStandardOutput());

            using (writer)
            {
                Processor.Split(Console.OpenStandardInput(), writer).Wait();
            }
        }
    }
}
