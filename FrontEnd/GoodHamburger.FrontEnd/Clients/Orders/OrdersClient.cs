using System.Net.Http.Json;

namespace GoodHamburger.FrontEnd.Clients.Orders;

public interface IOrdersClient
{
    Task<CreateOrderResponse> CreateOrderAsync(CreateOrderCommand command);
    Task<GetAllOrdersResponse> GetAllOrdersAsync(int page = 1, int pageSize = 10);
    Task<GetOrderByIdResponse> GetOrderByIdAsync(Guid id);
    Task<GetOrdersByCustomerResponse> GetOrdersByCustomerAsync(Guid customerId, int page = 1, int pageSize = 10);
    Task<GetOrdersByStatusResponse> GetOrdersByStatusAsync(OrderStatus status, int page = 1, int pageSize = 10);
    Task<UpdateOrderStatusResponse> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusCommand command);
    Task<CancelOrderResponse> CancelOrderAsync(Guid id, string reason);
    Task<CheckoutCalculationResponse> CalculateCheckoutAsync(CheckoutCalculationQuery query);
    Task<ViaCepAddressDto?> GetAddressByZipCodeAsync(string zipCode);
}

public class OrdersClient : IOrdersClient
{
    private readonly HttpClient _httpClient;

    public OrdersClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<CreateOrderResponse> CreateOrderAsync(CreateOrderCommand command) 
        => PostAsync<CreateOrderResponse>("api/Orders", command);
    
    public Task<GetAllOrdersResponse> GetAllOrdersAsync(int page = 1, int pageSize = 10) 
        => GetAsync<GetAllOrdersResponse>($"api/Orders?page={page}&pageSize={pageSize}");
    
    public Task<GetOrderByIdResponse> GetOrderByIdAsync(Guid id) 
        => GetAsync<GetOrderByIdResponse>($"api/Orders/{id}");
    
    public Task<GetOrdersByCustomerResponse> GetOrdersByCustomerAsync(Guid customerId, int page = 1, int pageSize = 10) 
        => GetAsync<GetOrdersByCustomerResponse>($"api/Orders/customer/{customerId}?page={page}&pageSize={pageSize}");
    
    public Task<GetOrdersByStatusResponse> GetOrdersByStatusAsync(OrderStatus status, int page = 1, int pageSize = 10) 
        => GetAsync<GetOrdersByStatusResponse>($"api/Orders/status/{status}?page={page}&pageSize={pageSize}");
    
    public async Task<UpdateOrderStatusResponse> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusCommand command)
    {
        var response = await _httpClient.PatchAsJsonAsync($"api/Orders/{id}/status", command);
        return await response.Content.ReadFromJsonAsync<UpdateOrderStatusResponse>() ?? throw new InvalidOperationException();
    }
    
    public Task<CancelOrderResponse> CancelOrderAsync(Guid id, string reason) 
        => PostAsync<CancelOrderResponse>($"api/Orders/{id}/cancel", reason);
    
    public Task<CheckoutCalculationResponse> CalculateCheckoutAsync(CheckoutCalculationQuery query) 
        => PostAsync<CheckoutCalculationResponse>("api/Orders/checkout/calculate", query);
    
    public async Task<ViaCepAddressDto?> GetAddressByZipCodeAsync(string zipCode)
    {
        try
        {
            return await GetAsync<ViaCepAddressDto>($"api/Address/via-cep/{zipCode}");
        }
        catch
        {
            return null;
        }
    }

    private async Task<T> PostAsync<T>(string url, object? body)
    {
        var response = await _httpClient.PostAsJsonAsync(url, body);
        return await response.Content.ReadFromJsonAsync<T>() ?? throw new InvalidOperationException("Response body is null");
    }

    private async Task<T> GetAsync<T>(string url)
    {
        return await _httpClient.GetFromJsonAsync<T>(url) ?? throw new InvalidOperationException("Response body is null");
    }
}
