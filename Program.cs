using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using ApiNotificacoesPush.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//  Adiciona o DbContext com SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new Exception("Connection string do SQL Server não configurada");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

//  Serviços da API
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

//  Firebase
var firebaseJson = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_JSON")
                   ?? throw new Exception("Configure FIREBASE_CREDENTIALS_JSON no Render");

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromJson(firebaseJson)
});

var app = builder.Build();

// Aqui aplicamos as migrations pendentes para criar/atualizar o banco automaticamente
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

//  Middlewares
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();


/*using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

// Configuração mínima necessária
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Firebase
var firebaseJson = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_JSON")
                   ?? throw new Exception("Configure FIREBASE_CREDENTIALS_JSON no Render");

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromJson(firebaseJson)
});

var app = builder.Build();

// Swagger sempre ativo
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();*/