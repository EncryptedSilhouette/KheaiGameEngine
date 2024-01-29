using KheaiGameEngine.Core;
using KheaiGameEngine.DevDebug;
using KheaiGameEngine.GameObjects;

namespace KheaiGameEngine.Data
{
    #region SceneData
    public class KSceneData
    {
        public string ID { get; set; }
        public List<KObjectData> GameObjects { get; set; }
    }
    #endregion

    public class KSceneHandler : KEngineComponent
    {
#if DEBUG
        protected string defaultScene = "debug";
#else
        protected string defaultScene = "title";
#endif
        protected Queue<string> removeObjects = new();
        protected Queue<KGameObject> addObjects = new();
        protected Dictionary<string, KGameObject> aliveGameObjects = new(0);
        protected Dictionary<string, int> entityCounts = new();

        public KRenderManager renderManager { get; protected set; }

        public KSceneHandler() 
        {
            Order = 1;
        }

        public override void Init()
        {
            defaultScene = (string) Engine.Application.appConfig["default_scene"];
        }

        public override void Start()
        {
            renderManager = Engine.GetComponent<KRenderManager>();
        }

        public override void End()
        {
            
        }

        public override void Update(ulong currentTick)
        {
            while (removeObjects.Count > 0)
            {
                aliveGameObjects.Remove(removeObjects.Dequeue());
            }
            while (addObjects.Count > 0)
            {
                KGameObject gameObject = addObjects.Dequeue();
                aliveGameObjects.Add(gameObject.ID, gameObject);
            }
            foreach (var gameObject in aliveGameObjects.Values)
            {
                gameObject.Update(currentTick);
            }
        }

        public override void FrameUpdate(ulong currentFrame)
        {
            foreach (var gameObject in aliveGameObjects.Values)
            {
                gameObject.FrameUpdate(currentFrame);
            }
        }

        public void LoadScene(string sceneID)
        {

        }

        public void SaveScene()
        {

        }

        public void RefreshHandler()
        {
            removeObjects.Clear();
            addObjects.Clear();
            aliveGameObjects.Clear();
            entityCounts.Clear();
        }

        public void Addobject(KGameObject gameObject)
        {
            if (entityCounts.ContainsKey(gameObject.ID))
            {
                entityCounts[gameObject.ID]++;
            }
            else
            {
                entityCounts.Add(gameObject.ID, 0);
            }
            gameObject.Name += entityCounts[gameObject.ID];
            addObjects.Enqueue(gameObject);
        }

        public void AddObjects(KGameObject[] gameObjects)
        {
            foreach (KGameObject gameObject in gameObjects)
            {
                Addobject(gameObject);
            }
        }

        public void RemoveObject(string id) 
        {
            removeObjects.Enqueue(id);
        }

        public void RemoveObjects(string[] ids)
        {
            foreach (var id in ids)
            {
                RemoveObject(id);
            }
        }
    }
}
