// namespace Infra;
//
// public abstract class Batcherator<T> : IEnumerable<List<T>>, IEnumerator<List<T>>
//     where T : class
// {
//     private readonly int _batchSize;
//     private readonly StreamReader _clefReader;
//
//     /// <param name="batchSize">In lines</param>
//     /// <param name="clef">You are responsible for closing the stream</param>
//     public Batcherator(int batchSize, Stream clef)
//     {
//         _batchSize = batchSize;
//         _clefReader = new StreamReader(clef);
//     }
//
//     public abstract Task Do();
//
//     public async Task<bool> MoveNext()
//     {
//         Current = new List<T>();
//         for (int i = 0; i < _batchSize; i++)
//         {
//             var line = _clefReader.ReadLine();
//             if (line == null) break;
//
//             await Do().ConfigureAwait(false);
//             // lol there's prolly a better way if it matters
//             var e = new T();
//             var dict = JsonSerializer.Deserialize<IDictionary<string, object>>(line, JsonSerializerOptions);
//             e.Data = dict!;
//             Current.Add(e);
//         }
//
//         return Current.Count != 0;
//     }
//
//     public IEnumerator<List<T>> GetEnumerator() => this;
//     IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
//     public List<T>? Current { get; private set; }
//     object? IEnumerator.Current => Current;
//
//     /// <summary>
//     /// Not supported
//     /// </summary>
//     /// <exception cref="NotImplementedException"></exception>
//     public void Reset() => throw new NotImplementedException();
//
//     public void Dispose()
//     {
//     }
// }
