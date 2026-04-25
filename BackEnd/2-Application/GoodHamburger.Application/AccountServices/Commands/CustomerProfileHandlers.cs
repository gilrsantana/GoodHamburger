using GoodHamburger.Database.Accounts.Entities;
using GoodHamburger.Domain.Accounts.Entities;
using GoodHamburger.Domain.Repositories.Accounts;
using GoodHamburger.Shared.Handlers;
using GoodHamburger.Shared.ValueObjects.Documents.Base;
using GoodHamburger.Shared.ValueObjects.Locations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.AccountServices.Commands;

public interface ICreateCustomerProfileHandler
{
    Task<Result<CreateCustomerProfileResponse>> HandleAsync(CreateCustomerProfileCommand command, CancellationToken cancellationToken = default);
}

public interface IUpdateCustomerProfileHandler
{
    Task<Result<UpdateCustomerProfileResponse>> HandleAsync(UpdateCustomerProfileCommand command, CancellationToken cancellationToken = default);
}

public interface IUpdateCustomerDocumentHandler
{
    Task<Result<UpdateCustomerDocumentResponse>> HandleAsync(UpdateCustomerDocumentCommand command, CancellationToken cancellationToken = default);
}

public interface IUpdateCustomerAddressHandler
{
    Task<Result<UpdateCustomerAddressResponse>> HandleAsync(UpdateCustomerAddressCommand command, CancellationToken cancellationToken = default);
}

public interface IUpdateCustomerBirthDateHandler
{
    Task<Result<UpdateCustomerBirthDateResponse>> HandleAsync(UpdateCustomerBirthDateCommand command, CancellationToken cancellationToken = default);
}

public interface IDeleteCustomerProfileHandler
{
    Task<Result<DeleteCustomerProfileResponse>> HandleAsync(DeleteCustomerProfileCommand command, CancellationToken cancellationToken = default);
}

public class CreateCustomerProfileHandler : ICreateCustomerProfileHandler
{
    private readonly ICustomerProfileRepository _customerProfileRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<CreateCustomerProfileHandler> _logger;

    public CreateCustomerProfileHandler(
        ICustomerProfileRepository customerProfileRepository,
        UserManager<ApplicationUser> userManager,
        ILogger<CreateCustomerProfileHandler> logger)
    {
        _customerProfileRepository = customerProfileRepository;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<CreateCustomerProfileResponse>> HandleAsync(CreateCustomerProfileCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate that the Identity user exists
            var identityUser = await _userManager.FindByIdAsync(command.IdentityId.ToString());
            if (identityUser == null)
            {
                _logger.LogWarning("Identity user {IdentityId} not found", command.IdentityId);
                return Result<CreateCustomerProfileResponse>.Failure(
                    new Error("CustomerProfile.IdentityNotFound", "Identity user not found. Please create the user account first.")
                );
            }

            // Check if a customer profile already exists for this IdentityId
            var existingProfile = await _customerProfileRepository.GetByIdentityIdAsync(command.IdentityId, cancellationToken);
            if (existingProfile != null)
            {
                _logger.LogWarning("Customer profile already exists for IdentityId {IdentityId}", command.IdentityId);
                return Result<CreateCustomerProfileResponse>.Failure(
                    new Error("CustomerProfile.AlreadyExists", "A customer profile already exists for this identity user.")
                );
            }

            // Check if document is already in use
            var existingByDocument = await _customerProfileRepository.GetByDocumentAsync(command.DocumentNumber, cancellationToken);
            if (existingByDocument != null)
            {
                _logger.LogWarning("Document {DocumentNumber} already registered", command.DocumentNumber);
                return Result<CreateCustomerProfileResponse>.Failure(
                    new Error("CustomerProfile.DocumentInUse", "This document number is already registered.")
                );
            }

            var document = Document.Create(command.DocumentNumber, command.DocumentType);
            var address = Address.Create(
                command.StreetTypeId,
                command.StreetName,
                command.Number,
                command.NeighborhoodId,
                command.ZipCode,
                command.Complement
            );

            var customerProfile = CustomerProfile.Create(
                command.IdentityId,
                command.FullName,
                command.DisplayName,
                document,
                address,
                command.BirthDate
            );

            if (!customerProfile.IsValid)
            {
                var errors = customerProfile.Notifications.Select(n => n.Message);
                _logger.LogWarning("Customer profile validation failed: {Errors}", string.Join(", ", errors));
                return Result<CreateCustomerProfileResponse>.Failure(
                    new Error("CustomerProfile.Validation", string.Join(", ", errors))
                );
            }

            await _customerProfileRepository.AddAsync(customerProfile, cancellationToken);

            _logger.LogInformation("Customer profile {CustomerId} created for IdentityId {IdentityId}", customerProfile.Id, command.IdentityId);

            return Result<CreateCustomerProfileResponse>.Success(new CreateCustomerProfileResponse(
                customerProfile.Id,
                customerProfile.IdentityId,
                customerProfile.FullName,
                customerProfile.DisplayName,
                customerProfile.Document.Number,
                customerProfile.Document.DocumentType,
                customerProfile.IsActive,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating customer profile");
            return Result<CreateCustomerProfileResponse>.Failure(
                new Error("CustomerProfile.CreationError", "An error occurred while creating the customer profile")
            );
        }
    }
}

public class UpdateCustomerProfileHandler : IUpdateCustomerProfileHandler
{
    private readonly ICustomerProfileRepository _customerProfileRepository;
    private readonly ILogger<UpdateCustomerProfileHandler> _logger;

    public UpdateCustomerProfileHandler(
        ICustomerProfileRepository customerProfileRepository,
        ILogger<UpdateCustomerProfileHandler> logger)
    {
        _customerProfileRepository = customerProfileRepository;
        _logger = logger;
    }

    public async Task<Result<UpdateCustomerProfileResponse>> HandleAsync(UpdateCustomerProfileCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var customerProfile = await _customerProfileRepository.GetByIdAsync(command.CustomerProfileId, cancellationToken);
            if (customerProfile == null)
            {
                _logger.LogWarning("Customer profile {CustomerProfileId} not found", command.CustomerProfileId);
                return Result<UpdateCustomerProfileResponse>.Failure(
                    new Error("CustomerProfile.NotFound", "Customer profile not found.")
                );
            }

            if (!string.IsNullOrWhiteSpace(command.FullName))
                customerProfile.UpdateFullNameCustomer(command.FullName);

            if (!string.IsNullOrWhiteSpace(command.DisplayName))
                customerProfile.UpdateDisplayNameCustomer(command.DisplayName);

            if (command.ProfilePictureUrl != null)
                customerProfile.UpdateProfilePictureCustomer(command.ProfilePictureUrl);

            if (command.IsActive.HasValue)
                customerProfile.UpdateActiveCustomer(command.IsActive.Value);

            if (!customerProfile.IsValid)
            {
                var errors = customerProfile.Notifications.Select(n => n.Message);
                _logger.LogWarning("Customer profile validation failed: {Errors}", string.Join(", ", errors));
                return Result<UpdateCustomerProfileResponse>.Failure(
                    new Error("CustomerProfile.Validation", string.Join(", ", errors))
                );
            }

            await _customerProfileRepository.UpdateAsync(customerProfile, cancellationToken);

            _logger.LogInformation("Customer profile {CustomerProfileId} updated", command.CustomerProfileId);

            return Result<UpdateCustomerProfileResponse>.Success(new UpdateCustomerProfileResponse(
                customerProfile.Id,
                customerProfile.FullName,
                customerProfile.DisplayName,
                customerProfile.IsActive,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating customer profile");
            return Result<UpdateCustomerProfileResponse>.Failure(
                new Error("CustomerProfile.UpdateError", "An error occurred while updating the customer profile")
            );
        }
    }
}

public class UpdateCustomerDocumentHandler : IUpdateCustomerDocumentHandler
{
    private readonly ICustomerProfileRepository _customerProfileRepository;
    private readonly ILogger<UpdateCustomerDocumentHandler> _logger;

    public UpdateCustomerDocumentHandler(
        ICustomerProfileRepository customerProfileRepository,
        ILogger<UpdateCustomerDocumentHandler> logger)
    {
        _customerProfileRepository = customerProfileRepository;
        _logger = logger;
    }

    public async Task<Result<UpdateCustomerDocumentResponse>> HandleAsync(UpdateCustomerDocumentCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var customerProfile = await _customerProfileRepository.GetByIdAsync(command.CustomerProfileId, cancellationToken);
            if (customerProfile == null)
            {
                _logger.LogWarning("Customer profile {CustomerProfileId} not found", command.CustomerProfileId);
                return Result<UpdateCustomerDocumentResponse>.Failure(
                    new Error("CustomerProfile.NotFound", "Customer profile not found.")
                );
            }

            // Check if document is already in use by another customer
            var existingByDocument = await _customerProfileRepository.GetByDocumentAsync(command.DocumentNumber, cancellationToken);
            if (existingByDocument != null && existingByDocument.Id != command.CustomerProfileId)
            {
                _logger.LogWarning("Document {DocumentNumber} already registered to another customer", command.DocumentNumber);
                return Result<UpdateCustomerDocumentResponse>.Failure(
                    new Error("CustomerProfile.DocumentInUse", "This document number is already registered to another customer.")
                );
            }

            var newDocument = Document.Create(command.DocumentNumber, command.DocumentType);
            customerProfile.UpdateDocument(newDocument);

            if (!customerProfile.IsValid)
            {
                var errors = customerProfile.Notifications.Select(n => n.Message);
                _logger.LogWarning("Customer profile validation failed: {Errors}", string.Join(", ", errors));
                return Result<UpdateCustomerDocumentResponse>.Failure(
                    new Error("CustomerProfile.Validation", string.Join(", ", errors))
                );
            }

            await _customerProfileRepository.UpdateAsync(customerProfile, cancellationToken);

            _logger.LogInformation("Customer profile {CustomerProfileId} document updated", command.CustomerProfileId);

            return Result<UpdateCustomerDocumentResponse>.Success(new UpdateCustomerDocumentResponse(
                customerProfile.Id,
                customerProfile.Document.Number,
                customerProfile.Document.DocumentType,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating customer document");
            return Result<UpdateCustomerDocumentResponse>.Failure(
                new Error("CustomerProfile.DocumentUpdateError", "An error occurred while updating the customer document")
            );
        }
    }
}

public class UpdateCustomerAddressHandler : IUpdateCustomerAddressHandler
{
    private readonly ICustomerProfileRepository _customerProfileRepository;
    private readonly ILogger<UpdateCustomerAddressHandler> _logger;

    public UpdateCustomerAddressHandler(
        ICustomerProfileRepository customerProfileRepository,
        ILogger<UpdateCustomerAddressHandler> logger)
    {
        _customerProfileRepository = customerProfileRepository;
        _logger = logger;
    }

    public async Task<Result<UpdateCustomerAddressResponse>> HandleAsync(UpdateCustomerAddressCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var customerProfile = await _customerProfileRepository.GetByIdAsync(command.CustomerProfileId, cancellationToken);
            if (customerProfile == null)
            {
                _logger.LogWarning("Customer profile {CustomerProfileId} not found", command.CustomerProfileId);
                return Result<UpdateCustomerAddressResponse>.Failure(
                    new Error("CustomerProfile.NotFound", "Customer profile not found.")
                );
            }

            var newAddress = Address.Create(
                command.StreetTypeId,
                command.StreetName,
                command.Number,
                command.NeighborhoodId,
                command.ZipCode,
                command.Complement
            );

            customerProfile.UpdateDeliveryAddress(newAddress);

            if (!customerProfile.IsValid)
            {
                var errors = customerProfile.Notifications.Select(n => n.Message);
                _logger.LogWarning("Customer profile validation failed: {Errors}", string.Join(", ", errors));
                return Result<UpdateCustomerAddressResponse>.Failure(
                    new Error("CustomerProfile.Validation", string.Join(", ", errors))
                );
            }

            await _customerProfileRepository.UpdateAsync(customerProfile, cancellationToken);

            _logger.LogInformation("Customer profile {CustomerProfileId} address updated", command.CustomerProfileId);

            return Result<UpdateCustomerAddressResponse>.Success(new UpdateCustomerAddressResponse(
                customerProfile.Id,
                customerProfile.DeliveryAddress.StreetTypeId,
                customerProfile.DeliveryAddress.StreetName,
                customerProfile.DeliveryAddress.Number,
                customerProfile.DeliveryAddress.NeighborhoodId,
                customerProfile.DeliveryAddress.ZipCode,
                customerProfile.DeliveryAddress.Complement,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating customer address");
            return Result<UpdateCustomerAddressResponse>.Failure(
                new Error("CustomerProfile.AddressUpdateError", "An error occurred while updating the customer address")
            );
        }
    }
}

public class UpdateCustomerBirthDateHandler : IUpdateCustomerBirthDateHandler
{
    private readonly ICustomerProfileRepository _customerProfileRepository;
    private readonly ILogger<UpdateCustomerBirthDateHandler> _logger;

    public UpdateCustomerBirthDateHandler(
        ICustomerProfileRepository customerProfileRepository,
        ILogger<UpdateCustomerBirthDateHandler> logger)
    {
        _customerProfileRepository = customerProfileRepository;
        _logger = logger;
    }

    public async Task<Result<UpdateCustomerBirthDateResponse>> HandleAsync(UpdateCustomerBirthDateCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var customerProfile = await _customerProfileRepository.GetByIdAsync(command.CustomerProfileId, cancellationToken);
            if (customerProfile == null)
            {
                _logger.LogWarning("Customer profile {CustomerProfileId} not found", command.CustomerProfileId);
                return Result<UpdateCustomerBirthDateResponse>.Failure(
                    new Error("CustomerProfile.NotFound", "Customer profile not found.")
                );
            }

            customerProfile.UpdateBirthDate(command.BirthDate);

            if (!customerProfile.IsValid)
            {
                var errors = customerProfile.Notifications.Select(n => n.Message);
                _logger.LogWarning("Customer profile validation failed: {Errors}", string.Join(", ", errors));
                return Result<UpdateCustomerBirthDateResponse>.Failure(
                    new Error("CustomerProfile.Validation", string.Join(", ", errors))
                );
            }

            await _customerProfileRepository.UpdateAsync(customerProfile, cancellationToken);

            _logger.LogInformation("Customer profile {CustomerProfileId} birth date updated", command.CustomerProfileId);

            return Result<UpdateCustomerBirthDateResponse>.Success(new UpdateCustomerBirthDateResponse(
                customerProfile.Id,
                customerProfile.BirthDate,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating customer birth date");
            return Result<UpdateCustomerBirthDateResponse>.Failure(
                new Error("CustomerProfile.BirthDateUpdateError", "An error occurred while updating the customer birth date")
            );
        }
    }
}

public class DeleteCustomerProfileHandler : IDeleteCustomerProfileHandler
{
    private readonly ICustomerProfileRepository _customerProfileRepository;
    private readonly ILogger<DeleteCustomerProfileHandler> _logger;

    public DeleteCustomerProfileHandler(
        ICustomerProfileRepository customerProfileRepository,
        ILogger<DeleteCustomerProfileHandler> logger)
    {
        _customerProfileRepository = customerProfileRepository;
        _logger = logger;
    }

    public async Task<Result<DeleteCustomerProfileResponse>> HandleAsync(DeleteCustomerProfileCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var customerProfile = await _customerProfileRepository.GetByIdAsync(command.CustomerProfileId, cancellationToken);
            if (customerProfile == null)
            {
                _logger.LogWarning("Customer profile {CustomerProfileId} not found", command.CustomerProfileId);
                return Result<DeleteCustomerProfileResponse>.Failure(
                    new Error("CustomerProfile.NotFound", "Customer profile not found.")
                );
            }

            // Note: The repository doesn't have a Delete method, so we'll need to handle this differently
            // For now, we'll deactivate the profile instead
            customerProfile.UpdateActiveCustomer(false);
            await _customerProfileRepository.UpdateAsync(customerProfile, cancellationToken);

            _logger.LogInformation("Customer profile {CustomerProfileId} deactivated (soft delete)", command.CustomerProfileId);

            return Result<DeleteCustomerProfileResponse>.Success(new DeleteCustomerProfileResponse(
                customerProfile.Id,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting customer profile");
            return Result<DeleteCustomerProfileResponse>.Failure(
                new Error("CustomerProfile.DeleteError", "An error occurred while deleting the customer profile")
            );
        }
    }
}
