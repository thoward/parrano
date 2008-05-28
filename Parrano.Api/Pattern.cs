namespace Parrano.Api
{
    public class Pattern
    {
        public Pattern(int resourceId) 
        {
            _resourceID = resourceId;
        }

        public Pattern(int width, int height, int xStep, int yStep, PostscriptPatternPaintType paintType)
        {
            _width = width;
            _height = height;
            _xStep = xStep;
            _yStep = yStep;
            _paintType = paintType;
        }

        private int _resourceID;
        private int _width;
        private int _height;
        private int _xStep;
        private int _yStep;
        private PostscriptPatternPaintType _paintType;

        public PostscriptPatternPaintType PaintType
        {
            get { return _paintType; }
            set { _paintType = value; }
        }

        public int YStep
        {
            get { return _yStep; }
            set { _yStep = value; }
        }

        public int XStep
        {
            get { return _xStep; }
            set { _xStep = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int ResourceID
        {
            get { return _resourceID; }
            set { _resourceID = value; }
        }
    }
}