using System.Net.Http.Json;

namespace GoodHamburger.FrontEnd.Clients.Coupons;

public interface ICouponsClient
{
    Task<CreateCouponResponse> CreateCouponAsync(CreateCouponCommand command);
    Task<GetAllCouponsResponse> GetAllCouponsAsync(int page = 1, int pageSize = 10);
    Task<GetCouponByIdResponse> GetCouponByIdAsync(Guid id);
    Task<GetCouponByCodeResponse> GetCouponByCodeAsync(string code);
    Task<GetActiveCouponsResponse> GetActiveCouponsAsync(int page = 1, int pageSize = 10);
    Task<UpdateCouponResponse> UpdateCouponAsync(Guid id, UpdateCouponCommand command);
    Task<CancelCouponResponse> CancelCouponAsync(Guid id);
}

public class CouponsClient : ICouponsClient
{
    private readonly HttpClient _httpClient;

    public CouponsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<CreateCouponResponse> CreateCouponAsync(CreateCouponCommand command) 
        => PostAsync<CreateCouponResponse>("api/Coupons", command);
    
    public Task<GetAllCouponsResponse> GetAllCouponsAsync(int page = 1, int pageSize = 10) 
        => GetAsync<GetAllCouponsResponse>($"api/Coupons?page={page}&pageSize={pageSize}");
    
    public Task<GetCouponByIdResponse> GetCouponByIdAsync(Guid id) 
        => GetAsync<GetCouponByIdResponse>($"api/Coupons/{id}");
    
    public Task<GetCouponByCodeResponse> GetCouponByCodeAsync(string code) 
        => GetAsync<GetCouponByCodeResponse>($"api/Coupons/code/{code}");
    
    public Task<GetActiveCouponsResponse> GetActiveCouponsAsync(int page = 1, int pageSize = 10) 
        => GetAsync<GetActiveCouponsResponse>($"api/Coupons/active?page={page}&pageSize={pageSize}");
    
    public async Task<UpdateCouponResponse> UpdateCouponAsync(Guid id, UpdateCouponCommand command)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/Coupons/{id}", command);
        return await response.Content.ReadFromJsonAsync<UpdateCouponResponse>() ?? throw new InvalidOperationException();
    }
    
    public Task<CancelCouponResponse> CancelCouponAsync(Guid id) 
        => PostAsync<CancelCouponResponse>($"api/Coupons/{id}/cancel", null);

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
