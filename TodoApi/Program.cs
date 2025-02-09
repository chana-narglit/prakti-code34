using Microsoft.EntityFrameworkCore;
using TodoApi;
using ToDoDbContext = TodoApi.ToDoDbContext;
using Task = TodoApi.Task;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("mytodod");
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        Policy => Policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAllOrigins");

app.MapGet("/task", async (ToDoDbContext db) =>
{
    var a = await db.Tasks.ToListAsync();
    return Results.Ok(a);
});

app.MapPost("/task", async (ToDoDbContext db, Task newTask) =>
{
    await db.Tasks.AddAsync(newTask);
    await db.SaveChangesAsync();
    return Results.Created($"/task/{newTask.Id}", newTask);
});

app.MapPut("/task/{id}", async (ToDoDbContext db, int id, bool inputTask) =>
{
    var task = await db.Tasks.FindAsync(id);

    if (task is null) return Results.NotFound();

    task.IsComplete = !task.IsComplete;
    await db.SaveChangesAsync();
    return Results.Ok(task);
});

app.MapDelete("/task/{id}", async (ToDoDbContext db, int id) =>
{
    var task = await db.Tasks.FindAsync(id);

    if (task is null) return Results.NotFound();

    db.Tasks.Remove(task);
    await db.SaveChangesAsync();

    return Results.Ok();
});

app.MapGet("/", () => "TodoApi is running!*");

app.Run();
