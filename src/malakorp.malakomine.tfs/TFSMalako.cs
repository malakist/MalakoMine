using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.Text;

namespace Malakorp.MalakoMine.TFS
{
    public class TFSMalako
	{
		TeamFoundationServer server;
		WorkItemStore store;
		Project project;

		public string ServerName { get; set; }
		public string ProjectName { get; set; }
		public NetworkCredential Credentials { get; set; }

		#region Private Methods
		/// <summary>
		/// Recupera match quantidade de workitens filhos de um pai
		/// </summary>
		/// <param name="id">id do pai</param>
		/// <returns>Quantidade de filhos</returns>
		private int CountChildren(int id)
		{
			return new Query(store,
				   " SELECT [System.Id] " +
				   " FROM WorkItemLinks " +
				   " WHERE ([Source].[System.Id] = " + id.ToString() + ") " +
				   " and ([System.Links.LinkType] = 'Child') " +
				   " mode(MustContain)").RunCountQuery() - 1;
		}

		/// <summary>
		/// Recupera o workitem pai de um filho
		/// </summary>
		/// <param name="id">id do filho</param>
		/// <returns>WorkItem Pai</returns>
		private WorkItem GetParent(int id)
		{
			var q = new Query(store,
				   " SELECT [Target].[System.Id] " +
				   " FROM WorkItemLinks " +
				   " WHERE ([Source].[System.Id] = " + id.ToString() + ") " +
				   " and ([System.Links.LinkType] = 'Parent') " +
				   " mode(MustContain)").RunLinkQuery();

			return GetWIById(q[1].TargetId);
		}

		/// <summary>
		/// Recupera os Bugs de uma tarefa
		/// </summary>
		/// <param name="id">ID da tarefa</param>
		/// <returns>Coleção de Bugs atrelados match uma tarefa</returns>
		private WorkItem GetBugByTaskId(int id)
		{
			var q = new Query(store,
				   " SELECT [Target].[System.Id] " +
				   " FROM WorkItemLinks " +
				   " WHERE ([Source].[System.Id] = " + id.ToString() + ") " +
				   " and ([System.Links.LinkType] = 'Tests') " +
				   " and ([Target].[System.WorkItemType] = 'Bug') " +
				   " mode(MustContain)").RunLinkQuery();

			return GetWIById(q[1].TargetId);
			//WorkItemType wiType = project.WorkItemTypes["Bug"];
			//WorkItem linkedBug = null;

			//for (int i = 0; i < wi.WorkItemLinks.Count; i++)
			//{
			//    linkedBug = store.GetWorkItem(wi.WorkItemLinks[i].TargetId);

			//    if (wi.WorkItemLinks[i].LinkTypeEnd.Name.Equals("Tests")
			//        && linkedBug.Type == wiType
			//        && !linkedBug.State.Equals("Closed"))
			//    {
			//        return linkedBug;
			//    }
			//}

			//return null;
		}

		/// <summary>
		/// Formata o titulo de uma nova task, com base no seu Pai
		/// </summary>
		/// <param name="parent">WorkItem pai</param>
		/// <param name="title">Titulo definido</param>
		/// <returns>Titulo formatado</returns>
		private string GetTitlePrefix(WorkItem parent, string title)
		{
			const string regexStr = @"([\d\.]+\s)";

			var match = Regex.Match(parent.Title, regexStr);

			if (match.Success)
			{
				return string.Format("{0}.{1} {2}", match.Value.Replace(" ", ""), CountChildren(parent.Id), Regex.Replace(title, regexStr, ""));
			}
			else
			{
				return parent.Title;
			}
		}

		/// <summary>
		/// Método responsável por alterar os status de um workitem
		/// </summary>
		/// <param name="wi">Workitem Alterado</param>
		/// <param name="taskState">Novo status</param>
		/// <param name="comments">Comentários</param>
		/// <param name="bugReason">Razão de fechamento do BUG</param>
		private void ChangeTaskStatus(WorkItem wi, string taskState, string comments, string bugReason)
		{
			var linkedBug = default(WorkItem);

			if (taskState == "Closed" && wi.State != "Closed")
			{
				linkedBug = GetBugByTaskId(wi.Id);

				if (linkedBug != null)
				{
					linkedBug.Fields["Assigned To"].Value = linkedBug.CreatedBy;
					linkedBug.State = "Resolved";
					linkedBug.Fields["Reason"].Value = bugReason;
					linkedBug.History = comments;
				}
			}

			wi.State = taskState;
			wi.History = comments;

			wi.Save();

			if (linkedBug != null)
				linkedBug.Save();
		}

        /// <summary>
        /// Recupera as tarefas atribuidas ao usuário logado
        /// </summary>
        /// <returns></returns>
        private WorkItemCollection PrivateGetTasks(System.DateTime? sinceDateTime)
        {
            StringBuilder query = new StringBuilder();
            query.Append(" SELECT [System.Id], [System.WorkItemType]," +
               " [System.State], [System.AssignedTo], [System.Title] " +
               " FROM WorkItems " +
               " WHERE [System.TeamProject] = '" + project.Name +
               "' and [System.AssignedTo] = @Me " +
               " and [System.WorkItemType] not in('Demanda', 'Requirement') " +
               " and [System.State] != 'Closed' ");
            
            if (sinceDateTime != null)
                query.Append(" and [System.CreatedDate] >= '" + sinceDateTime.Value.ToString("MM/dd/yyyy") + "'");

            query.Append(" ORDER BY [System.WorkItemType], [System.Id]");
            
            return store.Query(query.ToString());
        }
		#endregion

		#region Public Methods

		public bool Connect()
		{
			try
			{
				//
				//Trace.Write(WindowsIdentity.GetCurrent().Name);

				server = new TeamFoundationServer(ServerName, Credentials);
				store = server.GetService(typeof(WorkItemStore)) as WorkItemStore;
				project = store.Projects[ProjectName];

				//TODO: Validar conexão
				return true;
			}
			catch (Exception xa)
			{
				throw xa;
			}
		}

		/// <summary>
		/// Recupera as tarefas atribuidas ao usuário logado
		/// </summary>
		/// <returns></returns>
		public WorkItemCollection GetTasks()
		{
            return PrivateGetTasks(null);
		}

        public WorkItemCollection GetTasksSince(DateTime sinceDateTime)
        {
            return PrivateGetTasks(sinceDateTime);
        }

        public bool ThereAreNewTasksSince(DateTime sinceDateTime)
        {
            return (GetTasksSince(sinceDateTime).Count > 0);
        }

		/// <summary>
		/// Verifica se um bug já possui task
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool HasTask(int id)
		{
			return new Query(store,
				   " SELECT [System.Id] " +
				   " FROM WorkItemLinks " +
				   " WHERE ([Source].[System.Id] = " + id.ToString() + ") " +
				   " and ([System.Links.LinkType] = 'Tested By') " +
				   " and ([Target].[System.State] != 'Closed') " +
				   " mode(MustContain)").RunCountQuery() > 0;
		}

		/// <summary>
		/// Recupera um workItem baseado no ID
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public WorkItem GetWIById(int id)
		{
			return store.GetWorkItem(id);
		}

		/// <summary>
		/// Recupera os status disponiveis de um workitem
		/// </summary>
		/// <param name="currentState"></param>
		/// <param name="type">O tipo do Work Item</param>
		/// <returns></returns>
		public IEnumerable<string> GetAvailableStatesForWI(string currentState, string type)
		{
			var filters = new FieldFilterList();
			filters.Add(new FieldFilter(project.WorkItemTypes[type].FieldDefinitions[CoreField.State], currentState));
			
			//var allowed = project.WorkItemTypes[type].FieldDefinitions[CoreField.State].FilteredAllowedValues(filters);
			//var values = new List<string>(allowed.Count);

			//foreach (string value in allowed)
			//    values.Add(value);
			
			//return values;
			return project.WorkItemTypes[type].FieldDefinitions[CoreField.State].FilteredAllowedValues(filters).Cast<string>();
		}

		/// <summary>
		/// Recupera os motivos definidos para o fechamento de um bug
		/// </summary>
		/// <param name="idBug"></param>
		/// <returns></returns>
		public IEnumerable<string> GetReasons(int id, string newState)
		{
			var wi = GetWIById(id);
			
			// to find possible next states, the Work Item type definition (XML based format) is used.
			// get Work Item type definition
			var typedef = wi.Type.Export(true) as XmlDocument;
			
			// retrieve the transitions node
			var trans = typedef.SelectSingleNode("descendant::TRANSITIONS");

			#region //
			//var values = new List<string>();
			// for each transition definition (== possible next allowed state)
			//foreach (XmlNode node in trans.SelectNodes("TRANSITION"))
			//{
			//    // if the transition contains match next allowed state
			//    if (node.Attributes.GetNamedItem("from").Value == wi.State
			//        && node.Attributes.GetNamedItem("to").Value.ToUpper() == newState.ToUpper())
			//    {
			//        // retrieve the reasons node
			//        var reasons = node.SelectSingleNode("REASONS");
			//        // for each state change reason 
			//        foreach (XmlNode reason in reasons.ChildNodes)
			//        {
			//            values.Add(reason.Attributes[0].Value);
			//        }
			//    }
			//}
			//return values;
			#endregion

			return from node in trans.SelectNodes("TRANSITION").OfType<XmlNode>()
				   let attrs = node.Attributes
				   let @from = attrs.GetNamedItem("from").Value
				   let to	 = attrs.GetNamedItem("to").Value
				   where @from == wi.State && to.ToUpper() == newState.ToUpper()
				   from reason in node.SelectSingleNode("REASONS").OfType<XmlNode>()
				   select reason.Attributes[0].Value;
		}

		/// <summary>
		/// Recupera match lista de usuarios do sistema
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetUsers()
			//Rename do método quebrará código. Ao menos some com esse "List".
		{
			const string boleto = "itgroup\\Grupo Projeto IBBA - Boletos";
			
			// Get the group security service.
			var gss = server.TfsTeamProjectCollection.GetService<IGroupSecurityService2>();

			//Retrieve each user's SID.
			var sids = gss.ReadIdentity(SearchFactor.AccountName, boleto, QueryMembership.Expanded);

			#region //
			//List<string> ret = new List<string>();
			// Resolve to named identities.
			//foreach (var item in gss.ReadIdentities(SearchFactor.Sid, sids.Members, QueryMembership.None))
			//{
			//    ret.Add(item.DisplayName);
			//}
			//return ret;
			#endregion

			return gss.ReadIdentities(SearchFactor.Sid, sids.Members, QueryMembership.None).Select(val => val.DisplayName);
		}

		/// <summary>
		/// Atualiza as horas de uma task
		/// </summary>
		/// <param name="id"></param>
		/// <param name="qtdeHoras"></param>
		/// <param name="state"></param>
		/// <param name="comments"></param>
		/// <param name="bugReason"></param>
		public void UpdateHours(int id, float qtdeHoras, string state, string comments, string bugReason)
		{
			float remaining;
			float completed = 0F;

			const string remainingField = "Remaining Work";
			const string completedField = "Completed Work";

			var wi = GetWIById(id);

			if (float.TryParse(wi.Fields[remainingField].Value.ToString(), out remaining))
			{
				if (wi.Fields[completedField] != null)
					//TODO: WTF? Me parece estarem invertidas as condições.
					//float.TryParse("0", out completed);
					completed = 0;
				else
					float.TryParse("0" + wi.Fields[completedField].Value.ToString(), out completed);
			}

			remaining -= qtdeHoras;
			completed += qtdeHoras;

			//TODO: Tentar mover esta validação para atributos.
			if (remaining < 0)
				throw new Exception("Remaining Work não pode ser negativo!");

			wi.Fields[remainingField].Value = remaining;
			wi.Fields[completedField].Value = completed;

			ChangeTaskStatus(wi, state, comments, bugReason);
		}

		/// <summary>
		/// Responsável por criar tarefas de BUG
		/// </summary>
		/// <param name="idBug"></param>
		/// <param name="assignedTo"></param>
		/// <param name="title"></param>
		/// <returns></returns>
		public bool CreateBugTask(int idBug, string assignedTo, string title)
		{
			// TODO: Onde está se esperando a exceção? No save?
			try
			{
				var basedOn = GetWIById(idBug);

				var task = new WorkItem(project.WorkItemTypes["Task"]);
				var parent = GetParent(idBug);

				task.Description = string.Format("Verificar o Bug {0}\n\n{1}", basedOn.Title, basedOn.Description);
				task.Fields["Assigned To"].Value = assignedTo;

				basedOn.Fields["Assigned To"].Value = assignedTo;
				basedOn.State = "Active";
				task.Title = title;

				task.Links.Add(new RelatedLink(store.WorkItemLinkTypes.LinkTypeEnds["Tests"], basedOn.Id));

				//TODO: Existe a possibilidade de um bug não ter tarefa pai?
				if (parent != null)
				{
					task.Links.Add(new RelatedLink(store.WorkItemLinkTypes.LinkTypeEnds["Parent"], parent.Id));
					task.Title = GetTitlePrefix(parent, title);
				}

				#region //
				//for (int i = 0; i < basedOn.WorkItemLinks.Count; i++)
				//{
				//    if (basedOn.WorkItemLinks[i].LinkTypeEnd.Name.Equals("Parent"))
				//    {
				//        parentId = basedOn.WorkItemLinks[i].TargetId;
				//        task.Links.Add(new RelatedLink(store.WorkItemLinkTypes.LinkTypeEnds["Parent"], basedOn.WorkItemLinks[i].TargetId));
				//        break;
				//    }
				//}
				#endregion

				basedOn.Save();
				task.Save();

				return true;
			}
			catch (Exception)
			{
				return false;
			}

		}
		#endregion
	}
}
