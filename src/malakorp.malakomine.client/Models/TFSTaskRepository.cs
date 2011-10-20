using System.Collections.Generic;
using System.Linq;
using System.Web;
using Malakorp.MalakoMine.TFS;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Malakorp.MalakoMine.Client.Models
{
    public class TFSTaskRepository : ITaskRepository
    {
        private TFSMalako Malako
        {
            get
            {
                return (TFSMalako)HttpContext.Current.Session["TFSMalako"];
            }
        }

        public IEnumerable<Task> GetTasks()
        {
            return
                from wi in Malako.GetTasks().OfType<WorkItem>() where !Malako.HasTask(wi.Id)
                select new Task
                {
                    Title = wi.Title,
                    State = wi.State,
                    ID = wi.Id,
                    AssignedTo = wi.Fields["Assigned To"].Value.ToString(),
                    NextStates = Malako.GetAvailableStatesForWI(wi.State, wi.Type.Name),
                    WorkItemType = wi.Type.Name,
                    AssignedToList = wi.Fields["Assigned To"].AllowedValues,

                    HasBugRelated =
                        (from link in wi.Links.OfType<RelatedLink>()
                         where link.LinkTypeEnd.Name.Contains("Tests")
                         select link)
                        .FirstOrDefault() != null
                };
        }

        public void UpdateTask(Task task, string reason)
        {
            Malako.UpdateHours(task.ID, task.Hours, task.State, task.Comment, reason);
        }

        public bool CreateBugTask(Task task)
        {
            return Malako.CreateBugTask(task.ID, task.AssignedTo, task.Title);
        }

        public System.Collections.Generic.IEnumerable<string> GetUsers()
        {
            return Malako.GetListUsers();
        }

        public IEnumerable<string> GetReasons(int id)
        {
            WorkItem wi = this.Malako.GetWIById(id);

            foreach (Link item in wi.Links)
            {
                if (((RelatedLink)item).LinkTypeEnd.Name == "Tests")
                {
                    return Malako.GetBugReasons(((RelatedLink)item).RelatedWorkItemId);
                }
            }

            return null;
        }
    }
}