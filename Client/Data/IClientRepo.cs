using System.Collections.Generic;
using Client.Models;

namespace Client.Data
{
    public interface IClientRepo
    {
        bool SaveChanges();

        IEnumerable<Log> GetAllLogs();
        Log GetLogById(int id);
        void CreateLog(Log log);
        void UpdateLog(Log log);
        void DeleteLog(Log log);
    }
}