using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WPFGenerateSignal.Date.Context;
using WPFGenerateSignal.Date.Entities;

namespace WPFGenerateSignal.Repositoriess
{
    public class SignalRepository : ISignalRepository, IDisposable
    {
        private readonly ApplicationDbContext _context;

        public SignalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SignalEntity> SaveSignalAsync(SignalEntity signal, CancellationToken cancellationToken = default)
        {
            signal.Id = Guid.NewGuid();
            signal.CreatedAt = DateTime.Now;

            foreach (var point in signal.Points)
            {
                point.Id = Guid.NewGuid();
                point.SignalId = signal.Id;
            }

            await _context.Signals.AddAsync(signal, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return signal;
        }

        public async Task<List<SignalEntity>> GetAllSignalsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Signals
                .Include(s => s.Points)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}