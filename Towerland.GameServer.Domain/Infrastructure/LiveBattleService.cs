﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GameServer.Common.Models.GameActions;
using GameServer.Common.Models.GameField;
using GameServer.Common.Models.GameObjects;
using GameServer.Common.Models.State;
using Towerland.GameServer.Common.Logic.ActionResolver;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Core.DataAccess;
using Towerland.GameServer.Core.Entities;
using Towerland.GameServer.Domain.Interfaces;
using Towerland.GameServer.Domain.Models;

namespace Towerland.GameServer.Domain.Infrastructure
{
  public class LiveBattleService : ILiveBattleService, IBattleService
  {
    private readonly ConcurrentDictionary<Guid, int> _battles;

    private readonly ICrudRepository<Battle> _battleRepository;
    private readonly IProvider<LiveBattleModel> _provider;
    private readonly IStateChangeRecalculator _recalculator;
    private readonly IFieldFactory _fieldFactory;
    private readonly IMapper _mapper;

    public LiveBattleService(ICrudRepository<Battle> repo,
      IProvider<LiveBattleModel> provider, 
      IStateChangeRecalculator recalc, 
      IFieldFactory fieldFactory, 
      IMapper mapper)
    {
      _battleRepository = repo;
      _provider = provider;
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
      return _provider.Get(battleId).State;
    }

    public IEnumerable<GameTick> GetCalculatedActionsByTicks(Guid battleId)
    {
      return _provider.Get(battleId).Ticks;
    }

    public Guid InitNewBattle()
    {
      var id = Guid.NewGuid();
      while (!_battles.TryAdd(id, 0)) ;
      CreateBattleAsync(id);
      return id;
    }

    public async Task RecalculateAsync(StateChangeCommand command, int curTick)
    {
      await Task.Run(() =>
      {
        var fieldSerialized = _provider.Get(command.BattleId);
        var fieldState = fieldSerialized.State;
        var ticks = fieldSerialized.Ticks.Take(curTick);
        
        ResolveActions(fieldState, ticks);
    
        if (command.Id.HasFlag(CommandId.AddUnit))
        {
          foreach (var opt in command.UnitCreationOptions)
          {
            _recalculator.AddNewUnit(fieldState, opt.Type, _mapper.Map<CreationOptions>(opt));
          }
        }
        if (command.Id.HasFlag(CommandId.AddTower))
        {
          foreach (var opt in command.TowerCreationOptions)
          {
            _recalculator.AddNewTower(fieldState, opt.Type, _mapper.Map<CreationOptions>(opt));
          }
        }
        IncrementBattleVersionAsync(command.BattleId);
        
        fieldSerialized.State = fieldState;
        _provider.Update(fieldSerialized.Id, fieldSerialized);
      });
    }


    private async Task<bool> IncrementBattleVersionAsync(Guid battleId)
    {
      return await Task.Run(() =>
      {
        while (_battles.TryGetValue(battleId, out var curValue))
        {
          if (_battles.TryUpdate(battleId, curValue + 1, curValue))
            return true;
        }
        return false;
      });
    }

    private async void CreateBattleAsync(Guid battleId)
    {
      await Task.Run(() =>
      {
        var newBattle = new LiveBattleModel
        {
          Id = battleId,
          State = _fieldFactory.ClassicField,
          Ticks = Enumerable.Empty<GameTick>()
        };
        _provider.Add(newBattle);
      });
    }

    private static void ResolveActions(Field f, IEnumerable<GameTick> ticks)
    {
      if (ticks == null)
      {
        return;
      }
      var resolver = new FieldStateActionResolver(f);
      foreach (var tick in ticks)
      {
        if (tick.HasNoActions)
        {
          continue;
        }
        foreach (var action in tick.Actions)
        {
          resolver.Resolve(action);
        }
      }
    }
  }
}
