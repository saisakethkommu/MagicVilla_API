using MagicVilla_VillaAPI.Interfaces;
using MagicVilla_VillaAPI.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args); // logger is registered inside the application with this createbuilder function and can be utilized in dependency injection whereever we want to use logging

// Add services to the container.

// here we register the custom logger service as dependency injection
/*Log.Logger = new LoggerConfiguration()
 * aconfiguration levels
 * .MinimumLevel.Debug()
 * write location along with write location refresh rate and all properties
 * .WriteTo.File("/log/villaLogs.txt", rollingInterval: RollingInterval.Day)
 * create logger
 * .CreateLogger();*/

/*builder.Host.UseSerilog();*/

builder.Services.AddControllers(
    option =>
    { //option.ReturnHttpNotAcceptable = true }
    }).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();// option allow you to make sure if parameters of request are not appropriate   throw an error and only set them to application json. adding options is called as content negotiation
// adding AddXmlDataContractSerializerFormatters allow you to support Xml formatting when sending a request
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ILogging, Logging>(); // register the service in the container
// there are multiple lifetimes when registering a service longest is singleton cretad when the application starts and that object is used everytime an application requests for its implementation, scoped is for eveery request it will create a new object and provider it, transient does it on every time the object is object example if in a request an object is used 10 times it gets created 10 times

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// loggin plays a 