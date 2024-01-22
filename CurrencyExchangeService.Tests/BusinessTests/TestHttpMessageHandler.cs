namespace CurrencyExchangeService.Tests.BusinessTests
{
    public class TestHttpMessageHandler : HttpMessageHandler
    {
        public HttpResponseMessage Response { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Response);
        }

    }
}
