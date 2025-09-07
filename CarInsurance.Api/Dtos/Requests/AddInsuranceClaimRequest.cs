using FluentValidation;

namespace CarInsurance.Api.Dtos.Requests;
public record AddInsuranceClaimRequest(DateOnly ClaimDate, string Description, decimal Amount);

public class AddInsuranceClaimRequestValidator : AbstractValidator<AddInsuranceClaimRequest>
{
    public AddInsuranceClaimRequestValidator()
    {
        RuleFor(x => x.ClaimDate)
            .Must(d => d.ToDateTime(TimeOnly.MinValue) < DateTime.UtcNow)
            .WithMessage("Claim date must be in the past");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must have at most 500 characters");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0");
    }
}
