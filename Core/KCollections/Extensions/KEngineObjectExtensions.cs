namespace KheaiGameEngine.Core.KCollections.Extensions
{
    public static class KEngineObjectExtensions
    {
        public static T? Find<T>(this IEnumerable<IKEngineObject> collection) where T : IKEngineObject
        {
            foreach (var item in collection)
            {
                if (item is T i) return i;
            }
            return default;
        }

        public static IEnumerable<T> FindAll<T>(this IEnumerable<IKEngineObject> collection) where T : IKEngineObject
        {
            foreach (var item in collection)
            {
                if (item is T i) yield return i;
            }
        }
    }
}
