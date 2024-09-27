#if DEBUG

using SFML.System;

namespace KheaiGameEngine.wip
{
    //TODO Apply positional math for parent rotation.
    public class KTransform
    {
        private float _scaleX, _scaleY;

        private float _rotation;

        private Vector2f _relativePosition;

        private KTransform _parent;

        public event Action<KTransform> OnScaleChanged;
        public event Action<KTransform> OnRotationChanged;
        public event Action<KTransform> OnPositionChanged;
        public event Action<KTransform> OnParentChanged;

        public KTransform(KTransform parent = null, Vector2f postion = default, float rotation = 0.0f)
        {
            _parent = parent;
            _relativePosition = postion;
            _rotation = rotation;
        }

        public Vector2f Position
        {
            get => _parent is null ? _relativePosition : _parent.Position + _relativePosition;
            set => _relativePosition = _parent is null ? value : value - _parent.Position;
        }
    }

    public static class KMathExtensions
    {

    }
}

#endif