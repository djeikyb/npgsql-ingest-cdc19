using System.Text;

namespace Core.Tsv;

public interface ILabeled
{
    string Label { get; }
}

public class MultiKeyPreknowns<T> where T : ILabeled
{
    private readonly Dictionary<string, T> _byString = new();
    private readonly Dictionary<byte[], T> _byBytes = new();

    public MultiKeyPreknowns(IDictionary<string, T> d)
    {
        foreach (var (k, v) in d)
        {
            this[k] = v;
        }
    }

    public T this[string k]
    {
        get => _byString[k];
        set
        {
            _byString[k] = value;
            _byBytes[Encoding.UTF8.GetBytes(value.Label)] = value;
        }
    }

    public T this[byte[] k]
    {
        get => _byBytes[k];
        set
        {
            _byString[value.Label] = value;
            _byBytes[k] = value;
        }
    }
}
