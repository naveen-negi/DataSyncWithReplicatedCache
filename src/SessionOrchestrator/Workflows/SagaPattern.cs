using Microsoft.AspNetCore.Mvc;
using SessionOrchestrator.Controllers;
using SessionOrchestrator.Services;

namespace SessionOrchestrator.Workflows;

public class SagaPattern
{

    private readonly SessionServiceAPI _sessionServiceApi = new();
    private readonly ProductPricingServiceAPI _productPricingServiceApi = new();
    private readonly PaymentServiceClient _paymentServiceClient = new();

    public SessionTransactionResult SessionTransactionResult(SessionEndRequest request)
    {
        /*
         * Flow
         * session
         * pricing
         * payments
         * invoicing (if payment was not successful or missing payment details).
         * 
         * TODO: Another question, do we need a database on this service ?
         * TODO: Workflow state management
         *  Why do we need a database in orchestrator
         * Answer: If we don't .. how do we know if a session was billed or invoiced to customer. Who maintains this data
         * each service could save the session_id and associated meta data .. and what about the error state or half run workflows ?
         * for example payment service is down .. in this case if you want to know the full state of a session .. you have no where to go 
         * TODO: Do we need to do a lock on other services, so that same session is not processed twice ?
         * I think goal of distributed transaction is to have an atomic transaction
         * if any service fails we need to roll back changes
         * This means ...
         * If we fail to calculatePrice (because, perhaps Pricing service need to get some components from external service (think roaming tariff)
         * should we revert stop session changes, meaning revert session back to started state.
         * This sound preposterous, but we actually need to do this and send an error back to user.
         * When user sees an error, user would try again.
         * Who prepares the compensating transactions .. orchestractor or the service
         * Should I handle it with the try catch
         * What if compensating transaction fails.
         */
        var stoppedSession = _sessionServiceApi.StopSession(request);
        PricingCalculation? sessionWithPrice;
        try
        {
            sessionWithPrice = _productPricingServiceApi.CalculatePrice(stoppedSession);
        }
        catch (Exception e)
        {
            // If Price Calculation fails. We would need to rollback the changes in session service
            _sessionServiceApi.RollBackStopSessionUpdate(request);
            return new SessionTransactionResult(false, null);
        }

        try
        {
            var paymentResult = _paymentServiceClient.ChargeCustomer(sessionWithPrice);
            return new SessionTransactionResult(true, paymentResult);
        }
        catch (Exception e)
        {
            _sessionServiceApi.RollBackStopSessionUpdate(request);
            _productPricingServiceApi.RollBackPricingUpdate(stoppedSession);
            return new SessionTransactionResult(false, null);
        }
    }
}