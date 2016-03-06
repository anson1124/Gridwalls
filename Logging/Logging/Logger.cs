using System;

namespace Logging
{
    public interface Logger : IDisposable
    {
        InfoLevel MinimumInfoLevelBeforeWrite { set; get; }

        void Write<T>(String text);

        void Write<T>(InfoLevel infolevel, String text);
    }
}