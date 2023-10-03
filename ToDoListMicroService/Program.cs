using Microsoft.Extensions.Options;
using ToDoListMicroService.Repository;
using ToDoListMicroService.Services;
using ToDoListMicroService.DataBaseConfig;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MassTransit;
using ToDoListMicroService.Consumer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddLog4Net();

// Add services to the container.
builder.Services.Configure<ToDoListDataBaseSettings>(builder.Configuration.GetSection(nameof(ToDoListDataBaseSettings)));

builder.Services.AddSingleton<IToDoListDataBaseSettings>(sp => sp.GetRequiredService<IOptions<ToDoListDataBaseSettings>>().Value);
builder.Services.AddScoped<IToDoListCommandRepository, ToDoListCommandRepository>();
builder.Services.AddScoped<IToDoListQueryRepository, ToDoListQueryRepository>();
builder.Services.AddScoped<IToDoListService, ToDoListService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddHttpContextAccessor();

var configSection = builder.Configuration.GetSection("ServiceBus");
var connectionUri = configSection.GetSection("ConnectionUri").Value;
var usename = configSection.GetSection("Username").Value;
var password = configSection.GetSection("Password").Value;
var queueName = configSection.GetSection("QueueName").Value;

//RabbitMQ - Mass Transit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(connectionUri), h =>
        {
            h.Username(usename);
            h.Password(password);
        });
        cfg.ReceiveEndpoint(queueName, ep =>
        {
            ep.ConfigureConsumer<UserConsumer>(context);
        });
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMediatR(typeof(Program).Assembly);
builder.Services.AddCors();
////builder.Services.AddCors(options =>
////{
////    options.AddDefaultPolicy(
////    builder =>
////    {
////        builder.WithOrigins("http://localhost:4200")
////                            .AllowAnyHeader()
////                            .AllowAnyMethod();
////    });
////});

//JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
        ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration.GetSection("AppSettings:Token").Value)),        
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = false
    };
});

//Swagger
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "TODO List API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = " ",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo Items API"));

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();