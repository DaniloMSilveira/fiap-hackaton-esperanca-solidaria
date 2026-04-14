using FluentValidation;

namespace EsperancaSolidaria.Application.DTO.Inputs
{
    public class CriarUsuarioInput : BaseInput<CriarUsuarioInput>
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }
        public string Senha { get; set; }
        public string ConfirmacaoSenha { get; set; }

        protected override IValidator<CriarUsuarioInput> GetValidator()
        {
            return new CriarUsuarioInputValidator();
        }
    }

    public class CriarUsuarioInputValidator : AbstractValidator<CriarUsuarioInput>
    {
        public CriarUsuarioInputValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MinimumLength(2).WithMessage("Nome deve ter no mínimo 3 caracteres");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email inválido");

            RuleFor(x => x.Cpf)
                .NotEmpty().WithMessage("CPF é obrigatório")
                .Matches(@"^\d{11}$|^\d{3}\.\d{3}\.\d{3}-\d{2}$").WithMessage("CPF deve conter 11 dígitos");

            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage("Senha é obrigatória")
                .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres");

            RuleFor(x => x.ConfirmacaoSenha)
                .Equal(x => x.Senha).WithMessage("Senhas não conferem");
        }
    }
}