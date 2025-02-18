namespace CommonTypes.Extensions
{
    public interface IInterpolationString
    {
        string Generate(Func<string, string> reslove);
        ValueTask<string> Generate(Func<string, ValueTask<string>> reslove);
    }
}
