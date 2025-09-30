using Microsoft.EntityFrameworkCore;
using PatientReg.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers & Swagger
builder.Services.AddControllers().AddJsonOptions(o =>
{
    // Serialize DateOnly as yyyy-MM-dd
    o.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS (allow local frontend)
var corsPolicy = "_allowLocal";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicy, policy =>
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins(
            "http://localhost:5500", // if using Live Server or simple static server
            "http://127.0.0.1:5500",
            "http://localhost:5173",
            "http://localhost:3000"
        )
    );
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(corsPolicy);

app.MapControllers();

app.Run();

/// <summary> System.Text.Json converter for DateOnly </summary>
public sealed class DateOnlyJsonConverter : System.Text.Json.Serialization.JsonConverter<DateOnly>
{
    private const string Format = "yyyy-MM-dd";
    public override DateOnly Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        => DateOnly.Parse(reader.GetString()!);
    public override void Write(System.Text.Json.Utf8JsonWriter writer, DateOnly value, System.Text.Json.JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(Format));
}
