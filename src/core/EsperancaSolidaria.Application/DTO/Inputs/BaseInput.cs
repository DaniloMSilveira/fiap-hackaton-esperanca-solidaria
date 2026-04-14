using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.Results;

namespace EsperancaSolidaria.Application.DTO.Inputs
{
    public abstract class BaseInput<TInput> where TInput : BaseInput<TInput>
    {
        private ValidationResult _validationResult;
        [JsonIgnore]
        public ValidationResult ValidationResult => _validationResult;

        public bool IsValid()
        {
            var validator = GetValidator();
            if (validator == null)
                throw new InvalidOperationException("Validator not provided.");

            _validationResult = validator.Validate((TInput)this);
            return _validationResult.IsValid;
        }

        protected abstract IValidator<TInput> GetValidator();
    }
}