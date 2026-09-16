using System.ComponentModel.DataAnnotations;

public static class ValidationRouteExtensions
{
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder) where T : class
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            // Извлекаем аргумент нужного типа из контекста запроса
            if (context.Arguments.FirstOrDefault(x => x is T) is T argument)
            {
                var validationContext = new ValidationContext(argument);
                var validationResults = new List<ValidationResult>();

                // Встроенная валидация .NET
                bool isValid = Validator.TryValidateObject(argument, validationContext, validationResults, validateAllProperties: true);

                if (!isValid)
                {
                    // Группируем ошибки по именам полей
                    var errors = validationResults
                        .GroupBy(e => e.MemberNames.FirstOrDefault() ?? "model")
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage ?? "Validation Error").ToArray()
                        );

                    return Results.ValidationProblem(errors);
                }
            }

            return await next(context);
        });
    }
}
