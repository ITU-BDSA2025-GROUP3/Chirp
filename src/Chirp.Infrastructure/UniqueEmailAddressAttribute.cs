using System.ComponentModel.DataAnnotations;

using Chirp.Core.DomainModel;
using Chirp.Core.ServiceInterfaces;
using Chirp.Infrastructure.Services;

namespace Chirp.Infrastructure;


/// <summary>
/// Heavily inspired by https://stackoverflow.com/questions/71043332/how-and-where-to-validate-uniqueness-of-attribute-property-in-asp-net-core-mvc-u
/// </summary>
public class UniqueEmailAddressAttribute :ValidationAttribute
{
    
    protected override ValidationResult? IsValid(
        object? value, ValidationContext validationContext)
    {
        var _authorService = validationContext.GetService(typeof(IAuthorService))! as IAuthorService;
        
        var exists = _authorService.AuthorExists((value!).ToString()!).Result;
        if (exists)
        {
            return new ValidationResult("Email is already registered!");
        }
        return ValidationResult.Success;
    }
}