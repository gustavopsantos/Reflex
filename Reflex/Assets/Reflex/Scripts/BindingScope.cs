namespace Reflex
{
    //change size from 4 bytes to 1 byte
    internal enum BindingScope : byte
    {
        None,
        Transient,
        Singleton,
        Method
    }
}