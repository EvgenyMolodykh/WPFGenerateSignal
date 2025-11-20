using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WPFGenerateSignal.Date.Entities;

namespace WPFGenerateSignal.Repositoriess
{
    public interface ISignalRepository
    {
        Task<SignalEntity> SaveSignalAsync(SignalEntity signal, CancellationToken cancellationToken = default);
        Task<List<SignalEntity>> GetAllSignalsAsync(CancellationToken cancellationToken = default);
    }
}