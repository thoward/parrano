using System;

namespace Parrano.Parser
{
    [Flags]
    public enum ParserState
    {
        None = 0,
        Array = 1 << 0,
        Braces = 1 << 1,
        Parens = 1 << 2,
        Comment = 1 << 3,
        All = int.MaxValue
    }
}