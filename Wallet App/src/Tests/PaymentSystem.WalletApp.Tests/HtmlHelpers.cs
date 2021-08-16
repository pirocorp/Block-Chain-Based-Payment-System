namespace PaymentSystem.WalletApp.Tests
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using AngleSharp;
    using AngleSharp.Html.Dom;
    using AngleSharp.Io;

    public static class HtmlHelpers
    {
        public static async Task<IHtmlDocument> GetDocumentAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var document = await BrowsingContext.New()
                .OpenAsync(
                    (htmlResponse) => ResponseFactory(htmlResponse, response, content),
                    CancellationToken.None);

            return (IHtmlDocument)document;
        }

        private static void ResponseFactory(VirtualResponse htmlResponse, HttpResponseMessage response, string content)
        {
            htmlResponse
                .Address(response.RequestMessage.RequestUri)
                .Status(response.StatusCode);

            MapHeaders(response.Headers, htmlResponse);
            MapHeaders(response.Content.Headers, htmlResponse);

            htmlResponse.Content(content);
        }

        private static void MapHeaders(HttpHeaders headers, VirtualResponse htmlResponse)
        {
            foreach (var header in headers)
            {
                foreach (var value in header.Value)
                {
                    htmlResponse.Header(header.Key, value);
                }
            }
        }
    }
}
