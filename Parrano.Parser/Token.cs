namespace Parrano.Parser
{
    public class Token
    {
        public Token(string text, long position)
        {
            _text = text;
            _position = position;
        }
        protected string _text;
        protected long _position;

        public virtual string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public virtual long Position
        {
            get { return _position; }
            set { _position = value; }
        }
    }
}