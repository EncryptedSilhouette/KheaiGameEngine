using KheaiGameEngine.GameObjects;
using SFML.System;
using System.Text.Json.Serialization;

namespace KheaiGameEngine.ObjectComponents
{
    //TODO Apply positional math for parent rotation.
    public class KTransform : KObjectComponent 
    {
        private Vector2f _truePos = new(0, 0);
        private KTransform _parent;

        [JsonInclude]
        public int GameUnit = 1;
        [JsonInclude]
        public float rotation = 0.0f;
        [JsonInclude]
        public Vector2f Dimentions = new(1, 1);
        [JsonInclude]
        public Vector2f RelativePosition = new(0, 0);

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
                RelativePosition.X = value - parent.PosX;
                _truePos.X = value;
            }
        }

        public float PosY
        {
            get => _truePos.Y;
            set
            {
                RelativePosition.Y = value - parent.PosY;
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

        public float Left => _truePos.X - Width / 2;
        public float Right => _truePos.X + Width / 2;
        public float Top => _truePos.Y - Height / 2;
        public float Bottom => _truePos.Y + Height / 2;

        public override void End()
        {
            throw new NotImplementedException();
        }

        public override void FrameUpdate(uint currentFrame)
        {
            throw new NotImplementedException();
        }

        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override void Update(uint currentTick)
        {
            throw new NotImplementedException();
        }
    }
}