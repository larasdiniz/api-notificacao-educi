using FirebaseAdmin;
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
app.Run();