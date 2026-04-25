using GoodHamburger.Domain.Accounts.Entities;
using GoodHamburger.Domain.Repositories.Accounts;
using GoodHamburger.Shared.Handlers;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.AccountServices.Queries;

public interface IGetCustomerProfileByIdHandler
{
    Task<Result<GetCustomerProfileByIdResponse>> HandleAsync(GetCustomerProfileByIdQuery query, CancellationToken cancellationToken = default);
}

public interface IGetCustomerProfileByIdentityIdHandler
{
    Task<Result<GetCustomerProfileByIdentityIdResponse>> HandleAsync(GetCustomerProfileByIdentityIdQuery query, CancellationToken cancellationToken = default);
}

public interface IGetCustomerProfileByDocumentHandler
{
    Task<Result<GetCustomerProfileByDocumentResponse>> HandleAsync(GetCustomerProfileByDocumentQuery query, CancellationToken cancellationToken = default);
}

public interface IGetAllCustomerProfilesHandler
{
    Task<Result<GetAllCustomerProfilesResponse>> HandleAsync(GetAllCustomerProfilesQuery query, CancellationToken cancellationToken = default);
}

public interface IGetActiveCustomerProfilesHandler
{
    Task<Result<GetActiveCustomerProfilesResponse>> HandleAsync(GetActiveCustomerProfilesQuery query, CancellationToken cancellationToken = default);
}

public class GetCustomerProfileByIdHandler : IGetCustomerProfileByIdHandler
{
    private readonly ICustomerProfileRepository _customerProfileRepository;
    private readonly ILogger<GetCustomerProfileByIdHandler> _logger;

    public GetCustomerProfileByIdHandler(
        ICustomerProfileRepository customerProfileRepository,
        ILogger<GetCustomerProfileByIdHandler> logger)
    {
        _customerProfileRepository = customerProfileRepository;
        _logger = logger;
    }

    public async Task<Result<GetCustomerProfileByIdResponse>> HandleAsync(GetCustomerProfileByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var customerProfile = await _customerProfileRepository.GetByIdAsync(query.CustomerProfileId, cancellationToken);
            if (customerProfile == null)
            {
                _logger.LogWarning("Customer profile {CustomerProfileId} not found", query.CustomerProfileId);
                return Result<GetCustomerProfileByIdResponse>.Failure(
                    new Error("CustomerProfile.NotFound", "Customer profile not found.")
                );
            }

            return Result<GetCustomerProfileByIdResponse>.Success(MapToResponse(customerProfile));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving customer profile");
            return Result<GetCustomerProfileByIdResponse>.Failure(
                new Error("CustomerProfile.QueryError", "An error occurred while retrieving the customer profile")
            );
        }
    }

    private static GetCustomerProfileByIdResponse MapToResponse(CustomerProfile customerProfile)
    {
        return new GetCustomerProfileByIdResponse(
            customerProfile.Id,
            customerProfile.IdentityId,
            customerProfile.FullName,
            customerProfile.DisplayName,
            customerProfile.ProfilePictureUrl,
            customerProfile.IsActive,
            new DocumentDto(
                customerProfile.Document.Number,
                customerProfile.Document.DocumentType
            ),
            new AddressDto(
                customerProfile.DeliveryAddress.StreetTypeId,
                customerProfile.DeliveryAddress.StreetName,
                customerProfile.DeliveryAddress.Number,
                customerProfile.DeliveryAddress.NeighborhoodId,
                customerProfile.DeliveryAddress.ZipCode,
                customerProfile.DeliveryAddress.Complement
            ),
            customerProfile.BirthDate,
            customerProfile.CreatedAt,
            customerProfile.UpdatedAt
        );
    }
}

public class GetCustomerProfileByIdentityIdHandler : IGetCustomerProfileByIdentityIdHandler
{
    private readonly ICustomerProfileRepository _customerProfileRepository;
    private readonly ILogger<GetCustomerProfileByIdentityIdHandler> _logger;

    public GetCustomerProfileByIdentityIdHandler(
        ICustomerProfileRepository customerProfileRepository,
        ILogger<GetCustomerProfileByIdentityIdHandler> logger)
    {
        _customerProfileRepository = customerProfileRepository;
        _logger = logger;
    }

    public async Task<Result<GetCustomerProfileByIdentityIdResponse>> HandleAsync(GetCustomerProfileByIdentityIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var customerProfile = await _customerProfileRepository.GetByIdentityIdAsync(query.IdentityId, cancellationToken);
            if (customerProfile == null)
            {
                _logger.LogWarning("Customer profile for IdentityId {IdentityId} not found", query.IdentityId);
                return Result<GetCustomerProfileByIdentityIdResponse>.Failure(
                    new Error("CustomerProfile.NotFound", "Customer profile not found.")
                );
            }

            return Result<GetCustomerProfileByIdentityIdResponse>.Success(MapToResponse(customerProfile));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving customer profile by identity");
            return Result<GetCustomerProfileByIdentityIdResponse>.Failure(
                new Error("CustomerProfile.QueryError", "An error occurred while retrieving the customer profile")
            );
        }
    }

    private static GetCustomerProfileByIdentityIdResponse MapToResponse(CustomerProfile customerProfile)
    {
        return new GetCustomerProfileByIdentityIdResponse(
            customerProfile.Id,
            customerProfile.IdentityId,
            customerProfile.FullName,
            customerProfile.DisplayName,
            customerProfile.ProfilePictureUrl,
            customerProfile.IsActive,
            new DocumentDto(
                customerProfile.Document.Number,
                customerProfile.Document.DocumentType
            ),
            new AddressDto(
                customerProfile.DeliveryAddress.StreetTypeId,
                customerProfile.DeliveryAddress.StreetName,
                customerProfile.DeliveryAddress.Number,
                customerProfile.DeliveryAddress.NeighborhoodId,
                customerProfile.DeliveryAddress.ZipCode,
                customerProfile.DeliveryAddress.Complement
            ),
            customerProfile.BirthDate,
            customerProfile.CreatedAt,
            customerProfile.UpdatedAt
        );
    }
}

public class GetCustomerProfileByDocumentHandler : IGetCustomerProfileByDocumentHandler
{
    private readonly ICustomerProfileRepository _customerProfileRepository;
    private readonly ILogger<GetCustomerProfileByDocumentHandler> _logger;

    public GetCustomerProfileByDocumentHandler(
        ICustomerProfileRepository customerProfileRepository,
        ILogger<GetCustomerProfileByDocumentHandler> logger)
    {
        _customerProfileRepository = customerProfileRepository;
        _logger = logger;
    }

    public async Task<Result<GetCustomerProfileByDocumentResponse>> HandleAsync(GetCustomerProfileByDocumentQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var customerProfile = await _customerProfileRepository.GetByDocumentAsync(query.DocumentNumber, cancellationToken);
            if (customerProfile == null)
            {
                _logger.LogWarning("Customer profile for document {DocumentNumber} not found", query.DocumentNumber);
                return Result<GetCustomerProfileByDocumentResponse>.Failure(
                    new Error("CustomerProfile.NotFound", "Customer profile not found.")
                );
            }

            return Result<GetCustomerProfileByDocumentResponse>.Success(MapToResponse(customerProfile));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving customer profile by document");
            return Result<GetCustomerProfileByDocumentResponse>.Failure(
                new Error("CustomerProfile.QueryError", "An error occurred while retrieving the customer profile")
            );
        }
    }

    private static GetCustomerProfileByDocumentResponse MapToResponse(CustomerProfile customerProfile)
    {
        return new GetCustomerProfileByDocumentResponse(
            customerProfile.Id,
            customerProfile.IdentityId,
            customerProfile.FullName,
            customerProfile.DisplayName,
            customerProfile.ProfilePictureUrl,
            customerProfile.IsActive,
            new DocumentDto(
                customerProfile.Document.Number,
                customerProfile.Document.DocumentType
            ),
            new AddressDto(
                customerProfile.DeliveryAddress.StreetTypeId,
                customerProfile.DeliveryAddress.StreetName,
                customerProfile.DeliveryAddress.Number,
                customerProfile.DeliveryAddress.NeighborhoodId,
                customerProfile.DeliveryAddress.ZipCode,
                customerProfile.DeliveryAddress.Complement
            ),
            customerProfile.BirthDate,
            customerProfile.CreatedAt,
            customerProfile.UpdatedAt
        );
    }
}

public class GetAllCustomerProfilesHandler : IGetAllCustomerProfilesHandler
{
    private readonly ICustomerProfileRepository _customerProfileRepository;
    private readonly ILogger<GetAllCustomerProfilesHandler> _logger;

    public GetAllCustomerProfilesHandler(
        ICustomerProfileRepository customerProfileRepository,
        ILogger<GetAllCustomerProfilesHandler> logger)
    {
        _customerProfileRepository = customerProfileRepository;
        _logger = logger;
    }

    public async Task<Result<GetAllCustomerProfilesResponse>> HandleAsync(GetAllCustomerProfilesQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var allCustomers = await _customerProfileRepository.GetAllAsync(cancellationToken);
            
            // Apply search filter if provided
            var filteredCustomers = allCustomers;
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.ToLower();
                filteredCustomers = allCustomers
                    .Where(c => c.FullName.ToLower().Contains(search) || 
                                c.DisplayName.ToLower().Contains(search) ||
                                c.Document.Number.Contains(search))
                    .ToList();
            }

            var totalCount = filteredCustomers.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

            var pagedCustomers = filteredCustomers
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(MapToDto)
                .ToList();

            return Result<GetAllCustomerProfilesResponse>.Success(new GetAllCustomerProfilesResponse(
                pagedCustomers,
                totalCount,
                query.Page,
                query.PageSize,
                totalPages
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving customer profiles");
            return Result<GetAllCustomerProfilesResponse>.Failure(
                new Error("CustomerProfile.QueryError", "An error occurred while retrieving the customer profiles")
            );
        }
    }

    private static CustomerProfileDto MapToDto(CustomerProfile customerProfile)
    {
        return new CustomerProfileDto(
            customerProfile.Id,
            customerProfile.IdentityId,
            customerProfile.FullName,
            customerProfile.DisplayName,
            customerProfile.ProfilePictureUrl,
            customerProfile.IsActive,
            new DocumentDto(
                customerProfile.Document.Number,
                customerProfile.Document.DocumentType
            ),
            new AddressDto(
                customerProfile.DeliveryAddress.StreetTypeId,
                customerProfile.DeliveryAddress.StreetName,
                customerProfile.DeliveryAddress.Number,
                customerProfile.DeliveryAddress.NeighborhoodId,
                customerProfile.DeliveryAddress.ZipCode,
                customerProfile.DeliveryAddress.Complement
            ),
            customerProfile.BirthDate,
            customerProfile.CreatedAt,
            customerProfile.UpdatedAt
        );
    }
}

public class GetActiveCustomerProfilesHandler : IGetActiveCustomerProfilesHandler
{
    private readonly ICustomerProfileRepository _customerProfileRepository;
    private readonly ILogger<GetActiveCustomerProfilesHandler> _logger;

    public GetActiveCustomerProfilesHandler(
        ICustomerProfileRepository customerProfileRepository,
        ILogger<GetActiveCustomerProfilesHandler> logger)
    {
        _customerProfileRepository = customerProfileRepository;
        _logger = logger;
    }

    public async Task<Result<GetActiveCustomerProfilesResponse>> HandleAsync(GetActiveCustomerProfilesQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var activeCustomers = await _customerProfileRepository.GetActiveCustomersAsync(cancellationToken);
            
            var totalCount = activeCustomers.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

            var pagedCustomers = activeCustomers
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(MapToDto)
                .ToList();

            return Result<GetActiveCustomerProfilesResponse>.Success(new GetActiveCustomerProfilesResponse(
                pagedCustomers,
                totalCount,
                query.Page,
                query.PageSize,
                totalPages
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving active customer profiles");
            return Result<GetActiveCustomerProfilesResponse>.Failure(
                new Error("CustomerProfile.QueryError", "An error occurred while retrieving the active customer profiles")
            );
        }
    }

    private static CustomerProfileDto MapToDto(CustomerProfile customerProfile)
    {
        return new CustomerProfileDto(
            customerProfile.Id,
            customerProfile.IdentityId,
            customerProfile.FullName,
            customerProfile.DisplayName,
            customerProfile.ProfilePictureUrl,
            customerProfile.IsActive,
            new DocumentDto(
                customerProfile.Document.Number,
                customerProfile.Document.DocumentType
            ),
            new AddressDto(
                customerProfile.DeliveryAddress.StreetTypeId,
                customerProfile.DeliveryAddress.StreetName,
                customerProfile.DeliveryAddress.Number,
                customerProfile.DeliveryAddress.NeighborhoodId,
                customerProfile.DeliveryAddress.ZipCode,
                customerProfile.DeliveryAddress.Complement
            ),
            customerProfile.BirthDate,
            customerProfile.CreatedAt,
            customerProfile.UpdatedAt
        );
    }
}
