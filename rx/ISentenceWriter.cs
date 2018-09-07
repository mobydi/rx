using System;

namespace rx
{
    public interface ISentenceWriter : IDisposable
    {
        void WriteSentence(SentenseInfo sentense);
    }
}