using System.Collections.Concurrent;
using System.Collections;
using System.ComponentModel.Design;

namespace Meziantou.Framework.Http;

public sealed partial class HstsDomainPolicyCollection : IEnumerable<HstsDomainPolicy>
{
    private readonly List<ConcurrentDictionary<string, HstsDomainPolicy>> _policies = new(capacity: 8);
    private readonly TimeProvider _timeProvider;

    public static HstsDomainPolicyCollection Default { get; } = new HstsDomainPolicyCollection();

    public HstsDomainPolicyCollection(bool includePreloadDomains = true)
        : this(timeProvider: null, includePreloadDomains)
    {
    }

    public HstsDomainPolicyCollection(TimeProvider? timeProvider, bool includePreloadDomains = true)
    {
        _timeProvider = timeProvider ?? TimeProvider.System;
        if (includePreloadDomains)
        {
            Initialize(_timeProvider);
        }
    }

    public void Add(string host, TimeSpan maxAge, bool includeSubdomains)
    {
        Add(host, _timeProvider.GetUtcNow().Add(maxAge), includeSubdomains);
    }

    public void Add(string host, DateTimeOffset expiresAt, bool includeSubdomains)
    {
    }

    public bool Match(string host)
    {
        return false;
    }

    public IEnumerator<HstsDomainPolicy> GetEnumerator()
    {
        for (var i = 0; i < _policies.Count; i++)
        {
            var dictionary = _policies[i];
            if (dictionary is null)
                continue;

            foreach (var kvp in dictionary)
            {
                yield return kvp.Value;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
