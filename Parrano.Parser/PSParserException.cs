using System;

namespace Parrano.Parser
{
    [global::System.Serializable]
    public class PSParserException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public PSParserException() { }
        public PSParserException(string message) : base(message) { }
        public PSParserException(string message, Exception inner) : base(message, inner) { }
        protected PSParserException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}