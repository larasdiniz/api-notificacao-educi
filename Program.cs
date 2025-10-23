using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// ✅ USE O ARQUIVO LOCAL - remova a variável de ambiente
var firebaseJson = File.ReadAllText("firebase-service-account.json");

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