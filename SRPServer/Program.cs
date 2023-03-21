using Microsoft.EntityFrameworkCore;
using SRPServer.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Adding DB context
string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SRPContext>(options => options.UseSqlite(connection));

builder.Services.AddControllers();

var app = builder.Build();

/*
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
*/


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
