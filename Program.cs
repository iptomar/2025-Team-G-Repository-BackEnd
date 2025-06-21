using BackEndHorario;
using BackEndHorario.Controllers;
using BackEndHorario.Data;
using BackEndHorario.Hubs;
using BackEndHorario.Services;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", builder => {
        builder.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddSignalR();

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    SeedData.Inicializar(services);
}


using (var scope = app.Services.CreateScope()) {
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var importador = new ImportadorExcelService(context);
    var caminho = Path.Combine(Directory.GetCurrentDirectory(), "ficheiros", "Gestão de Projetos.xlsx");

    // 1. Primeiro importa os dados de base
    await importador.ImportarCursosAsync(caminho);
    await importador.ImportarDocentesAsync(caminho);  // << ? antes das UCs
    await importador.ImportarEscolasAsync(caminho);
    await importador.ImportarSalasAsync(caminho);
    await importador.ImportarTurmasAsync(caminho);
    await importador.ImportarHorariosAsync(caminho); // se aplic vel

    // 2. Depois importa as UCs (agora com nomes reais)
    await importador.ImportarUCsAsync(caminho);

    // 3. S  por fim gera blocos
    var gerador = new GeradorBlocosService(context);
    await gerador.LimparBlocosAsync();

    // 4. Gera blocos padrao
    await gerador.GerarBlocosPadraoAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseMigrationsEndPoint();
}
else {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.MapHub<HorarioHub>("/horarioHub"); // <-- Registo da rota do hub

app.Run();
