using Unity.Entities;

namespace Unity.DebugWatch
{

    public interface IWatch : IAccessor
    {
        string GetContextName();
        string GetName();
    }

    public interface IWatch<TValue> : IWatch, IAccessor<TValue>
    {
    }

    public interface IWorldWatch
    {
        World GetWorld();
    }


}