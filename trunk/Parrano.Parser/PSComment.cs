using System.Collections.Generic;
using System.Text;

namespace Parrano.Parser
{
    public class PSComment : PSEntity
    {
        public static StringBuilder sb = new StringBuilder();

        public static PSComment ParseTokens(IEnumerator<Token> tokens)
        {
            long pos = tokens.Current.Position;

            sb.Length = 0;

            if (tokens.MoveNext())
            {
                PSComment comment;

                if (tokens.Current.Text == "%")
                {
                    comment = new PSDscComment();
                }
                else
                {
                    sb.Append(tokens.Current.Text);
                    comment = new PSComment();
                }

                while ((tokens.Current.Text != "\n" && tokens.Current.Text != "\r") && tokens.MoveNext())
                {
                    sb.Append(tokens.Current.Text);
                }
                
                comment.Position = pos;
                comment.Text = sb.ToString();

                return comment;
            }

            return new PSComment();
        }

        public override string UnParse()
        {
            return "%" + base.UnParse();
        }
    }
}