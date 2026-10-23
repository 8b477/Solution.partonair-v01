using Domain.partonair_v01.Enums;
using System.ComponentModel.DataAnnotations;

namespace SharedModels.partonair_v01.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public sealed class AllowedRolesAttribute : ValidationAttribute
    {
        private readonly Roles[] _allowedRoles;

        public AllowedRolesAttribute(params Roles[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not Roles role)
            {
                return new ValidationResult("Invalid role.");
            }

            if (!_allowedRoles.Contains(role))
            {
                return new ValidationResult(
                    ErrorMessage ?? $"The role '{role}' is not allowed."
                );
            }

            return ValidationResult.Success;
        }
    }
}