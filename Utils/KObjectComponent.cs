namespace KheaiGameEngine.GameManagement
{
    #region ObjectComponent
    public abstract class KObjectComponent : IKComponent
    {
        public bool Enabled { get; set; }
        public ushort Order { get; set; }
        public string ID { get; set; }
        public KGameObject Owner { get; set; }

        public KObjectComponent()
        {
            ID = GetType().Name;
            Enabled = true;
        }

        public abstract void Init();
        public abstract void Start();
        public abstract void End();
        public abstract void Update(uint currentTick);
        public abstract void FrameUpdate(uint currentFrame);
    }
    #endregion

    #region ObjectData
    public class KObjectData
    {
        public string ID { get; set; }
        public List<KObjectComponent> Components { get; set; }

        public KGameObject CreateObject()
        {
            return CreateObject(ID);
        }

        public KGameObject CreateObject(string name)
        {
            KGameObject gameObject = new(ID, name);

            foreach (var component in Components)
            {

            }

            return gameObject;
        }
    }
    #endregion
}