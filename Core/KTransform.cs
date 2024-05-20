using SFML.System;
using System.Text.Json.Serialization;

namespace KheaiGameEngine.Core
{
    //TODO Apply positional math for parent rotation.
    public class KTransform
    {
        public static float GameUnit = 1;

        private Vector2f _truePos = new(0, 0);
        private KTransform _parent;

        [JsonInclude]
        public float rotation = 0.0f;
        [JsonInclude]
        public Vector2f RelativePos = new(0, 0);
        [JsonInclude]
        public Vector2f Dimentions = new(1, 1);

        [JsonIgnore]
        public float Width
        {
            get => Dimentions.X;
            set => Dimentions.X = value;
        }

        [JsonIgnore]
        public float Height
        {
            get => Dimentions.Y;
            set => Dimentions.Y = value;
        }

        [JsonIgnore]
        public float PosX
        {
            get => _truePos.X;
            set
            {
                RelativePos.X = value - parent.PosX;
                _truePos.X = value;
            }
        }

        [JsonIgnore]
        public float PosY
        {
            get => _truePos.Y;
            set
            {
                RelativePos.Y = value - parent.PosY;
                _truePos.Y = value;
            }
        }

        [JsonIgnore]
        public Vector2f Position
        {
            get => _truePos;
            set
            {
                PosX = value.X;
                PosY = value.Y;
            }
        }

        [JsonIgnore]
        public KTransform parent
        {
            get
            {
                if (_parent == null)
                {
                    _parent = this;
                }
                return _parent;
            }
            set => _parent = value;
        }

        [JsonIgnore]
        public float Left => _truePos.X - Width / 2;

        [JsonIgnore]
        public float Right => _truePos.X + Width / 2;

        [JsonIgnore]
        public float Top => _truePos.Y - Height / 2;

        [JsonIgnore]
        public float Bottom => _truePos.Y + Height / 2;
    }
}