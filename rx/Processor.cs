using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace rx
{
    public static class Processor
    {
        private static readonly int _bufferSize = 1024;
        private static readonly char[] WordSeparators = {' ', ',', ':', ';', '\t', '"'};
        private static readonly char[] SentenceSeparators = {'.', '!', '?'};

        public static async Task Split(Stream inputStream, ISentenceWriter sentenceWriter)
        {
            var buffer = new BufferBlock<string>(new DataflowBlockOptions {BoundedCapacity = _bufferSize});

            var sentenceSplitter = new TransformBlock<string, string[]>(line => SplitToSentences(line),
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount,
                    BoundedCapacity = _bufferSize
                });

            var wordSorter =
                new TransformBlock<string[], SentenseInfo[]>(sentences => sentences.Select(SplitWordsAndSort).ToArray(),
                    new ExecutionDataflowBlockOptions
                    {
                        MaxDegreeOfParallelism = Environment.ProcessorCount,
                        BoundedCapacity = _bufferSize
                    });

            var writer = new ActionBlock<SentenseInfo[]>(sentenceInfos =>
            {
                foreach (var info in sentenceInfos)
                {
                    sentenceWriter.WriteSentence(info);
                }
            }, new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = _bufferSize,
                SingleProducerConstrained = true
            });

#pragma warning disable 4014
            buffer.LinkTo(sentenceSplitter);
            buffer.Completion.ContinueWith(task => sentenceSplitter.Complete());

            sentenceSplitter.LinkTo(wordSorter);
            sentenceSplitter.Completion.ContinueWith(task => wordSorter.Complete());

            wordSorter.LinkTo(writer);
            wordSorter.Completion.ContinueWith(task => writer.Complete());
#pragma warning restore 4014

            using (var reader = new StreamReader(inputStream))
                while (true)
                {
                    var line = await reader.ReadLineAsync();
                    if (line == null)
                        break;

                    //block until buffer has free slot
                    while (!buffer.Post(line))
                    {
                    }
                }

            buffer.Complete();
            await writer.Completion;
        }

        private static SentenseInfo SplitWordsAndSort(string sentence)
        {
            //we use RemoveEmptyEntries to get rid of double spaces & separators
            var words = sentence.Split(WordSeparators, StringSplitOptions.RemoveEmptyEntries);
            Array.Sort(words);
            return new SentenseInfo(words);
        }

        private static string[] SplitToSentences(string line)
        {
            //we use RemoveEmptyEntries to get rid of "Some sentence.. Second Sentence." cases
            return line.Split(SentenceSeparators, StringSplitOptions.RemoveEmptyEntries)
                .Where(sentence => !string.IsNullOrWhiteSpace(sentence)).ToArray();
        }
    }
}