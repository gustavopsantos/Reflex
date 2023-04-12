using Reflex.Attributes;

public sealed class AttributeInjectedService
{
    public int NumberMethod;
    [Inject] public int NumberField;
    [Inject] public int NumberProperty { get; private set; }

    [Inject]
    private void Inject(int number)
    {
        NumberMethod = number;
    }
}