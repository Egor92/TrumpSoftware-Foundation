namespace TrumpSoftware.Common.Interfaces
{
    public interface IUpdatable<in T>
    {
        void Update(T source);
    }

    public static class UpdatableExtensions
    {
        public static TTarget UpdateAndReturn<TTarget, TSource>(this TTarget target, TSource source)
            where TTarget : IUpdatable<TSource>
        {
            target.Update(source);
            return target;
        }
    }
}
