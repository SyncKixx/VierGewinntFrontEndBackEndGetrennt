using VierGewinntApi;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<BackEndService>();
builder.Services.AddTransient<CounterService>();
// Add services to the container.
builder.Services.AddControllers();
//policys die erlauben vin welchen sachen anfragen geschickt werden dürfen
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
            //policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
        });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
// Beispiel für code performance
//
// Das Sieb des Eratostenes
// 
//
//