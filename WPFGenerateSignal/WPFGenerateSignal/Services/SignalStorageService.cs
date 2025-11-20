using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WPFGenerateSignal.Date.Entities;
using WPFGenerateSignal.Models;
using WPFGenerateSignal.Repositoriess;

namespace WPFGenerateSignal.Services
{
    public class SignalStorageService : ISignalStorageService
    {
        private readonly ISignalRepository _repository;

        public SignalStorageService(ISignalRepository repository)
        {
            _repository = repository;
        }

        public async Task SaveSignalAsync(SignalMetadata signal, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            var entity = MapToEntity(signal);
            await _repository.SaveSignalAsync(entity, cancellationToken);
            progress?.Report(100);
        }

        public async Task<List<SignalMetadata>> LoadAllSignalsAsync(IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAllSignalsAsync(cancellationToken);
            progress?.Report(100);
            return entities.Select(MapToModel).ToList();
        }

        private SignalEntity MapToEntity(SignalMetadata model)
        {
            return new SignalEntity
            {
                Name = model.Name ?? "Сигнал",
                SignalType = model.SignalType,
                Amplitude = model.Amplitude,
                Frequency = model.Frequency,
                Phase = model.Phase,
                Points = model.Points.Select(p => new SignalPointEntity
                {
                    Time = p.X,
                    Value = p.Y
                }).ToList()
            };
        }

        private SignalMetadata MapToModel(SignalEntity entity)
        {
            return new SignalMetadata
            {
                Id = entity.Id,
                Name = entity.Name,
                SignalType = entity.SignalType,
                Amplitude = entity.Amplitude,
                Frequency = entity.Frequency,
                Phase = entity.Phase,
                Points = entity.Points.Select(p => new DataPoint
                {
                    X = p.Time,
                    Y = p.Value
                }).ToList(),
                CreatedAt = entity.CreatedAt
            };
        }
    }
}