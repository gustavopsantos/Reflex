namespace Project.Code.DI
{
    public interface IReflexFactory<T>
    {
        public T Create();
    }
}