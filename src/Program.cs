using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PizzaStore.Models; 
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<PizzaDb>(options => options.UseInMemoryDatabase("items"));
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Todo Api",
            Description = "testando 123",
            Version = "v1"
        });
    });
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
    });
}


app.MapGet("/", () => "Hello World!");

app.MapGet("/pizza/{id}", async (PizzaDb db, int id)=> await db.Pizzas.FindAsync(id)); 
app.MapPost("/pizzas", async (Pizza pizza, PizzaDb db) =>{
    await  db.Pizzas.AddAsync(pizza); 
    await db.SaveChangesAsync(); 
    return Results.Created($"/pizza/{pizza.Id}", pizza); 
});

app.MapPut("/pizza/{id}", async (PizzaDb db, Pizza update, int id )=>{
    var pizza  = await db.Pizzas.FindAsync(); 
    if(pizza is null) return Results.NotFound(); 
    pizza.Name = update.Name; 
    pizza.Description = update.Description; 
    await db.SaveChangesAsync(); 
    return Results.NoContent(); 
}); 

app.MapDelete("/pizza/{id}", async (PizzaDb db, int id)=>{
    var pizza =  await db.Pizzas.FindAsync(id); 
    if(pizza is null) return Results.NotFound(); 
    db.Pizzas.Remove(pizza); 
    await db.SaveChangesAsync(); 
    return Results.Ok(); 

}); 

app.Run(); 