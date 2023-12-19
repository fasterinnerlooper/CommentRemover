using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentRemover
{
    public interface IConsoleWrapper
    {
        int CursorTop { get; }
        int BufferWidth { get; }

        void SetCursorPosition(int left, int top);
        void Write(string text);
        void WriteLine(string text);
        void Write(char[] text);
    }
    public class ConsoleWrapper : IConsoleWrapper
    {
        public void SetCursorPosition(int left, int top)
        {
            if (top <= 0) top = 0;
            Console.SetCursorPosition(left, top);
        }

        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }

        public void Write(char[] text)
        {
            Console.Write(text);
        }

        public void Write(string text)
        {
            Console.Write(text);
        }

        public int CursorTop
        {
            get
            {
                return Console.CursorTop;
            }
        }

        public int BufferWidth
        {
            get
            {
                return Console.BufferWidth;
            }
        }
    }
}
