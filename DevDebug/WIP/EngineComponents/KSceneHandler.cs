using SFML.Graphics;

namespace KheaiGameEngine.EngineComponents
{
    public class KSceneHandler : KEngineComponent
    {
        private List<string> removeObjects = new();
        private List<KGameObject> addObjects = new();
        private Dictionary<string, KGameObject> gameObjects = new();

        public IEnumerable<KGameObject> GameObjects
        {
            get
            {
                return gameObjects.Values;
            }
        }
     
        public override void Init()
        {

        }

        public override void Start()
        {

        }

        public override void End()
        {
            foreach (KGameObject gameObject in gameObjects.Values)
            {
                gameObject.End();
            }
        }

        public override void FixedUpdate()
        {
            foreach (KGameObject obj in gameObjects.Values)
            {
                obj.FixedUpdate();
            }

            if (removeObjects.Count > 0)
            {
                foreach (string name in removeObjects)
                {
                    gameObjects[name].End();
                    gameObjects.Remove(name);
                }
                removeObjects.Clear();
            }

            if (addObjects.Count > 0)
            {
                foreach (KGameObject obj in addObjects)
                {
                    gameObjects.Add(GenerateObjectID(obj), obj);
                    obj.Start();
                }
                addObjects.Clear();
            }
        }

        public override void FrameUpdate(double deltaTIme, RenderTarget target)
        {
            foreach (KGameObject obj in gameObjects.Values)
            {
                obj.FrameUpdate(deltaTIme);
            }
        }

        public void RemoveObject(string id)
        {
            //Locked for insertion
            lock (removeObjects)
            {
                removeObjects.Add(id);
            }
        }

        public void AddObject(KGameObject gameObject)
        {
            gameObject.SceneManager = this;
            gameObject.Init();

            //Locked for insertion
            lock (addObjects)
            {
                addObjects.Add(gameObject);
            }
        }

        public void AddObject(KGameObject gameObject, string name)
        {
            gameObject.Name = name;
            AddObject(gameObject);
        }

        public void AddObjects(KGameObject[] gameObjects)
        {
            foreach (KGameObject gameObject in gameObjects)
            {
                AddObject(gameObject);
            }
        }

        private string GenerateObjectID(KGameObject obj)
        {
            string name = obj.Name;
            for (int i = 0; gameObjects.ContainsKey(name); i++)
            {
                name = obj.Name + i;
            }
            return name;
        }
    }
}
