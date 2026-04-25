using GoodHamburger.Application.Services.Ordering.Contracts;
using GoodHamburger.Application.Services.Ordering.Helper;
using GoodHamburger.Application.Services.Ordering.Implementations;
using GoodHamburger.Application.IdentityServices.Commands;
using GoodHamburger.Application.IdentityServices.Queries;
using GoodHamburger.Application.IdentityServices.Services;
using GoodHamburger.Application.CatalogServices.Commands;
using GoodHamburger.Application.CatalogServices.Queries;
using GoodHamburger.Application.AccountServices.Commands;
using GoodHamburger.Application.AccountServices.Queries;
using GoodHamburger.Application.OrderingServices.Commands;
using GoodHamburger.Application.OrderingServices.Queries;
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

        services.AddScoped<ICreateCategoryHandler, CreateCategoryHandler>();
        services.AddScoped<IUpdateCategoryHandler, UpdateCategoryHandler>();
        services.AddScoped<IDeleteCategoryHandler, DeleteCategoryHandler>();
        services.AddScoped<IActivateCategoryHandler, ActivateCategoryHandler>();
        services.AddScoped<IDeactivateCategoryHandler, DeactivateCategoryHandler>();
        services.AddScoped<IGetAllCategoriesHandler, GetAllCategoriesHandler>();
        services.AddScoped<IGetCategoryByIdHandler, GetCategoryByIdHandler>();
        services.AddScoped<IGetCategoryBySlugHandler, GetCategoryBySlugHandler>();
        services.AddScoped<IGetCategoriesByTypeHandler, GetCategoriesByTypeHandler>();
        services.AddScoped<IGetActiveCategoriesHandler, GetActiveCategoriesHandler>();

        services.AddScoped<ICreateIngredientHandler, CreateIngredientHandler>();
        services.AddScoped<IUpdateIngredientHandler, UpdateIngredientHandler>();
        services.AddScoped<IDeleteIngredientHandler, DeleteIngredientHandler>();
        services.AddScoped<IActivateIngredientHandler, ActivateIngredientHandler>();
        services.AddScoped<IDeactivateIngredientHandler, DeactivateIngredientHandler>();
        services.AddScoped<IGetAllIngredientsHandler, GetAllIngredientsHandler>();
        services.AddScoped<IGetIngredientByIdHandler, GetIngredientByIdHandler>();
        services.AddScoped<IGetActiveIngredientsHandler, GetActiveIngredientsHandler>();
        services.AddScoped<ISearchIngredientsHandler, SearchIngredientsHandler>();
        services.AddScoped<IGetIngredientsByPriceRangeHandler, GetIngredientsByPriceRangeHandler>();

        services.AddScoped<ICreateMenuItemHandler, CreateMenuItemHandler>();
        services.AddScoped<IUpdateMenuItemHandler, UpdateMenuItemHandler>();
        services.AddScoped<IDeleteMenuItemHandler, DeleteMenuItemHandler>();
        services.AddScoped<IActivateMenuItemHandler, ActivateMenuItemHandler>();
        services.AddScoped<IDeactivateMenuItemHandler, DeactivateMenuItemHandler>();
        services.AddScoped<ISetMenuItemAvailabilityHandler, SetMenuItemAvailabilityHandler>();
        services.AddScoped<IAddIngredientToMenuItemHandler, AddIngredientToMenuItemHandler>();
        services.AddScoped<IRemoveIngredientFromMenuItemHandler, RemoveIngredientFromMenuItemHandler>();
        services.AddScoped<IUpdateIngredientRemovabilityHandler, UpdateIngredientRemovabilityHandler>();
        services.AddScoped<IGetAllMenuItemsHandler, GetAllMenuItemsHandler>();
        services.AddScoped<IGetMenuItemByIdHandler, GetMenuItemByIdHandler>();
        services.AddScoped<IGetMenuItemBySkuHandler, GetMenuItemBySkuHandler>();
        services.AddScoped<IGetMenuItemsByCategoryHandler, GetMenuItemsByCategoryHandler>();
        services.AddScoped<IGetActiveMenuItemsHandler, GetActiveMenuItemsHandler>();
        services.AddScoped<IGetAvailableMenuItemsHandler, GetAvailableMenuItemsHandler>();
        services.AddScoped<ISearchMenuItemsHandler, SearchMenuItemsHandler>();
        services.AddScoped<IGetMenuItemsByPriceRangeHandler, GetMenuItemsByPriceRangeHandler>();
        
        services.AddScoped<ICreateCustomerProfileHandler, CreateCustomerProfileHandler>();
        services.AddScoped<IUpdateCustomerProfileHandler, UpdateCustomerProfileHandler>();
        services.AddScoped<IUpdateCustomerDocumentHandler, UpdateCustomerDocumentHandler>();
        services.AddScoped<IUpdateCustomerAddressHandler, UpdateCustomerAddressHandler>();
        services.AddScoped<IUpdateCustomerBirthDateHandler, UpdateCustomerBirthDateHandler>();
        services.AddScoped<IDeleteCustomerProfileHandler, DeleteCustomerProfileHandler>();
        services.AddScoped<IGetCustomerProfileByIdHandler, GetCustomerProfileByIdHandler>();
        services.AddScoped<IGetCustomerProfileByIdentityIdHandler, GetCustomerProfileByIdentityIdHandler>();
        services.AddScoped<IGetCustomerProfileByDocumentHandler, GetCustomerProfileByDocumentHandler>();
        services.AddScoped<IGetAllCustomerProfilesHandler, GetAllCustomerProfilesHandler>();
        services.AddScoped<IGetActiveCustomerProfilesHandler, GetActiveCustomerProfilesHandler>();
        
        services.AddScoped<ICreateEmployeeProfileHandler, CreateEmployeeProfileHandler>();
        services.AddScoped<IUpdateEmployeeProfileHandler, UpdateEmployeeProfileHandler>();
        services.AddScoped<IUpdateEmployeeCodeHandler, UpdateEmployeeCodeHandler>();
        services.AddScoped<IUpdateEmployeeRoleTitleHandler, UpdateEmployeeRoleTitleHandler>();
        services.AddScoped<IDeleteEmployeeProfileHandler, DeleteEmployeeProfileHandler>();
        services.AddScoped<IGetEmployeeProfileByIdHandler, GetEmployeeProfileByIdHandler>();
        services.AddScoped<IGetEmployeeProfileByIdentityIdHandler, GetEmployeeProfileByIdentityIdHandler>();
        services.AddScoped<IGetAllEmployeeProfilesHandler, GetAllEmployeeProfilesHandler>();
        services.AddScoped<IGetActiveEmployeeProfilesHandler, GetActiveEmployeeProfilesHandler>();

        // Ordering Services
        services.AddScoped<ICreateOrderHandler, CreateOrderHandler>();
        services.AddScoped<IUpdateOrderStatusHandler, UpdateOrderStatusHandler>();
        services.AddScoped<ICancelOrderHandler, CancelOrderHandler>();
        services.AddScoped<IGetAllOrdersHandler, OrderQueryHandlers>();
        services.AddScoped<IGetOrderByIdHandler, OrderQueryHandlers>();
        services.AddScoped<IGetOrdersByCustomerHandler, OrderQueryHandlers>();
        services.AddScoped<IGetOrdersByStatusHandler, OrderQueryHandlers>();

        // Coupon Services
        services.AddScoped<ICreateCouponHandler, CouponHandlers>();
        services.AddScoped<IUpdateCouponHandler, CouponHandlers>();
        services.AddScoped<ICancelCouponHandler, CouponHandlers>();
        services.AddScoped<IGetAllCouponsHandler, CouponQueryHandlers>();
        services.AddScoped<IGetCouponByIdHandler, CouponQueryHandlers>();
        services.AddScoped<IGetCouponByCodeHandler, CouponQueryHandlers>();
        services.AddScoped<IGetActiveCouponsHandler, CouponQueryHandlers>();
        
        return services;
    }
}