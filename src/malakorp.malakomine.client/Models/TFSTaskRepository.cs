using System.Collections.Generic;
using System.Linq;
using System.Web;
using Malakorp.MalakoMine.TFS;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Malakorp.MalakoMine.Client.Models
{
	public class TFSTaskRepository : ITaskRepository
	{
		private MalaKueryProvider Malako
		{
			get { return (MalaKueryProvider)HttpContext.Current.Session["TFSMalako"]; }
		}

		public IEnumerable<Task> GetTasks()
		{
			//return from item in Malako.GetTasks().OfType<WorkItem>()
			//       where !Malako.HasTask(item.Id)
			//       select new Task
			//       {
			//           Title = item.Title,
			//           State = item.State,
			//           ID = item.Id,
			//           AssignedTo = item.Fields["Assigned To"].Value.ToString(),
			//           NextStates = Malako.GetNextStates(item.State, item.Type.Name),
			//           WorkItemType = item.Type.Name,
			//           HasBugRelated = item.RelatedLinks("Parent").SingleOrDefault() != null,
			//           RemainingWork = item.Type.Name == "Task" ? (double?)item.Fields["Remaining Work"].Value : default(double?)
			//       };

			foreach (var item in Malako.GetTasks().OfType<WorkItem>().Where(i => !Malako.HasTask(i.Id)))
			{
				var remaining = item.Type.Name == "Task" ? (double?)item.Fields["Remaining Work"].Value : default(double?);
				yield return new Task
				{
					Title = item.Title,
					State = item.State,
					ID = item.Id,
					AssignedTo = item.Fields["Assigned To"].Value.ToString(),
					NextStates = Malako.GetNextStates(item.State, item.Type.Name),
					WorkItemType = item.Type.Name,
					HasBugRelated = item.RelatedLinks("Parent").SingleOrDefault() != null,
					RemainingWork = remaining
				};
			}
		}

		public void UpdateTask(Task task, string taskReason, string bugReason)
		{
			Malako.UpdateHours(task.ID, task.Hours, task.State, task.Comment, taskReason, bugReason);
		}

		public bool CreateBugTask(Task task)
		{
			return Malako.CreateBugTask(task.ID, task.AssignedTo, task.Title);
		}

		public IEnumerable<string> GetUsers()
		{
			return Malako.GetUsers();
		}

		public IEnumerable<string> GetReasons(int id, string state)
		{
			return Malako.GetReasons(id, state);

			//var wi = Malako.GetWorkItem(id);

			//foreach (Link item in wi.Links)
			//{
			//    if ((item as RelatedLink).LinkTypeEnd.Name == "Tests")
			//    {
			//        return Malako.GetReasons((item as RelatedLink).RelatedWorkItemId, state);
			//    }
			//}

			//return null;
		}


		public IEnumerable<IGrouping<string, string>> GetReasons(int id)
		{
			return Malako.GetReasons(id);
		}
	}

	public static class TFSExtensions
	{
		public static IEnumerable<RelatedLink> RelatedLinks(this WorkItem item, string linkEndName)
		{
			return item.Links.OfType<RelatedLink>().Where(link => link.LinkTypeEnd.Name == linkEndName);
		}
	}
}