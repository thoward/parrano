using System.Collections.Generic;
using System.IO;

namespace Parrano.Parser
{
    public class Parser
    {
        public static IEnumerable<Token> GetTokens(Stream stream)
        {
            int i = -1;
            
            PSEntity entity = default(PSEntity);

            while (true)
            {
                i = PSEntity.Parse(stream, out entity);
                
                if(entity != default(PSEntity))
                {
                    yield return entity;
                }

                char c = (char)i;

                if(PSSymbol.IsPSSymbol(c))
                {
                    yield return new PSSymbol(c.ToString(), stream.Position);
                }
                else if (char.IsWhiteSpace(c))
                {
                    yield return new Token(c.ToString(), stream.Position);
                }
                if(i == -1)
                {
                    break;
                }
            }
        }

        public static IEnumerable<PSEntity> GetEntities(Stream stream)
        {
            IEnumerable<Token> tokens= GetTokens(stream);
            IEnumerator<Token> tokenEnumerator = tokens.GetEnumerator();
            return GetEntities(tokenEnumerator, ParserState.None);
        }

        internal static IEnumerable<PSEntity> GetEntities(IEnumerator<Token> tokenEnumerator, ParserState state)
        {
            bool endStream = false;
            while (!endStream && tokenEnumerator.MoveNext()) 
            {
                Token token = tokenEnumerator.Current;
                if (token is PSSymbol)
                {
                    switch (token.Text[0])
                    {
                        case '%':
                            yield return PSComment.ParseTokens(tokenEnumerator);
                            break;
                        case '[':
                            state = state | ParserState.Array;
                            yield return PSArray.ParseTokens(tokenEnumerator, state);
                            break;
                        case ']':
                            if(ContainsFlag(state, ParserState.Array))
                            {
                                state &= ~ParserState.Array;
                                endStream = true;
                            }
                            break;
                        case '{':
                            state = state | ParserState.Braces;
                            yield return PSScopeBlock.ParseTokens(tokenEnumerator, state);
                            break;
                        case '}':
                            if (ContainsFlag(state, ParserState.Braces))
                            {
                                state &= ~ParserState.Braces;
                                endStream = true;
                            }
                            break;
                        case '/':
                            yield return PSName.ParseTokens(tokenEnumerator);
                            break;
                        default:
                            break;
                    }
                }
                else if (token is PSEntity)
                {
                    yield return (PSEntity)token;
                }
            } 
        }

        public static bool ContainsFlag(ParserState value, ParserState flag) 
        {   
            return (value | flag) != 0;
        }
    }
}
