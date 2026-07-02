using RinneganCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddRinnegan(Assembly.GetAssembly(typeof(HelloRinnegan.Services.Rinnegan<,>)));

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
