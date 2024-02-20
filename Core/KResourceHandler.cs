using KheaiGameEngine.GameObjects;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Reflection;
using KheaiGameEngine.ObjectComponents;
using System.Security.AccessControl;

namespace KheaiGameEngine.Core
{
    #region KPolyTypeResolver
    public class KPolyTypeResolver : DefaultJsonTypeInfoResolver
    {
        ///<summary>
        ///Dictionary for identifying what types derive form a base type. 
        ///Uses the base type as the key, and a list of derived types as the value.
        ///</summary>
        private Dictionary<Type, List<Type>> _derivedTypes = new();

        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

            if (_derivedTypes.ContainsKey(type))
            {
                IEnumerable<Type> derivedTypes = _derivedTypes[type];

                jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
                {
                    TypeDiscriminatorPropertyName = $"${type.Name}_id",
                    IgnoreUnrecognizedTypeDiscriminators = true,
                    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                };

                foreach (var derivedType in derivedTypes)
                {
                    jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new(derivedType, derivedType.Name));
                }
            }

            return jsonTypeInfo;
        }


        public void AddDerivedType<Base, Derived>() where Derived : Base
        {
            AddDerivedType(typeof(Base), typeof(Derived));
        }

        public void AddDerivedType(Type baseType, Type type)
        {
            if (!_derivedTypes.ContainsKey(baseType))
            {
                _derivedTypes.Add(baseType, new());
            }
            _derivedTypes[baseType].Add(type);
        }
    }
    #endregion

    #region KResourceHandler
    public class KResourceHandler : KEngineComponent
    {
        public string GameDataDirectory = "Dat";
        public static KPolyTypeResolver polyTypeResolver = new();
        public static JsonSerializerOptions serializerOptions = new()
        {
            WriteIndented = true,
            IncludeFields = true,
            TypeInfoResolver = polyTypeResolver
        };

        public override void Init()
        {
            IEnumerable<Type> componentTypes = Assembly.GetCallingAssembly().GetTypes()
                .Where((Type t) => t.IsSubclassOf(typeof(KObjectComponent)));

            foreach (var componentType in componentTypes)
            {
                polyTypeResolver.AddDerivedType(typeof(KObjectComponent), componentType);
            }
        }

        public override void Start()
        {

        }

        public override void Update(uint currentTick)
        {
            throw new NotImplementedException();
        }

        public override void End()
        {
            throw new NotImplementedException();
        }

        public override void FrameUpdate(uint currentFrame)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
