# Usar imagem oficial do SDK .NET 8 para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copiar csproj e restaurar dependências
COPY *.csproj ./
RUN dotnet restore

# Copiar todo o restante do código e publicar
COPY . ./
RUN dotnet publish -c Release -o out

# Imagem runtime para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app
COPY --from=build /app/out ./

# Expõe a porta padrão 80
EXPOSE 80

# Comando para rodar a API
ENTRYPOINT ["dotnet", "ApiNotificacoesPush.dll"]
