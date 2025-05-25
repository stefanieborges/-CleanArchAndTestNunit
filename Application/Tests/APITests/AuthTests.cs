using Application.Dtos;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net;
using System.Text;

namespace Application.Tests.APITests
{
    [TestFixture]
    public class AuthTests
    {
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7112")
            };
        }

        [Test]
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


        [Test]
        public async Task Login_ShouldReturn_JWTToken()
        {
            var payload = new
            {
                email = "testuser@example.com",
                senha = "StrongPassword123!"
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/user/login", content);
            var body = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"StatusCode: {response.StatusCode}");
            Console.WriteLine($"Body: {body}");

            var result = JsonConvert.DeserializeObject<LoginResponse>(body);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(result.Token, Is.Not.Null.And.Not.Empty);
            Assert.That(result.Token.Length, Is.GreaterThan(10));
        }

        [Test]
        public async Task Login_WithInvalidPassword_ShouldReturn_Unauthorized()
        {
            var payload = new
            {
                email = "testuser@example.com",
                senha = "SenhaErrada123!"
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/user/login", content);
            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"StatusCode: {response.StatusCode}");
            Console.WriteLine($"Body: {body}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task Register_ShouldReturn_Conflict_WhenEmailAlreadyExists()
        {
            var payload = new
            {
                nome = "Usuário Existente",
                email = "testuser@example.com", 
                senha = "StrongPassword123!",
                confimarSenha = "StrongPassword123!"
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/user/register", content);

            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"StatusCode: {response.StatusCode}");
            Console.WriteLine($"Body: {body}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        }
    }
}
