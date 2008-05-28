namespace Parrano.Api
{
    public class RGBColor : Color
    {
        public RGBColor(float red, float green, float blue)
        {
            _red = red;
            _green = green;
            _blue = blue;
        }

        private float _red;
        private float _green;
        private float _blue;

        public float Red
        {
            get { return _red;}
            set { _red = value;}
        }
        
        public float Green
        {
            get { return _green;}
            set { _green = value;}
        }
        
        public float Blue
        {
            get { return _blue;}
            set { _blue = value;}
        }

        public override float[]  ToFloatArray()
        {
            return new float[] { _red, _green, _blue };
        }
    }
}