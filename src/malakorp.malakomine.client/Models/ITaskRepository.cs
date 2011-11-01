
using System.Collections.Generic;
using System.Linq;

namespace Malakorp.MalakoMine.Client.Models
{
    public interface ITaskRepository
    {
        IEnumerable<Task> GetTasks();
        void UpdateTask(Task task, string taskReason, string bugReason);
        bool CreateBugTask(Task task);
        IEnumerable<string> GetUsers();
        IEnumerable<string> GetReasons(int id, string reason);

        IEnumerable<IGrouping<string, string>> GetReasons(int id);
    }
}