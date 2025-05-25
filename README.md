# Clean Architecture e Testes com Nunit
O presente projeto retrata um estudo de caso que aborda os conceitos de arquitetura limpa e seus testes em uma api de login de usuários.
Além disso foi utilizado o entity framework para conexão com o banco de dados além da validação de login usando JWT.
## Clean Architecture - Utilizando o padrão Onion
A estrutura das pastas do projeto ficaram da seguinte forma 
<img src=""/>
## Testes com o Nunit
Foram feitos 4 teste nesse presente projeto:
### 1º - Teste de registro de usuário retornando status 200 - OK

```csharp
public async Task Register_ShouldReturn_Created()
{
    var payload = new
    {
        nome = "Usuário de Teste",
        email = $"novo{Guid.NewGuid()}@example.com",
        senha = "StrongPassword123!",
        confimarSenha = "StrongPassword123!"
    };

    var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

    try
    {
        var response = await _client.PostAsync("/api/user/register", content);

        Console.WriteLine($"StatusCode: {response.StatusCode}");
        var body = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Body: {body}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"HttpRequestException: {ex.Message}");
        throw;
    }
}
```

### Exemplo de saída do teste

```
StatusCode: OK
Body: {"flag":true,"message":"Login realizado com sucesso!","token":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjMiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiVXN1w6FyaW8gZGUgVGVzdGUiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJ0ZXN0dXNlckBleGFtcGxlLmNvbSIsImV4cCI6MTc0ODY0Mzc2OSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzExMiIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjcxMTIifQ.HNCI1e3w2D03vzk_d7d_8WtD2SmAzfkZTslyvFRPuDA"}
```

2º  - Teste de login do usuário retornando o token JWT e também retornado status 200 - OK
<img src=""/>
3º - Teste de erro de senha no login do usuário devendo retornar status 401 - Unauthorized
<img src=""/>
4º - Teste tentando cadastrar um email já existente no banco de dados devendo retornar status 409 - Conflict
<img src=""/>
