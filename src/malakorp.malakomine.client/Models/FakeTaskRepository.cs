using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Malakorp.MalakoMine.Client.Models
{
    public class FakeTaskRepository 
    {
        public IEnumerable<Task> GetTasks()
        {
            return new Task[] {
                new Task {
                    ID = 666,
                    AssignedTo = "rafael",
                    Comment = "Blah",
                    NextStates = new [] { "Closed", "Active" },
                    State = "Active",
                    Title = "Tarefa Malaka",
                    WorkItemType = "Task",
                    HasBugRelated = true
                }
            };
        }

        public void UpdateTask(Task task,  string s, string reason)
        {
        }

        public bool CreateBugTask(Task task)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetUsers()
        {
            return new[] { "blah" };
        }

        public IEnumerable<string> GetReasons(int id, string reason)
        {
            return new[] { "blah" };
        }
    }
}