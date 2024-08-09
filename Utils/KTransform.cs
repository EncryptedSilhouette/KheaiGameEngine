#if DEBUG
using SFML.System;

namespace KheaiUtils
{
    //TODO Apply positional math for parent rotation.
    public class KTransform
    {   
        //The true coordiantes
        private Vector2f _truePos = new(0, 0);

        //Refrence to parent transform
        private KTransform _parent;

        ///<summary>The transform's rotation in degrees.</summary>
        public float Rotation = 0.0f;
        ///<summary>position relative to parent.</summary>
        public Vector2f RelativePos = new(0, 0);
        ///<summary>The dimentions for the transform.</summary>
        public Vector2f Dimentions = new(1, 1);

        ///<summary>The left coordiates for the transform.</summary>
        public float Left => _truePos.X - Width / 2;
        ///<summary>The right coordiates for the transform.</summary>
        public float Right => _truePos.X + Width / 2;
        ///<summary>The top coordiates for the transform.</summary>
        public float Top => _truePos.Y - Height / 2;
        ///<summary>The bottom coordiates for the transform.</summary>
        public float Bottom => _truePos.Y + Height / 2;

        ///<summary>.</summary>
        public Vector2f TopLeft => new(PosX, PosY);

        ///<summary>.</summary>
        public Vector2f TopRight => new(PosX + Width, PosY);

        ///<summary>.</summary>
        public Vector2f BottomLeft => new(PosX, PosY + Height);

        ///<summary>.</summary>
        public Vector2f BottomRight => new(PosX + Width, PosY + Height);


        ///<summary>The width of this transforms.</summary>
        public float Width
        {
            get => Dimentions.X;
            set => Dimentions.X = value;
        }

        ///<summary>The height of this transforms.</summary>
        public float Height
        {
            get => Dimentions.Y;
            set => Dimentions.Y = value;
        }

        ///<summary>The transform's X position.</summary>
        public float PosX
        {
            get => _truePos.X;
            set
            {
                RelativePos.X = value - parent.PosX;
                _truePos.X = value;
            }
        }

        ///<summary>The transform's Y position.</summary>
        public float PosY
        {
            get => _truePos.Y;
            set
            {
                RelativePos.Y = value - parent.PosY;
                _truePos.Y = value;
            }
        }

        ///<summary>The transform's position.</summary>
        public Vector2f Position
        {
            get => _truePos;
            set
            {
                PosX = value.X;
                PosY = value.Y;
            }
        }

        ///<summary>The transform's parent transform.</summary>
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
#endif