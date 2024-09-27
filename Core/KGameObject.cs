using KheaiGameEngine.Extensions;

namespace KheaiGameEngine.Core
{
    public class KGameObject : IKObject
    {
        private bool _enabled = true;
        private KSortedQueuedList<IKObject> _children = new();

        ///<summary>Fires when the IKObject is enabled.</summary>
        public event Action<IKObject> OnEnable;
        ///<summary>Fires when the IKObject is disabled.</summary>
        public event Action<IKObject> OnDisable;

        ///<summary>A collection of child KObjects.</summary>
        public KSortedQueuedList<IKObject> Children => _children;

        //Implemented from IKObject
        public bool IsUnique { get; init; } = false;
        public int Order { get; init; }
        public string ID { get; init; }
        public IKObject Parent { get; set; }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                //prevents code from being fired if it is being set to the same state
                if (_enabled == value) return;
                if (_enabled = value) OnEnable?.Invoke(this);
                else OnDisable?.Invoke(this);
            }
        }

        public KGameObject() => ID = string.Empty;

        public virtual void Start() => _children.ForEach(child => child.Start());

        public virtual void End() => _children.ForEach(child => child.End());

        public virtual void Update(uint currentFrame)
        {
            if (!Enabled) _children.ForEach((value) => value.Update(currentFrame));
        }

        public virtual void FrameUpdate(uint currentFrame)
        {
            if (!Enabled) _children.ForEach((value) => value.FrameUpdate(currentFrame));
        }
    }
}