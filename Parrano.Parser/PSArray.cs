using System.Collections.Generic;

namespace Parrano.Parser
{
    public class PSArray : PSEntity
    {
        public PSArray()
        {
            _values = new List<PSEntity>();
        }
        public PSArray(string text, long position)
            : base(text, position)
        {
            _values = new List<PSEntity>();
        }

        public PSArray(Token token)
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

        public static PSArray ParseTokens(IEnumerator<Token> tokens, ParserState state)
        {
            PSArray array = new PSArray(string.Empty, tokens.Current.Position);
            
            foreach (PSEntity entity in Parser.GetEntities(tokens, state))
            {
                array.Values.Add(entity);
            }

            return array;
        }

        public override string UnParse()
        {
            return "[" + string.Join(" ", _values.ConvertAll<string>(Convert).ToArray()) + "]";
        }
    }
}