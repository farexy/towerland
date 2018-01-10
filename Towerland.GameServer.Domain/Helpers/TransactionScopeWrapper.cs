using System;
using System.Transactions;

namespace Towerland.GameServer.Domain.Helpers
{
  public class TransactionScopeWrapper : IDisposable
  {
    private readonly TransactionScope _transaction;
    private bool _disposed;
    private static readonly TimeSpan DefaulTimeout = TimeSpan.FromSeconds(30);
    private const IsolationLevel DefaultIsolationLevel = IsolationLevel.ReadCommitted;

    public TransactionScopeWrapper()
      : this(TransactionScopeOption.Required, DefaulTimeout)
    {}

    public TransactionScopeWrapper(TransactionScopeOption transactionScopeOption)
      : this(transactionScopeOption, DefaulTimeout)
    {}

    public TransactionScopeWrapper(TimeSpan timeout)
      : this(TransactionScopeOption.Required, timeout)
    {}

    public TransactionScopeWrapper(
      TransactionScopeOption transactionScopeOption,
      TimeSpan timeout,
      IsolationLevel isolationLevel = DefaultIsolationLevel)
    {
      _transaction = new TransactionScope(transactionScopeOption, new TransactionOptions
      {
        IsolationLevel = isolationLevel,
        Timeout = timeout
      });
    }

    public void Complete()
    {
      _transaction.Complete();
    }

    public void Dispose()
    {
      if (!_disposed)
      {
        _transaction.Dispose();
        _disposed = true;
      }
    }
  }
}