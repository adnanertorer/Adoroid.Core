# Adoroid.Core

**Adoroid.Core** is a modular .NET Core library that brings together essential infrastructure components commonly used across backend projects. It's designed to be reusable, extendable, and easy to integrate into your applications.

---

## ✨ Features

- Request validation using [FluentValidation](https://fluentvalidation.net/)
- MinimalMediatR pipeline behaviors
- Centralized exception handling middleware
- Generic repository pattern
- Dynamic LINQ query helpers
- Built-in pagination support
- Response Pattern support

---

## 🚀 Installation

You can install the packages via NuGet:

```bash
dotnet add package Adoroid.Core.Repository
dotnet add package Adoroid.Core.Application

⚙️ Usage Examples
1. Registering Pipeline Behaviors and Validations

using FluentValidation;
using Adoroid.Core.Application.Pipelines.Validation;
using Adoroid.Core.Application.Rules;
using MinimalMediatR.Behaviors;
using MinimalMediatR.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

public static class ExampleServiceApplicationServiceCollection
{
    public static IServiceCollection AddExampleServiceApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddSubClassesOfType(Assembly.GetExecutingAssembly(), type: typeof(BaseBusinessRule));
        services.AddMinimalMediatR(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }

     public static IServiceCollection AddSubClassesOfType(this IServiceCollection services, Assembly assembly, Type type,
        Func<IServiceCollection, Type, IServiceCollection>? addWithLifeCycle = null)
        {
            var types = assembly.GetTypes().Where(t => t.IsSubclassOf(type) && type != t).ToList();
            foreach (var item in types)
                if (addWithLifeCycle == null)
                    services.AddScoped(item);
                else
                    addWithLifeCycle(services, type);
            return services;
        }
}

2. Generic Repository Usage

Entity

public class User : Entity<long>
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public bool IsActive { get; set; }
    public string? OtpCode { get; set; }
}

Repository Interface

public interface IUserRepository : IAsyncRepository<User, int>
{
}

Impl.
internal class UserRepository : RepositoryBase<User, int, ExampleDbContext>, IUserRepository
{
    public UserRepository(ExampleDbContext context) : base(context) { }
}

3. Reponse Pattern Usage

return Response<CompanyModel>.Fail(BusinessMessages.CompanyIsAlreadyExists);
or
return Response<CompanyModel>.Success(resultEntity.ToModel());


🤝 Contributing
Contributions are welcome! Feel free to open an issue or submit a pull request.


