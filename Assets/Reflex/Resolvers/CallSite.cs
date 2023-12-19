public sealed class CallSite
{
    public string ClassName { get; }
    public string FunctionName { get; }
    public string Path { get; }
    public int Line { get; }

    public CallSite(string className, string functionName, string path, int line)
    {
        ClassName = className;
        FunctionName = functionName;
        Path = path;
        Line = line;
    }
}