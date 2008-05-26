using System.Collections.Generic;

namespace Parrano.Parser
{
    public class PSScopeBlock : PSEntity
    {
        public PSScopeBlock()
        {
            _values = new List<PSEntity>();
        }
        public PSScopeBlock(string text, long position)
            : base(text, position)
        {
            _values = new List<PSEntity>();
        }

        public PSScopeBlock(Token token)
            : this(token.Text, token.Position)
        {
        }

        private List<PSEntity> _values;
        public List<PSEntity> Values
        {
            get { return _values; }
            set { _values = value; }
        }

        public string Convert(PSEntity entity)
        {
            return entity.Text;
        }

        public static PSScopeBlock ParseTokens(IEnumerator<Token> tokens, ParserState state)
        {
            PSScopeBlock scopeBlock = new PSScopeBlock(string.Empty, tokens.Current.Position);

            foreach (PSEntity entity in Parser.GetEntities(tokens, state))
            {
                scopeBlock.Values.Add(entity);
            }

            return scopeBlock;
        }

        public override string UnParse()
        {
            return "{" + string.Join(" ", _values.ConvertAll<string>(Convert).ToArray()) + "}";
        }

    }
}