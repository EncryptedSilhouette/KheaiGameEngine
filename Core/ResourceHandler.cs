using KheaiGameEngine.GameObjects;
using KheaiGameEngine.ObjectComponents;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace KheaiGameEngine.Core
{
    public static class ResourceHandler
    {

    }

    public class KPolyTypeResolver : DefaultJsonTypeInfoResolver
    {
        public class TypeResolver<BaseType>
        {
            public delegate JsonTypeInfo ResolveTypeInfo();

            private List<Type> _derivedTypes = new();
            private ResolveTypeInfo _resolution;

            public TypeResolver(ResolveTypeInfo resolve)
            {
                _resolution = resolve;
            }

            public void AddDerivedType<T>() where T : BaseType
            {
                _derivedTypes.Add(typeof(T));
            }
        }

        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            

            Type baseType = typeof(KObjectComponent);

            JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

            if (jsonTypeInfo.Type == baseType)
            {
                jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
                {
                    TypeDiscriminatorPropertyName = "component-type",
                    IgnoreUnrecognizedTypeDiscriminators = true,
                    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                    DerivedTypes =
                {
                    new JsonDerivedType(typeof(KTransform), "test_a"),
                }
                };
            }

            return jsonTypeInfo;
        }

        private static void resolveObjectComponents()
        {

        }
    }
}
