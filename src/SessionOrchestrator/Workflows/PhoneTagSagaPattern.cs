using SessionOrchestrator.Transactions;

namespace SessionOrchestrator.Workflows;

[CommunicationType(Communication.SYNC)]
[ConsistencyType(Consistency.Atomic)]
[CoordinationType(Coordination.Choreography)]
public class PhoneTagSagaPattern
{
   /* This is a saga pattern that is based on the following:
    * Communication: Synchronous
    * Consistency: Atomic
    * Coordination: Choreography
    * This means that there is no central coordinator, and each service is responsible for its own transaction.
    * For session end => Session service will be responsible for the transaction and act as front controller.
    */
}