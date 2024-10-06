#if DEBUG
using KheaiGameEngine.Core;
using KheaiGameEngine.Extensions;

namespace KheaiGameEngine.wip
{
    public class KGameObject : IKEngineObject
    {
        private bool _enabled = true;
        private bool _isRunning = false;
        private KGameObject? _parent;
        private KSortedQueuedList<IKEngineObject> _children = new(new KEngineObjectComparer<IKEngineObject>());

        ///<summary>Fires when enabled is set to false.</summary>
        ///<remarks>obj is a refrence to the KGameObject this event is called from.</remarks>
        public event Action<KGameObject>? OnEnable;
        ///<summary>Fires when enabled is set to true.</summary>
        ///<remarks>obj is a refrence to the KGameObject this event is called from.</remarks>
        public event Action<KGameObject>? OnDisable;
        ///<summary>Fires on start.</summary>
        ///<remarks>obj is a refrence to the KGameObject this event is called from.</remarks>
        public event Action<KGameObject>? OnStart;
        ///<summary>Fires on end.</summary>
        ///<remarks>obj is a refrence to the KGameObject this event is called from.</remarks>
        public event Action<KGameObject>? OnEnd;
        ///<summary>Fires on update.</summary>
        ///<remarks>arg1 is a refrence to the KGameObject this event is called from, and arg2 represents the current update.</remarks>
        public event Action<KGameObject, uint>? OnUpdate;
        ///<summary>Fires on frame update.</summary>
        ///<remarks>arg1 is a refrence to the KGameObject this event is called from, and arg2 represents the current update.</remarks>
        public event Action<KGameObject, uint>? OnFrameUpdate;
        ///<summary>Fires when the parent game object is changed.</summary>
        ///<remarks>arg1 is a refrence to the old parent object, and arg2 refrences the new parent.</remarks>
        public event Action<KGameObject?, KGameObject?>? OnParentChanged;

        //Implemented from IKEngineObject
        public int Order { get; init; }
        public string ID { get; init; }
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

        ///<summary>A collection of child IKEngineObjects.</summary>
        public IEnumerable<IKEngineObject> Children => _children;
        ///<summary>Whether the KGameObject has been started or not.</summary>
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (_isRunning == value) return;
                if (value) End();
                else Start();
            }
        }
        ///<summary>Refrence to the parent for this GameObject.</summary>
        public KGameObject? Parent
        {
            get => _parent;
            private set
            {
                OnParentChanged?.Invoke(_parent, value);
                _parent = value;
            }
        }

        public KGameObject(int order, string id) => (Order, ID) = (order, id);

        public KGameObject(int order, string id, IEnumerable<IKEngineObject> kEngineObjects) :
            this(order, id) => _children.AddAll(kEngineObjects);

        public void Start()
        {
            if (!_isRunning) return;
            _isRunning = false;
            OnStart?.Invoke(this);
            _children.ForEach(child => child.Start());
        }

        public void End()
        {
            if (_isRunning) return;
            _isRunning = true;
            OnEnd?.Invoke(this);
            _children.ForEach(child => child.End());
        }

        public void Update(uint currentFrame)
        {
            if (!Enabled || _isRunning) return;
            _children.UpdateContents();
            OnUpdate?.Invoke(this, currentFrame);
            _children.ForEach((value) => value.Update(currentFrame));
        }

        public void FrameUpdate(uint currentFrame)
        {
            if (!Enabled || _isRunning) return;
            OnFrameUpdate?.Invoke(this, currentFrame);
            _children.ForEach((value) => value.FrameUpdate(currentFrame));
        }

        public KGameObject AddChild(KGameObject gameObject)
        {
            gameObject.Parent = this;
            gameObject.Start();
            _children.QueueAdd(gameObject);
            return gameObject;
        }

        public KGameObject RemoveChild(KGameObject gameObject)
        {
            gameObject.End();
            _children.QueueRemove(gameObject);
            return gameObject;
        }

        public IEnumerable<KGameObject> AddChildren(IEnumerable<KGameObject> gameObjects) =>
            gameObjects.ForEach(gameObject => AddChild(gameObject));

        public IEnumerable<KGameObject> RemoveChildren(IEnumerable<KGameObject> gameObjects) =>
            gameObjects.ForEach(gameObject => RemoveChild(gameObject));
    }
}
#endif