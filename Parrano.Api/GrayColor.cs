namespace Parrano.Api
{
    public class GrayColor : Color
    {
        public GrayColor(float intensity)
        {
            _intensity = intensity;
        }

        private float _intensity;

        public float Intensity
        {
            get { return _intensity; }
            set { _intensity = value; }
        }

        public override float[] ToFloatArray()
        {
            return new float[] { _intensity };
        }
    }
}