# A Use Case of Immutable Collections

## [Medium Article: A Use Case of Immutable Collections](https://codeburst.io/a-use-case-of-immutable-collections-dd558f614722)

Caching can greatly improve an application’s performance by reducing the trips to fetch data that changes infrequently. The consistency of cached values is critical at run-time. At development time, we can avoid some human errors by setting caches to be **Immutable** Collections, so that cached values are not able to be mutated accidentally in code.

This blog post will use the simplest cache `IMemoryCache` in .NET Core to demonstrate usages of Immutable Collections, which is also native in .NET framework. The sample code is in this GitHub repository.
