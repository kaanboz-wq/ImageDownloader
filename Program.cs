using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/getimage", (String filePath = "") =>
{
    try
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return Results.BadRequest("filePath must be provided.");
        }
        FileInfo fileInfo = new FileInfo(app.Configuration.GetSection("ImagePath").Value + filePath);
        byte[] data = new byte[fileInfo.Length];

        using (FileStream fs = fileInfo.OpenRead())
        {
            fs.Read(data, 0, data.Length);
        }

        return Results.File(data, fileInfo.Extension switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            _ => "application/octet-stream"
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest("Not Found.");
    }
    

}).WithName("GetImage");

app.Run();
