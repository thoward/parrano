namespace Parrano.Api
{
    public class CMYKColor : Color
    {
        public CMYKColor(float cyan, float yellow, float magenta, float black)
        {
            _cyan = cyan;
            _yellow = yellow;
            _magenta = magenta;
            _black = black;
        }

        private float _cyan;
        private float _yellow;
        private float _magenta;
        private float _black;

        public float Cyan
        {
            get { return _cyan;}
            set { _cyan = value;}
        }
        
        public float Yellow
        {
            get { return _yellow;}
            set { _yellow = value;}
        }
        
        public float Magenta
        {
            get { return _magenta;}
            set { _magenta = value;}
        }

        public float Black
        {
            get { return _black;}
            set { _black = value;}
        }

        public override float[]  ToFloatArray()
        {
            return new float[] { _cyan, _yellow, _magenta, _black };
        }
    }
}