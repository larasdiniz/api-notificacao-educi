using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do Firebase (API v1) - Modificação para o Render
var firebaseCredentialsJson = builder.Configuration["FIREBASE_CREDENTIALS_JSON"]
                             ?? Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_JSON");

if (string.IsNullOrEmpty(firebaseCredentialsJson))
{
    throw new Exception("Variável FIREBASE_CREDENTIALS_JSON não encontrada.");
}

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromJson(firebaseCredentialsJson)
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();