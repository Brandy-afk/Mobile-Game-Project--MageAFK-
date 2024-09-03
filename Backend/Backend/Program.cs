using Backend.Data;
using Backend.Domain.Inputs.Types;
using Backend.Domain.Types.Many;
using Backend.Domain.Types.One;
using Backend.Mutations;
using Backend.Queries;
using Backend.Repositories;
using Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<GameDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseConnectionString"));
});

#region Scoped

builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IMilestoneRepository, MilestoneRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<ISpellStatisticRepository, SpellStatisticRepository>();
builder.Services.AddScoped<IStatisticRepository, StatisticRepository>();
builder.Services.AddScoped<ITypeRepository, TypeRepository>();
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();


#endregion

builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    // Many
    .AddType<CurrencyObjectType>()
    .AddType<ElixirObjectType>()
    .AddType<GameRunObjectType>()
    .AddType<HistoryObjectType>()
    .AddType<LocationObjectType>()
    .AddType<MilestoneObjectType>()
    .AddType<PlayerObjectType>()
    .AddType<RecipeObjectType>()
    .AddType<RecipeShopObjectType>()
    .AddType<SkillObjectType>()
    .AddType<SpellStatisticObjectType>()
    .AddType<StatisticObjectType>()
    .AddType<LeaderBoardObjectType>()
    // One
    .AddType<CurrencyTypeObjectType>()
    .AddType<LocationTypeObjectType>()
    .AddType<MilestoneTypeObjectType>()
    .AddType<RecipeTypeObjectType>()
    .AddType<SkillTypeObjectType>()
    .AddType<SpellStatisticTypeObjectType>()
    .AddType<StatisticTypeObjectType>()
    // Input
    .AddType<AbstractTypeInputType>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGraphQL();

app.Run();
