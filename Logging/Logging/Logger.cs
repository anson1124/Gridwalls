using System;

namespace Logging
{
    public interface Logger : IDisposable
    {
        void Write(String text);
        void Write<T>(String text);
        void Write<T>(InfoLevel infolevel, String text);
    }
}