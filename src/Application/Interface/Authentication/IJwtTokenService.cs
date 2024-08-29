using Application.Token;
using Domain.Interface.Authorization;

namespace Application.Interface.Authentication
{
    internal interface IJwtTokenService
    {
        JwtTokenTransferObject Generate(IPassport ppPassport, IPassportToken ppToken);
    }
}