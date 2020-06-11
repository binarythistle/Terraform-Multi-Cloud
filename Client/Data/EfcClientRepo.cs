using System;
using System.Collections.Generic;
using System.Linq;
using Client.Models;

namespace Client.Data
 {
    public class EfcClientRepo : IClientRepo
    {
        private readonly ClientContext _context;

        public EfcClientRepo(ClientContext context)
        {
            _context = context;
        }

        public void CreateLog(Log log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            Console.WriteLine("Attemptting to write to context");
            _context.LogItems.Add(log);

        }

        public void DeleteLog(Log log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            _context.LogItems.Remove(log);
        }

        public IEnumerable<Log> GetAllLogs()
        {
            return _context.LogItems.ToList();
        }

        public Log GetLogById(int id)
        {
            return _context.LogItems.FirstOrDefault(p => p.Id == id);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateLog(Log log)
        {
            //Do nothing
        }
    }
}