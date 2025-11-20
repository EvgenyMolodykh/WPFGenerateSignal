using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WPFGenerateSignal.Models;

namespace WPFGenerateSignal.Services
{
    public interface ISignalStorageService
    {
        Task SaveSignalAsync(SignalMetadata signal, IProgress<double> progress = null, CancellationToken cancellationToken = default);
        Task<List<SignalMetadata>> LoadAllSignalsAsync(IProgress<double> progress = null, CancellationToken cancellationToken = default);
    }
}