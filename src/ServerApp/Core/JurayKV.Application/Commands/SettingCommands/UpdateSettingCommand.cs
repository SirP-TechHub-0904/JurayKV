using JurayKV.Application.Caching.Handlers;
using JurayKV.Application.Infrastructures;
using JurayKV.Application.Queries.SettingQueries;
using JurayKV.Domain.Aggregates.SettingAggregate;
using JurayKV.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using TanvirArjel.ArgumentChecker;

namespace JurayKV.Application.Commands.SettingCommands;

public sealed class UpdateSettingCommand : IRequest
{
    public UpdateSettingCommand(SettingDetailsDto setting)
    {
        Setting = setting;
         
    }

    public SettingDetailsDto Setting { get; }
     
}

internal class UpdateSettingCommandHandler : IRequestHandler<UpdateSettingCommand>
{
    private readonly ISettingRepository _settingRepository;
    private readonly ISettingCacheHandler _settingCacheHandler;
    private readonly IStorageService _storage;

    public UpdateSettingCommandHandler(
        ISettingRepository settingRepository,
        ISettingCacheHandler settingCacheHandler,
        IStorageService storage)
    {
        _settingRepository = settingRepository;
        _settingCacheHandler = settingCacheHandler;
        _storage = storage;
    }

    public async Task Handle(UpdateSettingCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));
        var getupdate = await _settingRepository.GetByIdAsync(request.Setting.Id);
        if (getupdate != null)
        {
             

            getupdate.MinimumAmountBudget = request.Setting.MinimumAmountBudget;
            getupdate.DefaultAmountPerView = request.Setting.DefaultAmountPerView;
             

            await _settingRepository.UpdateAsync(getupdate);

            await _settingCacheHandler.RemoveListAsync();
            await _settingCacheHandler.RemoveGetByIdAsync(getupdate.Id);
        }
    }
}
