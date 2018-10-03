# Polly Logging Context Sample

A small sample project showing how to use the Polly Context to pass a logger during execution of a policy.

This is useful if you want to log during any of the delegates provided for the various policies, such as OnRetry in this example.

NOTE: This is not intended as final production code, simply a sample of using the context to pass a logger. In general I've applied other personal opinions/preferences for structuring the Polly related code.