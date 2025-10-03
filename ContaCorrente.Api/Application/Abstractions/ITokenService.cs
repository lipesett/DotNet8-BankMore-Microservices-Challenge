using ContaCorrenteEntity = ContaCorrente.Api.Domain.ContaCorrente;

namespace ContaCorrente.Api.Application.Abstractions
{
    public interface ITokenService
    {
        string CreateToken(ContaCorrenteEntity user);
    }
}