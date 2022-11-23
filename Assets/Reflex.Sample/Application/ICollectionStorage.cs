namespace Reflex.Sample.Application
{
    public interface ICollectionStorage
    {
        int Count();
        void Clear();
        void Add(string id);
        bool IsCollected(string id);
    }
}