using JurayKV.Application.Caching.Handlers;
using JurayKV.Domain.Aggregates.CompanyAggregate;
using JurayKV.Domain.ValueObjects;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace JurayKV.Application.Commands.CompanyCommands;

public sealed class CreateCompanyCommand : IRequest<Guid>
{
    public CreateCompanyCommand(string name, Guid userId)
    {
        Name = name; 
        UserId = userId;
    }

    public string Name { get; }
    public Guid UserId { get;}
     
}

internal class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Guid>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ICompanyCacheHandler _companyCacheHandler;

    public CreateCompanyCommandHandler(
            ICompanyRepository companyRepository,
            ICompanyCacheHandler companyCacheHandler)
    {
        _companyRepository = companyRepository;
        _companyCacheHandler = companyCacheHandler;
    }

    public async Task<Guid> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        _ = request.ThrowIfNull(nameof(request));

      
        Company company = new Company(Guid.NewGuid());
        company.Name = request.Name;
        company.UserId = request.UserId;
        // Persist to the database
        await _companyRepository.InsertAsync(company);

        // Remove the cache
        await _companyCacheHandler.RemoveListAsync();
        await _companyCacheHandler.RemoveDropdownListAsync();
        await _companyCacheHandler.RemoveGetAsync(company.Id);
        await _companyCacheHandler.RemoveDetailsByIdAsync(company.Id);
        return company.Id;
    }
}