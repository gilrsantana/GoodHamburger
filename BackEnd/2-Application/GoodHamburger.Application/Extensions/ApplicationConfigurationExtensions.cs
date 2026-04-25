using GoodHamburger.Application.Services.Ordering.Contracts;
using GoodHamburger.Application.Services.Ordering.Helper;
using GoodHamburger.Application.Services.Ordering.Implementations;
using GoodHamburger.Application.IdentityServices.Commands;
using GoodHamburger.Application.IdentityServices.Queries;
using GoodHamburger.Application.IdentityServices.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GoodHamburger.Application.Extensions;

public static class ApplicationConfigurationExtensions
{
    public static IServiceCollection AddApplicationServicesConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IOrderDiscountService, CategoryDiscountService>();
        services.AddScoped<HashSetEqualityComparer>();

        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddScoped<ICreateUserHandler, CreateUserHandler>();
        services.AddScoped<IUpdateUserHandler, UpdateUserHandler>();
        services.AddScoped<IDeleteUserHandler, DeleteUserHandler>();
        services.AddScoped<ICreateRoleHandler, CreateRoleHandler>();
        services.AddScoped<IAssignRoleToUserHandler, AssignRoleToUserHandler>();
        services.AddScoped<IRemoveRoleFromUserHandler, RemoveRoleFromUserHandler>();
        services.AddScoped<IAddClaimToUserHandler, AddClaimToUserHandler>();
        services.AddScoped<IRemoveClaimFromUserHandler, RemoveClaimFromUserHandler>();
        services.AddScoped<IGenerateEmailConfirmationTokenHandler, GenerateEmailConfirmationTokenHandler>();
        services.AddScoped<IGeneratePasswordResetTokenHandler, GeneratePasswordResetTokenHandler>();
        services.AddScoped<IResetPasswordHandler, ResetPasswordHandler>();

        services.AddScoped<ILoginHandler, LoginHandler>();
        services.AddScoped<IRefreshTokenHandler, RefreshTokenHandler>();
        services.AddScoped<ILogoutHandler, LogoutHandler>();

        services.AddScoped<IGetUserByIdHandler, GetUserByIdHandler>();
        services.AddScoped<IGetAllUsersHandler, GetAllUsersHandler>();
        services.AddScoped<IGetAllRolesHandler, GetAllRolesHandler>();
        
        return services;
    }
}