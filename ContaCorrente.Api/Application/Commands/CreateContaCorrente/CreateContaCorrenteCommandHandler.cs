using ContaCorrente.Api.Domain;
using MediatR;
using ContaCorrenteEntity = ContaCorrente.Api.Domain.ContaCorrente;

namespace ContaCorrente.Api.Application.Commands.CreateContaCorrente
{
    public class CreateContaCorrenteCommandHandler : IRequestHandler<CreateContaCorrenteCommand, int>
    {
        public async Task<int> Handle(CreateContaCorrenteCommand request, CancellationToken cancellationToken)
        {
            // 1. Validar o CPF
            if (!IsValidCpf(request.Cpf))
            {
                throw new ArgumentException("CPF inválido");
            }

            // 2. Criar a nova entidade de Conta Corrente
            var novaConta = new ContaCorrenteEntity
            {
                Numero = new Random().Next(10000, 99999),
                Nome = "Nome do Titular",
                Ativo = true,
                Senha = request.Senha
            };

            // 3. Persistir na tabela CONTACORRENTE com Dapper
            // ... Lógica do Dapper virá aqui ...

            // 4. Retornar o número da conta
            return novaConta.Numero;
        }

        // Função simples de validação de CPF (apenas para exemplo)
        private bool IsValidCpf(string cpf)
        {
            // Lógica de validação de CPF viria aqui.
            // Por enquanto, apenas checa o comprimento.
            return !string.IsNullOrWhiteSpace(cpf) && cpf.Length == 11;
        }
    }
}