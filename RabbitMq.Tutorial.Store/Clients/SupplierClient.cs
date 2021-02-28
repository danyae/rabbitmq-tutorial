using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RabbitMq.Tutorial.Store.Clients.Models;
using RabbitMq.Tutorial.Store.Options;

namespace RabbitMq.Tutorial.Store.Clients
{
    public class SupplierClient
    {
        private const string BasePath = "api";
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true,
        };
        
        private readonly IHttpClientFactory _clientFactory;
        private readonly SupplierOptions _supplierOptions;

        public SupplierClient(IHttpClientFactory clientFactory, IOptions<SupplierOptions> supplierOptions)
        {
            _clientFactory = clientFactory;
            _supplierOptions = supplierOptions.Value;
        }

        private HttpClient CreateClient()
        {
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri($"{_supplierOptions.HostName}:{_supplierOptions.Port.ToString()}/{BasePath}/");
            return client;
        }

        public async Task<IEnumerable<Product>> GetProducts(CancellationToken ct = default)
        {
            var client = CreateClient();

            var response = await client.GetAsync("products", ct);
            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync(ct);
            
            return await JsonSerializer.DeserializeAsync<IEnumerable<Product>>(responseStream, Options, ct);
        }

        public async Task<IEnumerable<Stock>> GetStocks(CancellationToken ct = default)
        {
            var client = CreateClient();

            var response = await client.GetAsync("stock", ct);
            response.EnsureSuccessStatusCode();
            
            await using var responseStream = await response.Content.ReadAsStreamAsync(ct);
            
            return await JsonSerializer.DeserializeAsync<IEnumerable<Stock>>(responseStream, Options, ct);
        }
    }
}
