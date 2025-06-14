During the implementation of the SmsOutbound service I tried to be as abstract as possible, still wanted to maintain proper segregation between layers. Tried to follow clean architecture for separation by layers, hence  business logic is isolated from infrastructure and host layers. 

Pipeline approach was choosen in order to simplify adding/modifying of stages and minimize impact to existing stages (OCP and SRP).

Comments describing points for replacement when switching to real integrations can be found in different places within the solution. 

Having a constraint not to use real infrastructural integrations I would highlight following points of replacement with migration to real world communication:
- Commands consumer targets a real message bus
- Events publisher also transfers messages to a dedicated message queue
- Proper idempotency check may be implemented with a persistant storage to synchronize several service instances
- Same storage will serve as an ooutbox pattern for failed messages. In such case RetryCount field shall be added.

Resilience policy should be adjusted for a specific API. Basic rule to retry resilient errors with an exponential backoff.
Rate limiter, in my opinion, should be carried by API provider itself. Can discuss pros/cons of such approach.

App uses .NET 9.0 and doesn't require any additional setup. Just build and run.

Solution was prepared in Visual Studio Code with usage of copilot for autosuggestions and autocompletion.