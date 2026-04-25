namespace GoodHamburger.Shared.Handlers;

public record ValidationError(Error[] Errors) 
    : Error("Validation.Error", "One or more validation failures occurred.");