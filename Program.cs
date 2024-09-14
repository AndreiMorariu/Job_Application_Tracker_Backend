using Application_Tracker.Contracts;
using Application_Tracker.Data;
using Application_Tracker.Models;
using Application_Tracker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(p =>
{
  p.AddPolicy("all", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    options.AddSecurityDefinition(
        "oauth2",
        new OpenApiSecurityScheme
        {
          In = ParameterLocation.Header,
          Name = "Authorization",
          Type = SecuritySchemeType.ApiKey,
        }
    )
);

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder
    .Services.AddIdentityApiEndpoints<User>(options =>
    {
      options.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connection"))
);

builder.Services.AddTransient<IApplicationService, ApplicationService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IEmailService, EmailService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
  app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("all");
app.UseAuthentication();
app.UseAuthorization();
app.MapGroup("/identity").MapIdentityApi<User>();
app.MapControllers();
app.Run();
