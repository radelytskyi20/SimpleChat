using Microsoft.EntityFrameworkCore;
using SimpleChat.Library.Constants;
using SimpleChat.Library.Data;
using SimpleChat.Library.Hubs;
using SimpleChat.Library.Interfaces;
using SimpleChat.Library.Models;
using SimpleChat.Library.Repos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString(ConnectionNames.DefaultConnection));
});

builder.Services.AddTransient<IRepo<User>, UsersRepo>();
builder.Services.AddTransient<IRepo<Chat>, ChatsRepo>();
builder.Services.AddTransient<IRepo<Message>, MessagesRepo>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chat");

app.Run();
