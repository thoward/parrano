using Parrano.Api;

namespace Parrano.Api
{
    public class SpotColor
    {
        public SpotColor(string name, ColorSpace colorSpace, Color color)
        {
            _name = name;
            _colorSpace = colorSpace;
            _color = color;
        }

        public SpotColor(string name, Color color)
        {
            _name = name;
            _color = color;
            _colorSpace = (_color is CMYKColor) ? 
                                ColorSpace.cmyk : (_color is RGBColor) ? 
                                        ColorSpace.rgb : (_color is GrayColor) ? 
                                                ColorSpace.gray : default(ColorSpace);
        }

        private string _name;
        private ColorSpace _colorSpace;
        private Color _color;
        private int _resourceID;

        public string Name
        {
            get { return _name;}
            set { _name = value;}
        }

        public ColorSpace ColorSpace
        {
            get { return _colorSpace;}
            set { _colorSpace = value;}
        }

        public Color Color
        {
            get { return _color;}
            set { _color = value;}
        }

        public int ResourceID
        {
            get { return _resourceID;}
            set { _resourceID = value;}
        }
    }
}