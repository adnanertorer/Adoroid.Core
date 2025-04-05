# Adoroid.Core

**Adoroid.Core**, .NET projelerinde sıkça ihtiyaç duyulan temel altyapı bileşenlerini sağlayan modüler bir çekirdek kütüphanedir. Projelerine kolayca entegre edilebilir ve geliştirici deneyimini iyileştirir.

---

## ✨ Özellikler

- **✅ Validationlar**: FluentValidation ile entegre, kolay doğrulama mekanizmaları.
- **🚨 Exception Handlerlar**: Merkezi hata yönetimi için middleware altyapısı.
- **🧩 Pipeline'lar**: MediatR pipeline behavior desteği ile genişletilebilir mimari.
- **📥 Request / 📤 Response Modelleri**: CQRS desenine uygun nesneler.
- **🧠 Generic Repository**: Entity bazlı genel CRUD işlemleri.
- **🔍 Dynamic LINQ**: Dinamik sorgular oluşturmak için yardımcı extension'lar.
- **📄 Paging**: Sayfalama işlemleri için hazır metodlar.

---

## 🚀 Kurulum

NuGet üzerinden yüklemek için:

```bash
dotnet add package Adoroid.Core.Repository
dotnet add package Adoroid.Core.Application

⚙️ Kullanım Örnekleri
1. Pipeline, Validation ve Business Rule Setup'ı

using FluentValidation;
using Adoroid.Core.Application.Pipelines.Validation;
using Adoroid.Core.Application.Rules;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

public static class ExampleServiceApplicationServiceCollection
{
    public static IServiceCollection AddExampleServiceApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddSubClassesOfType(Assembly.GetExecutingAssembly(), type: typeof(BaseBusinessRule));
       
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
}

2. Generic Repository Kullanımı

Entity Tanımı
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


🤝 Katkıda Bulunma
Katkılarınızı memnuniyetle karşılıyoruz! Lütfen bir issue açarak öneride bulunabilir veya doğrudan pull request göndererek katkı sağlayabilirsiniz.


