#if DEBUG

using KheaiGameEngine.Data;
using KheaiGameEngine.GameObjects;

namespace KheaiGameEngine.DevDebug
{
    public class DevTest
    {
        #region scene generation
        public static void GenerateScene()
        {
            KSceneData sceneData = new KSceneData()
            {
                ID = "dev_scene",
                GameObjects = new()
                {
                    new KObjectData()
                    {
                        ID = "debug_obj",
                        Components = new()
                        {
                            new()
                            {
                                { "id", "KTransform" },
                                { "width", 10 },
                                { "height", 10 },
                                { "pos_x", 50 },
                                { "pos_y", 50 }
                            },

                            new()
                            {
                                { "id", "KSpriteRenderer" },
                                { "dep", "KTransform" },
                                { "sprite_path", "debug.png" }
                            }
                        }
                    }
                }
            };
        }
        #endregion

        //load res
            //load game obj reg

        //load scene
            //scene has gameobject
                //create obj from reg

        void CreateObject()
        {
            //create from gameobject data
                //components
                    //get comp id
        }
    }
}
#endif