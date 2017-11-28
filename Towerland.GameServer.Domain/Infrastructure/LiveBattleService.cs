using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using AutoMapper;
using GameServer.Common.Models.GameField;
using GameServer.Common.Models.GameObjects;
using GameServer.Common.Models.State;
using Towerland.GameServer.Common.Logic;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Core.DataAccess;
using Towerland.GameServer.Core.Entities;
using Towerland.GameServer.Domain.Helpers;
using Towerland.GameServer.Domain.Interfaces;

namespace Towerland.GameServer.Domain.Infrastructure
{
  public class LiveBattleService : ILiveBattleService, IBattleProvider
  {
    private readonly ConcurrentDictionary<Guid, int> _battles;

    private readonly ICrudRepository<LiveBattle> _repository;
    private readonly IStateChangeRecalculator _recalculator;
    private readonly IFieldFactory _fieldFactory;
    private readonly IMapper _mapper;

    public LiveBattleService(ICrudRepository<LiveBattle> repo, IStateChangeRecalculator recalc, IFieldFactory fieldFactory, IMapper mapper)
    {
      _repository = repo;
      _recalculator = recalc;
      _battles = new ConcurrentDictionary<Guid, int>();
      _fieldFactory = fieldFactory;
      _mapper = mapper;
    }

    public bool CheckChanged(Guid battleId, int version)
    {
      if (!_battles.ContainsKey(battleId))
      {
        throw new ArgumentException("No such battle");
      }
      return _battles[battleId] != version;
    }

    public Field GetFieldState(Guid battleId)
    {
      return _repository.Get(battleId).SerializedState.FromJsonString<Field>();
    }

    public Guid InitNewBattle()
    {
      var id = Guid.NewGuid();
      while (!_battles.TryAdd(id, 0)) ;
      CreateBattleAsync(id);
      return id;
    }

    public async Task RecalculateAsync(StateChangeCommand command, FieldState fieldState)
    {
      await Task.Run(() =>
      {
        var fieldSerialized = _repository.Get(command.BattleId);
        var field = fieldSerialized.SerializedState.FromJsonString<Field>();
        
        field.SetState(fieldState);
        
        if (command.Id.HasFlag(CommandId.AddUnit))
        {
          foreach (var opt in command.UnitCreationOptions)
          {
            _recalculator.AddNewUnit(field, opt.Type, _mapper.Map<CreationOptions>(opt));
          }
        }
        if (command.Id.HasFlag(CommandId.AddTower))
        {
          foreach (var opt in command.TowerCreationOptions)
          {
            _recalculator.AddNewTower(field, opt.Type, _mapper.Map<CreationOptions>(opt));
          }
        }
        IncrementBattleVersion(command.BattleId);
        
        fieldSerialized.SerializedState = field.ToJsonString();
        _repository.Update(fieldSerialized);
        _repository.SaveStateAsync();
      });
    }


    private bool IncrementBattleVersion(Guid battleId)
    {
      int curValue;
      while(_battles.TryGetValue(battleId, out curValue))
      {
        if(_battles.TryUpdate(battleId, curValue + 1, curValue))
          return true;
      }
      return false;
    }

    private async void CreateBattleAsync(Guid battleId)
    {
      await Task.Run(() =>
      {
        var newBattle = new LiveBattle
        {
          Id = battleId,
          SerializedState = _fieldFactory.ClassicField.ToJsonString()
        };
        _repository.Create(newBattle);
        _repository.SaveStateAsync();
      });
    }
  }
}
