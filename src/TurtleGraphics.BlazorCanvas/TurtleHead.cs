using System.Drawing;

namespace TurtleGraphics.BlazorCanvas
{
    public record TurtleHead
    {
        public float X { get; init; } = 0;

        public float Y { get; init; } = 0;

        private float _angle;
        public float Angle
        {
            get => _angle;
            init
            {
                _angle = value % 360;
                if (_angle < 0)
                {
                    _angle += 360;
                }
            }
        }

        public bool ShowTurtle { get; init; } = true;

        public bool PenVisible { get; init; } = true;

        public int Delay { get; init; } = 0;

        public static readonly Color DefaultColor = Color.Blue;
        public Color PenColor { get; init; } = DefaultColor;

        public const int DefaultPenSize = 7;
        public float PenSize { get; init; } = DefaultPenSize;

        public int Left { get; init; }
        public int Top { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
    }
}
