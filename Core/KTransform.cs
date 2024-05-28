using SFML.System;

namespace KheaiGameEngine.Core
{
    //TODO Apply positional math for parent rotation.
    public class KTransform
    {
        public static float GameUnit = 1;

        private Vector2f _truePos = new(0, 0);
        private KTransform _parent;

        public float rotation = 0.0f;
        public Vector2f RelativePos = new(0, 0);
        public Vector2f Dimentions = new(1, 1);

        public float Left => _truePos.X - Width / 2;
        public float Right => _truePos.X + Width / 2;
        public float Top => _truePos.Y - Height / 2;
        public float Bottom => _truePos.Y + Height / 2;

        public float Width
        {
            get => Dimentions.X;
            set => Dimentions.X = value;
        }

        public float Height
        {
            get => Dimentions.Y;
            set => Dimentions.Y = value;
        }

        public float PosX
        {
            get => _truePos.X;
            set
            {
                RelativePos.X = value - parent.PosX;
                _truePos.X = value;
            }
        }

        public float PosY
        {
            get => _truePos.Y;
            set
            {
                RelativePos.Y = value - parent.PosY;
                _truePos.Y = value;
            }
        }

        public Vector2f Position
        {
            get => _truePos;
            set
            {
                PosX = value.X;
                PosY = value.Y;
            }
        }

        public KTransform parent
        {
            get
            {
                if (_parent == null) _parent = this;
                return _parent;
            }
            set => _parent = value;
        }
    }
}