During the implementation of SmsOutbound service I tried to be as abstract as possible, still wanted to maintain proper segregation between layers.

Having a constraint not to use real infrastructural integrations I would highlight following points of replacement with migration to real world communication:
- Commands consumer targets a message bus
- Events publisher also transfers messages to a dedicated message queue
- Proper idempotency check may be implemented with a persistant storage to synchronize several service instances
- Same storage will serve as an ooutbox pattern for failed messages. In such case RetryCount field shall be added.

Resilience policy should be adjusted for a specific API. Basic rule to retry resilient errors with an exponential backoff.
Rate limiter, in my opinion, should be carried by API provider itself. Can discuss pros/cons of such approach.

App uses .NET 9.0 and doesn't require any additional setup. Just build and run.