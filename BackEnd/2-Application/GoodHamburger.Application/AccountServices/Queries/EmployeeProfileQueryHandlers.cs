using GoodHamburger.Domain.Accounts.Entities;
using GoodHamburger.Domain.Repositories.Accounts;
using GoodHamburger.Shared.Handlers;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.AccountServices.Queries;

public interface IGetEmployeeProfileByIdHandler
{
    Task<Result<GetEmployeeProfileByIdResponse>> HandleAsync(GetEmployeeProfileByIdQuery query, CancellationToken cancellationToken = default);
}

public interface IGetEmployeeProfileByIdentityIdHandler
{
    Task<Result<GetEmployeeProfileByIdentityIdResponse>> HandleAsync(GetEmployeeProfileByIdentityIdQuery query, CancellationToken cancellationToken = default);
}

public interface IGetAllEmployeeProfilesHandler
{
    Task<Result<GetAllEmployeeProfilesResponse>> HandleAsync(GetAllEmployeeProfilesQuery query, CancellationToken cancellationToken = default);
}

public interface IGetActiveEmployeeProfilesHandler
{
    Task<Result<GetActiveEmployeeProfilesResponse>> HandleAsync(GetActiveEmployeeProfilesQuery query, CancellationToken cancellationToken = default);
}

public class GetEmployeeProfileByIdHandler : IGetEmployeeProfileByIdHandler
{
    private readonly IEmployeeProfileRepository _employeeProfileRepository;
    private readonly ILogger<GetEmployeeProfileByIdHandler> _logger;

    public GetEmployeeProfileByIdHandler(
        IEmployeeProfileRepository employeeProfileRepository,
        ILogger<GetEmployeeProfileByIdHandler> logger)
    {
        _employeeProfileRepository = employeeProfileRepository;
        _logger = logger;
    }

    public async Task<Result<GetEmployeeProfileByIdResponse>> HandleAsync(GetEmployeeProfileByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var employeeProfile = await _employeeProfileRepository.GetByIdAsync(query.EmployeeProfileId, cancellationToken);
            if (employeeProfile == null)
            {
                _logger.LogWarning("Employee profile {EmployeeProfileId} not found", query.EmployeeProfileId);
                return Result<GetEmployeeProfileByIdResponse>.Failure(
                    new Error("EmployeeProfile.NotFound", "Employee profile not found.")
                );
            }

            return Result<GetEmployeeProfileByIdResponse>.Success(MapToResponse(employeeProfile));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving employee profile");
            return Result<GetEmployeeProfileByIdResponse>.Failure(
                new Error("EmployeeProfile.QueryError", "An error occurred while retrieving the employee profile")
            );
        }
    }

    private static GetEmployeeProfileByIdResponse MapToResponse(EmployeeProfile employeeProfile)
    {
        return new GetEmployeeProfileByIdResponse(
            employeeProfile.Id,
            employeeProfile.IdentityId,
            employeeProfile.FullName,
            employeeProfile.DisplayName,
            employeeProfile.EmployeeCode,
            employeeProfile.RoleTitle,
            employeeProfile.ProfilePictureUrl,
            employeeProfile.IsActive,
            employeeProfile.CreatedAt,
            employeeProfile.UpdatedAt
        );
    }
}

public class GetEmployeeProfileByIdentityIdHandler : IGetEmployeeProfileByIdentityIdHandler
{
    private readonly IEmployeeProfileRepository _employeeProfileRepository;
    private readonly ILogger<GetEmployeeProfileByIdentityIdHandler> _logger;

    public GetEmployeeProfileByIdentityIdHandler(
        IEmployeeProfileRepository employeeProfileRepository,
        ILogger<GetEmployeeProfileByIdentityIdHandler> logger)
    {
        _employeeProfileRepository = employeeProfileRepository;
        _logger = logger;
    }

    public async Task<Result<GetEmployeeProfileByIdentityIdResponse>> HandleAsync(GetEmployeeProfileByIdentityIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var employeeProfile = await _employeeProfileRepository.GetByIdentityIdAsync(query.IdentityId, cancellationToken);
            if (employeeProfile == null)
            {
                _logger.LogWarning("Employee profile for IdentityId {IdentityId} not found", query.IdentityId);
                return Result<GetEmployeeProfileByIdentityIdResponse>.Failure(
                    new Error("EmployeeProfile.NotFound", "Employee profile not found.")
                );
            }

            return Result<GetEmployeeProfileByIdentityIdResponse>.Success(MapToResponse(employeeProfile));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving employee profile by identity");
            return Result<GetEmployeeProfileByIdentityIdResponse>.Failure(
                new Error("EmployeeProfile.QueryError", "An error occurred while retrieving the employee profile")
            );
        }
    }

    private static GetEmployeeProfileByIdentityIdResponse MapToResponse(EmployeeProfile employeeProfile)
    {
        return new GetEmployeeProfileByIdentityIdResponse(
            employeeProfile.Id,
            employeeProfile.IdentityId,
            employeeProfile.FullName,
            employeeProfile.DisplayName,
            employeeProfile.EmployeeCode,
            employeeProfile.RoleTitle,
            employeeProfile.ProfilePictureUrl,
            employeeProfile.IsActive,
            employeeProfile.CreatedAt,
            employeeProfile.UpdatedAt
        );
    }
}

public class GetAllEmployeeProfilesHandler : IGetAllEmployeeProfilesHandler
{
    private readonly IEmployeeProfileRepository _employeeProfileRepository;
    private readonly ILogger<GetAllEmployeeProfilesHandler> _logger;

    public GetAllEmployeeProfilesHandler(
        IEmployeeProfileRepository employeeProfileRepository,
        ILogger<GetAllEmployeeProfilesHandler> logger)
    {
        _employeeProfileRepository = employeeProfileRepository;
        _logger = logger;
    }

    public async Task<Result<GetAllEmployeeProfilesResponse>> HandleAsync(GetAllEmployeeProfilesQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var allEmployees = await _employeeProfileRepository.GetAllAsync(cancellationToken);
            
            // Apply search filter if provided
            var filteredEmployees = allEmployees;
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.ToLower();
                filteredEmployees = allEmployees
                    .Where(e => e.FullName.ToLower().Contains(search) || 
                                e.DisplayName.ToLower().Contains(search) ||
                                e.EmployeeCode.ToLower().Contains(search) ||
                                e.RoleTitle.ToLower().Contains(search))
                    .ToList();
            }

            var totalCount = filteredEmployees.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

            var pagedEmployees = filteredEmployees
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(MapToDto)
                .ToList();

            return Result<GetAllEmployeeProfilesResponse>.Success(new GetAllEmployeeProfilesResponse(
                pagedEmployees,
                totalCount,
                query.Page,
                query.PageSize,
                totalPages
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving employee profiles");
            return Result<GetAllEmployeeProfilesResponse>.Failure(
                new Error("EmployeeProfile.QueryError", "An error occurred while retrieving the employee profiles")
            );
        }
    }

    private static EmployeeProfileDto MapToDto(EmployeeProfile employeeProfile)
    {
        return new EmployeeProfileDto(
            employeeProfile.Id,
            employeeProfile.IdentityId,
            employeeProfile.FullName,
            employeeProfile.DisplayName,
            employeeProfile.EmployeeCode,
            employeeProfile.RoleTitle,
            employeeProfile.ProfilePictureUrl,
            employeeProfile.IsActive,
            employeeProfile.CreatedAt,
            employeeProfile.UpdatedAt
        );
    }
}

public class GetActiveEmployeeProfilesHandler : IGetActiveEmployeeProfilesHandler
{
    private readonly IEmployeeProfileRepository _employeeProfileRepository;
    private readonly ILogger<GetActiveEmployeeProfilesHandler> _logger;

    public GetActiveEmployeeProfilesHandler(
        IEmployeeProfileRepository employeeProfileRepository,
        ILogger<GetActiveEmployeeProfilesHandler> logger)
    {
        _employeeProfileRepository = employeeProfileRepository;
        _logger = logger;
    }

    public async Task<Result<GetActiveEmployeeProfilesResponse>> HandleAsync(GetActiveEmployeeProfilesQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var activeEmployees = await _employeeProfileRepository.GetActiveEmployeesAsync(cancellationToken);
            
            var totalCount = activeEmployees.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

            var pagedEmployees = activeEmployees
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(MapToDto)
                .ToList();

            return Result<GetActiveEmployeeProfilesResponse>.Success(new GetActiveEmployeeProfilesResponse(
                pagedEmployees,
                totalCount,
                query.Page,
                query.PageSize,
                totalPages
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving active employee profiles");
            return Result<GetActiveEmployeeProfilesResponse>.Failure(
                new Error("EmployeeProfile.QueryError", "An error occurred while retrieving the active employee profiles")
            );
        }
    }

    private static EmployeeProfileDto MapToDto(EmployeeProfile employeeProfile)
    {
        return new EmployeeProfileDto(
            employeeProfile.Id,
            employeeProfile.IdentityId,
            employeeProfile.FullName,
            employeeProfile.DisplayName,
            employeeProfile.EmployeeCode,
            employeeProfile.RoleTitle,
            employeeProfile.ProfilePictureUrl,
            employeeProfile.IsActive,
            employeeProfile.CreatedAt,
            employeeProfile.UpdatedAt
        );
    }
}
