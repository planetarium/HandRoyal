using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace HandRoyal.Bot;

public sealed class JobCollection : IEnumerable<IJob>
{
    private readonly List<IJob> _jobs;
    private readonly Dictionary<string, IJob> _jobByName;
    private readonly Dictionary<Type, IJob> _jobByType;

    internal JobCollection(IJob[] jobs)
    {
        _jobs = new List<IJob>(jobs.Length);
        _jobByName = new Dictionary<string, IJob>(jobs.Length);
        _jobByType = new Dictionary<Type, IJob>(jobs.Length);
        foreach (var job in jobs)
        {
            _jobByName.Add(job.Name, job);
            _jobs.Add(job);
            _jobByType.Add(job.GetType(), job);
        }
    }

    public int Count => _jobs.Count;

    internal static JobCollection Empty { get; } = new([]);

    public IJob this[string name] => _jobByName[name];

    public IJob this[int index] => _jobs[index];

    public IJob this[Type type] => _jobByType[type];

    public bool Contains(IJob state) => _jobs.Contains(state);

    public bool Contains(string name) => _jobByName.ContainsKey(name);

    public bool Contains(Type type) => _jobByType.ContainsKey(type);

    public bool TryGetJob(string name, [MaybeNullWhen(false)] out IJob job)
        => _jobByName.TryGetValue(name, out job);

    public bool TryGetJob(Type type, [MaybeNullWhen(false)] out IJob job)
        => _jobByType.TryGetValue(type, out job);

    IEnumerator<IJob> IEnumerable<IJob>.GetEnumerator() => _jobs.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _jobs.GetEnumerator();
}
