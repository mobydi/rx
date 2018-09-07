using System;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace rx.test
{
    [TestFixture]
    internal class Tests
    {
        class TestWriter : ISentenceWriter
        {
            private readonly StringBuilder _result = new StringBuilder();

            public void Dispose()
            {
                _result.AppendLine("==DISPOSED==");
            }

            public void WriteSentence(SentenseInfo sentense)
            {
                _result.AppendLine(string.Join(", ", sentense.Words));
            }

            public override string ToString()
            {
                return _result.ToString();
            }
        }

        [Test]
        public void Test_Sample1()
        {
            TestWriter writer;
            using (writer = new TestWriter())
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "rx.test.samples.sample1.txt";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    Processor.Split(stream, writer).Wait();
                }
            }

            Assert.AreEqual("a, had, lamb, little, Mary\r\n" +
                            "Aesop, and, called, came, for, Peter, the, wolf\r\n" +
                            "Cinderella, likes, shoes\r\n" +
                            "==DISPOSED==\r\n", 
                            writer.ToString());
        }

        [Test]
        public void Test_Sample2()
        {
            var writer = new TestWriter();
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "rx.test.samples.sample2.txt";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                Processor.Split(stream, writer).Wait();
            }

            Assert.AreEqual("a, had, lamb, little, Mary\r\n" +
                            "Aesop, and, called, came, for, Peter, the, wolf\r\n" +
                            "Cinderella, likes, shoes\r\n", 
                            writer.ToString());
        }

        [Test]
        public void Test_Sample3()
        {
            var writer = new TestWriter();
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "rx.test.samples.sample3.txt";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                Processor.Split(stream, writer).Wait();
            }

            Assert.AreEqual("a, had, lamb, little, Mary\r\n" +
                            "Aesop, and, called, came, for, Peter, the, wolf\r\n" +
                            "Cinderella, likes, shoes\r\n" +
                            "adipisicing, aliqua, amet, consectetur, do, dolor, dolore, eiusmod, elit, et, incididunt, ipsum, labore, Lorem, magna, sed, sit, tempor, ut\r\n" +
                            "ad, aliquip, commodo, consequat, ea, enim, ex, exercitation, laboris, minim, nisi, nostrud, quis, ullamco, ut, Ut, veniam\r\n" +
                            "aute, cillum, dolor, dolore, Duis, esse, eu, fugiat, in, in, irure, nulla, reprehenderit, velit, voluptate\r\n",
                writer.ToString());

        }

        [Test]
        public void Test_CsvWriter()
        {
            var stream = new MemoryStream();
            var writer = new CsvSentenseWriter(stream);

            writer.WriteSentence(new SentenseInfo(new[] {"word1", "word2", "word3"}));
            writer.WriteSentence(new SentenseInfo(new[] {"word2_1", "word2_3", "word2_2"}));
            writer.Dispose();

            Assert.AreEqual(
                ", Word 1, Word 2, Word 3, Word 4, Word 5, Word 6, Word 7, Word 8\r\n" +
                "Sentence 1, word1, word2, word3\r\n" +
                "Sentence 2, word2_1, word2_3, word2_2\r\n",
                Encoding.UTF8.GetString(stream.ToArray()));
        }

        [Test]
        public void Test_XmlWriter()
        {
            var stream = new MemoryStream();
            var writer = new XmlSentenseWriter(stream, false);

            writer.WriteSentence(new SentenseInfo(new[] { "word1", "word2", "word3" }));
            writer.WriteSentence(new SentenseInfo(new[] { "word2_1", "word2_3", "word2_2" }));
            writer.Dispose();

            Assert.AreEqual(
                "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<text>" +
                "<sentence><word>word1</word><word>word2</word><word>word3</word></sentence>" +
                "<sentence><word>word2_1</word><word>word2_3</word><word>word2_2</word></sentence>" +
                "</text>",
                Encoding.UTF8.GetString(stream.ToArray()));
        }
    }
}
