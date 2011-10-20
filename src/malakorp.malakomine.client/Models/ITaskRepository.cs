
using System.Collections.Generic;

namespace Malakorp.MalakoMine.Client.Models
{
    public interface ITaskRepository
    {
        IEnumerable<Task> GetTasks();
        void UpdateTask(Task task, string reason);
        bool CreateBugTask(Task task);
        IEnumerable<string> GetUsers();
        IEnumerable<string> GetReasons(int id);
    }
}