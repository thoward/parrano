namespace Parrano.Parser
{
    public class PSSymbol : Token
    {
        public PSSymbol(string text, long position)
            : base(text, position)
        {
        }

        public PSSymbol(Token token)
            : this(token.Text, token.Position)
        {
        }

        private static readonly char[] PSSymbols = {'/', '{', '}', '%', '(', ')', '[', ']'};
        
        public static bool IsPSSymbol(char c)
        {
            for (int i = 0; i < PSSymbols.Length; i++)
            {
                if(PSSymbols[i] == c)
                {
                    return true;
                }
            }

            return false;
        }
    }
}