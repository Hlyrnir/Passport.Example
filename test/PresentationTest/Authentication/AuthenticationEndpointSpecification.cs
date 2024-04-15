using FluentAssertions;
using Presentation.Endpoint;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace PresentationTest.Authentication
{
    public class AuthenticationEndpointSpecification
    {
        private readonly HttpClient httpClient = new HttpClient()
        {
            BaseAddress = new Uri("https://localhost:7247")
        };

        //[Fact]
        public async Task GenerateTokenByCredential_should_return_status_code_OK_with_valid_credential()
        {
            // Arrange
            using StringContent jsonContent = new(
                                    JsonSerializer.Serialize(new
                                    {
                                        Provider = "DEFAULT_PROVIDER",
                                        Credential = "DEFAULT_CREDENTIAL",
                                        Signature = "DEFAULT_SIGNATURE"
                                    }),
                                    Encoding.UTF8,
                                    "application/json");

            // Act
            HttpResponseMessage httpResponse = await httpClient.PostAsync(EndpointRoute.Authentication.Token, jsonContent);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        //[Fact]
        public async Task GenerateTokenByCredential_should_return_status_code_BadRequest_with_invalid_signature()
        {
            // Arrange
            using StringContent jsonContent = new(
                                    JsonSerializer.Serialize(new
                                    {
                                        Provider = "DEFAULT_PROVIDER",
                                        Credential = "DEFAULT_CREDENTIAL",
                                        Signature = "WRONG_SIGNATURE"
                                    }),
                                    Encoding.UTF8,
                                    "application/json");

            // Act
            HttpResponseMessage httpResponse = await httpClient.PostAsync(EndpointRoute.Authentication.Token, jsonContent);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        //[Fact]
        public async Task GenerateTokenByRefreshToken_should_return_status_code_BadRequest_with_invalid_refresh_token()
        {
            // Arrange
            using StringContent jsonContent = new(
                                    JsonSerializer.Serialize(new
                                    {
                                        PassportId = Guid.NewGuid().ToString(),
                                        Provider = "DEFAULT_JWT",
                                        RefreshToken = Guid.NewGuid().ToString()
                                    }),
                                    Encoding.UTF8,
                                    "application/json");

            // Act
            HttpResponseMessage httpResponse = await httpClient.PostAsync(EndpointRoute.Authentication.RefreshToken, jsonContent);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}