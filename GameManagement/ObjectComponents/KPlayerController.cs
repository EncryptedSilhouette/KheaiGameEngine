using KheaiGameEngine.GameObjects;
using SFML.Window;

namespace KheaiGameEngine.GameManagement.ObjectComponents
{
    public class KPlayerController : KObjectComponent
    {
        private KTransform transform;

        public override void Init()
        {
            
        }

        public override void Start()
        {
            transform = Owner.GetComponent<KTransform>();
        }

        public override void Update(ulong currentTick)
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.W)) transform.PosY -= 1;
            if (Keyboard.IsKeyPressed(Keyboard.Key.S)) transform.PosY += 1;
            if (Keyboard.IsKeyPressed(Keyboard.Key.A)) transform.PosX -= 1;
            if (Keyboard.IsKeyPressed(Keyboard.Key.D)) transform.PosX += 1;
        }

        public override void FrameUpdate(ulong currentFrame)
        {
            throw new NotImplementedException();
        }

        public override void End()
        {
            throw new NotImplementedException();
        }
    }
}
