using Gimnasio.Extensions;
using Gimnasio.Middleware;
using Gimnasio.Data;
using Gimnasio.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GymDbContext>();
    await db.Database.MigrateAsync();
}

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gym API v1");
        c.RoutePrefix = string.Empty;
    });

    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

    if (!context.Users.Any())
    {
        context.Roles.AddRange(
            new Role { Name = "ADMIN", NormalizedName = "ADMIN" },
            new Role { Name = "ENTRENADOR", NormalizedName = "ENTRENADOR" },
            new Role { Name = "SOCIO", NormalizedName = "SOCIO" }
        );

        await context.SaveChangesAsync();
        
        var adminRole = context.Roles.First(r => r.Name == "ADMIN");

        var admin = new User
        {
            UserName           = "JosephAlexanderYN2000",
            NormalizedUserName = "JOSEPHALEXANDERYN2000",
            Email              = "admin@gym.local",
            NormalizedEmail    = "ADMIN@GYM.LOCAL",
            PasswordHash       = BCrypt.Net.BCrypt.HashPassword("Joseph18"),
            PhoneNumber        = "+51 123456789",
            CreatedAt          = DateTime.UtcNow
        };
        admin.UserRoles.Add(new UserRole { User = admin, Role = adminRole });
        context.Users.Add(admin);
        await context.SaveChangesAsync();
    }
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
