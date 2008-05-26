using System.Collections.Generic;

namespace Parrano.Parser
{
    public class PSName : PSEntity
    {
        public PSName(string text, long position)
            : base(text, position)
        {
        }

        public PSName(Token token)
            : this(token.Text, token.Position)
        {
        }

        public static PSName ParseTokens(IEnumerator<Token> tokens)
        {
            long pos = tokens.Current.Position;

            tokens.MoveNext();
            PSEntity entity = tokens.Current as PSEntity;

            if (entity != null)
            {
                return new PSName(entity);
            }

            if (tokens.Current != null)
            {
                pos = tokens.Current.Position;
            }

            throw new PSParserException(string.Format("Syntax error at position {0}.", pos));
        }

        public override string UnParse()
        {
            return "/" + base.UnParse();
        }
    }
}