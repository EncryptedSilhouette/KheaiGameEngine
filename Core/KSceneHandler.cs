using KheaiGameEngine.GameObjects;
using System.Text.Json;

namespace KheaiGameEngine.Core
{
    #region Scene

    public struct KSceneData
    {
        public string ID { get; set; }
        public KGameObject[] GameObjects { get; set; }
    }

    public sealed class KScene
    {
        //Serialization
        public string ID { get; private set; }

        private Queue<string> removeObjects = new();
        private Queue<KGameObject> addObjects = new();
        private Dictionary<string, KGameObject> aliveGameObjects = new(0);
        private Dictionary<string, int> entityCounts = new();

        public KScene(KSceneData sceneData)
        {
            ID = sceneData.ID;
            AddObjects(sceneData.GameObjects);
        }

        public void Update(uint currentTick)
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

        public void FrameUpdate(uint currentFrame)
        {
            foreach (var gameObject in aliveGameObjects.Values)
            {
                gameObject.FrameUpdate(currentFrame);
            }
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

        public static KScene GenerateSceneFromFile(string sceneFilePath)
        {
            KSceneData data = JsonSerializer.Deserialize<KSceneData>(File.Open(sceneFilePath, FileMode.Open));
            return GenerateScene(data);
        }

        public static KScene GenerateScene(string dataString)
        {
            KSceneData data = JsonSerializer.Deserialize<KSceneData>(dataString);
            return GenerateScene(data);
        }

        public static KScene GenerateScene(KSceneData sceneData)
        {
            return new KScene(sceneData);
        }
    }
    #endregion

    #region SceneHander
    public class KSceneHandler : KEngineComponent
    {
#if DEBUG
        protected string defaultScene = "debug";
#else
        protected string defaultScene = "title";
#endif

        public KSceneHandler()
        {
            Order = 1;
        }

        public override void Init()
        {

        }

        public override void Start()
        {

        }

        public override void End()
        {

        }

        public override void Update(uint currentTick)
        {
         
        }

        public override void FrameUpdate(uint currentFrame)
        {
           
        }
    }
    #endregion
}
