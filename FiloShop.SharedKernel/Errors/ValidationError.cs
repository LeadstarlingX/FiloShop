namespace FiloShop.SharedKernel.Errors;

public sealed record ValidationError(string PropertyName, string ErrorMessage);