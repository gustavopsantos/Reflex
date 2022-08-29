namespace IL2CPPTest.Models
{
    public interface ITestGenericStructure<T> where T : struct
    {
        T Value { get; set; }
    }
}