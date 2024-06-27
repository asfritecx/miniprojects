using Amazon;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;

namespace aws_sestool
{
    internal class Program
    {
        private async static Task Main(string[] args)
        {
            var client = new AmazonSimpleEmailServiceV2Client("", "", RegionEndpoint.USEast1);

            var utility = new SimpleEmailServiceUtility(client);

            var result = await utility.GetSuppressionList();

            Console.WriteLine(result.ToString());
        }

    }

    public interface ISimpleEmailServiceUtility
    {
    }

    public class SimpleEmailServiceUtility : ISimpleEmailServiceUtility
    {
        private readonly IAmazonSimpleEmailServiceV2 _client;

        public SimpleEmailServiceUtility(IAmazonSimpleEmailServiceV2 client)
        {
            _client = client;
        }

        public async Task<ListSuppressedDestinationsResponse> GetSuppressionList()
        {
            ListSuppressedDestinationsRequest request = new ListSuppressedDestinationsRequest();
            request.PageSize = 10;
            request.NextToken = string.Empty;

            ListSuppressedDestinationsResponse response = new ListSuppressedDestinationsResponse();

            try
            {
                response = await _client.ListSuppressedDestinationsAsync(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ListSuppressedDestinationsAsync failed with exception: " + ex.Message);
            }

            return response;
        }
    }
}