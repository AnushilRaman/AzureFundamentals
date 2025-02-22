using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using AzureServicesViews.Services;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
/*
 * Azure key vault to store secret keys or connection strings
 */
var keyVaultUri = builder.Configuration["AzureKeyVault:VaultUri"];
var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
KeyVaultSecret secret = await secretClient.GetSecretAsync(builder.Configuration["ConnectionStringName:ImageContainer"]);

builder.Services.AddSingleton(x => new BlobServiceClient(
    secret.Value
    ));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IContainerServices, ContainerServices>();
builder.Services.AddSingleton<IBlobService, BlobService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
