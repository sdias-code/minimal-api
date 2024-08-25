var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Olá pessoal!");
app.MapPost("/login", (LoginDTO loginDTO) => {
    if(loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "123456")
        return Results.Ok("Login realizado com sucesso");    
    else
        return Results.Unauthorized();
});

app.Run();

public class LoginDTO{
    public string Email { get; set; } = default!;
    public string Senha { get; set; } = default!;
}