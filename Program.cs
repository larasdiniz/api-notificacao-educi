using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Firebase - Modo Render (só variável de ambiente)
var firebaseJson = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_JSON")
                   ?? throw new Exception("Configure FIREBASE_CREDENTIALS_JSON no Render");

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromJson(firebaseJson)
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();