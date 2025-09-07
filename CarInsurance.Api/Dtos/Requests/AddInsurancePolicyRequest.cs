using FluentValidation;

namespace CarInsurance.Api.Dtos.Requests;
public record AddInsurancePolicyRequest(string? Provider, DateOnly StartDate, DateOnly EndDate);

public class AddInsurancePolicyRequestValidator : AbstractValidator<AddInsurancePolicyRequest>
{
    public AddInsurancePolicyRequestValidator()
    {
        RuleFor(x => x.StartDate)
            .Must((req, startDate) => startDate < req.EndDate)
            .WithMessage("Start date must be before end date");
    }
}
