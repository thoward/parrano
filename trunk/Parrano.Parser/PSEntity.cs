using System.IO;
using System.Text;

namespace Parrano.Parser
{
    public class PSEntity : Token
    {
        public PSEntity(string text, long position)
            : base(text, position)
        {
        }

        public PSEntity(Token token)
            : this(token.Text, token.Position)
        {
        }

        private static readonly StringBuilder sb = new StringBuilder();

        protected PSEntity() : base(string.Empty, -1)
        {
        }

        public static int Parse(Stream stream, out PSEntity entity)
        {
            entity = null;
            sb.Length = 0;

            long pos = stream.Position;

            while (true)
            {
                int i = stream.ReadByte();

                if (i == -1 || PSSymbol.IsPSSymbol((char)i) || char.IsWhiteSpace((char)i))
                {
                    string tokenText = sb.ToString();

                    if (!string.IsNullOrEmpty(tokenText))
                    {
                        entity = new PSEntity(tokenText, pos);
                    }

                    return i;
                }
                sb.Append((char) i);
            }
        }

        public virtual string UnParse()
        {
            return _text;
        }
    }
}