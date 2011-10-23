using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.Net;
using System.Diagnostics;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace Malakorp.MalakoMine.TFS
{
    public class MalakoQueryProvider
    {
        TeamFoundationServer tfs;
        WorkItemStore wiStore;
        Project tfsProject;

        public MalakoQueryProvider(string serverName, string projectName, NetworkCredential credentials) {
            ServerName = serverName;
            ProjectName = projectName;
            Credentials = credentials;
        }

        public string ServerName { get; set; }
        public string ProjectName { get; set; }
        public NetworkCredential Credentials { get; set; }

        #region Private Methods
        /// <summary>
        /// Recupera a quantidade de workitens filhos de um pai
        /// </summary>
        /// <param name="id">id do pai</param>
        /// <returns>Quantidade de filhos</returns>
        private int CountChilds(int id)
        {
            return new Query(wiStore,
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
            WorkItemLinkInfo[] a = new Query(wiStore,
                   " SELECT [Target].[System.Id] " +
                   " FROM WorkItemLinks " +
                   " WHERE ([Source].[System.Id] = " + id.ToString() + ") " +
                   " and ([System.Links.LinkType] = 'Parent') " +
                   " mode(MustContain)").RunLinkQuery();

            return this.GetWIById(a[1].TargetId);
        }

        /// <summary>
        /// Recupera os Bugs de uma tarefa
        /// </summary>
        /// <param name="id">ID da tarefa</param>
        /// <returns>Coleção de Bugs atrelados a uma tarefa</returns>
        private WorkItem GetBugByTaskId(int id)
        {
            WorkItemLinkInfo[] a = new Query(wiStore,
                   " SELECT [Target].[System.Id] " +
                   " FROM WorkItemLinks " +
                   " WHERE ([Source].[System.Id] = " + id.ToString() + ") " +
                   " and ([System.Links.LinkType] = 'Tests') " +
                   " and ([Target].[System.WorkItemType] = 'Bug') " +
                   " mode(MustContain)").RunLinkQuery();

            return this.GetWIById(a[1].TargetId);
            //WorkItemType wiType = tfsProject.WorkItemTypes["Bug"];
            //WorkItem linkedBug = null;

            //for (int i = 0; i < wi.WorkItemLinks.Count; i++)
            //{
            //    linkedBug = wiStore.GetWorkItem(wi.WorkItemLinks[i].TargetId);

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
            string formatedTitle = parent.Title;
            string regexStr = @"([\d\.]+\s)";

            title = Regex.Replace(title, regexStr, "");
            Match a = Regex.Match(formatedTitle, regexStr);

            if (a.Success)
            {
                formatedTitle = string.Format("{0}.{1} {2}", a.Value.Replace(" ",""), this.CountChilds(parent.Id), title);
            }

            return formatedTitle;
        }

        /// <summary>
        /// Método responsável por alterar os status de um workitem
        /// </summary>
        /// <param name="wi">Workitem Alterado</param>
        /// <param name="taskState">Novo status</param>
        /// <param name="comments">Comentários</param>
        /// <param name="bugReason">Razão de fechamento do BUG</param>
        private void ChangeStatusTask(WorkItem wi, string taskState, string comments, string bugReason)
        {
            WorkItem linkedBug = null;

            if (taskState == "Closed"
                && wi.State != "Closed")
            {
                linkedBug = this.GetBugByTaskId(wi.Id);

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
        #endregion

        public bool Connect()
        {
            try
            {
                Trace.Write(WindowsIdentity.GetCurrent().Name);

                tfs = new TeamFoundationServer(ServerName, Credentials);

                wiStore = (WorkItemStore)tfs.GetService(typeof(WorkItemStore));
                tfsProject = wiStore.Projects[this.ProjectName];

                //TODO: Validar conexão
                return true;
            }
            catch (Exception xa)
            {
                throw xa;
            }
        }

        #region Public Methods
        /// <summary>
        /// Recupera as tarefas atribuidas ao usuário logado
        /// </summary>
        /// <returns></returns>
        public WorkItemCollection GetTasks()
        {
            return wiStore.Query(
               " SELECT [System.Id], [System.WorkItemType]," +
               " [System.State], [System.AssignedTo], [System.Title] " +
               " FROM WorkItems " +
               " WHERE [System.TeamProject] = '" + tfsProject.Name +
               "' and [System.AssignedTo] = @Me " +
               " and [System.WorkItemType] not in('Demanda', 'Requirement') " +
               " and [System.State] != 'Closed' " +
               " ORDER BY [System.WorkItemType], [System.Id]");
        }

        /// <summary>
        /// Verifica se um bug já possui task
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasTask(int id)
        {
            return new Query(wiStore,
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
            return wiStore.GetWorkItem(id);
        }

        /// <summary>
        /// Recupera os status disponiveis de um workitem
        /// </summary>
        /// <param name="currentState"></param>
        /// <param name="workItemType"></param>
        /// <returns></returns>
        public IList<string> GetAvailableStatesForWI(string currentState, string workItemType)
        {
            FieldFilterList filterList = new FieldFilterList();
            FieldFilter filter = new FieldFilter(tfsProject.WorkItemTypes[workItemType].FieldDefinitions[CoreField.State], currentState);
            filterList.Add(filter);
            AllowedValuesCollection allowedValues = tfsProject.WorkItemTypes[workItemType].FieldDefinitions[CoreField.State].FilteredAllowedValues(filterList);
            IList<string> values = new List<string>(allowedValues.Count);

            foreach (string value in allowedValues)
            {
                values.Add(value);
            }

            return values;
        }

        /// <summary>
        /// Recupera os motivos definidos para o fechamento de um bug
        /// </summary>
        /// <param name="idBug"></param>
        /// <returns></returns>
        public IList<string> GetReasons(int id, string newState)
        {
            WorkItem wi = this.GetWIById(id);
            IList<string> values = new List<string>();
            // to find possible next states, the Work Item type definition (XML based format) is used.
            // get Work Item type definition
            XmlDocument witd = wi.Type.Export(true);
            // retrieve the transitions node
            XmlNode transitionsNode = witd.SelectSingleNode("descendant::TRANSITIONS");
            // for each transition definition (== possible next allowed state)
            foreach (XmlNode transitionNode in transitionsNode.SelectNodes("TRANSITION"))
            {
                // if the transition contains a next allowed state
                if (transitionNode.Attributes.GetNamedItem("from").Value == wi.State
                    && transitionNode.Attributes.GetNamedItem("to").Value.ToUpper() == newState.ToUpper())
                {
                    // retrieve the reasons node
                    XmlNode reasonsNode = transitionNode.SelectSingleNode("REASONS");
                    // for each state change reason 
                    foreach (XmlNode reason in reasonsNode.ChildNodes)
                    {
                        values.Add(reason.Attributes[0].Value);
                    }
                }
            }

            return values;
        }

        /// <summary>
        /// Recupera a lista de usuarios do sistema
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetListUsers()
        {
            List<string> ret = new List<string>();

            //Guid collectionGuid = m_collectionGuid;
            TfsTeamProjectCollection tpc = tfs.TfsTeamProjectCollection;

            // Get the group security service.
            var gss = tpc.GetService<IGroupSecurityService2>();

            //Retrieve each user's SID.
            Identity sids = gss.ReadIdentity(SearchFactor.AccountName, "itgroup\\Grupo Projeto IBBA - Boletos", QueryMembership.Expanded);

            // Resolve to named identities.
            foreach (Identity item in gss.ReadIdentities(SearchFactor.Sid, sids.Members, QueryMembership.None))
            {
                ret.Add(item.DisplayName);
            }

            return ret;
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
            float remainingWork;
            float completedWork;

            WorkItem wi = this.GetWIById(id);

            float.TryParse(wi.Fields["Remaining Work"].Value.ToString(), out remainingWork);
            if (wi.Fields["Completed Work"] != null)
                float.TryParse("0", out completedWork);
            else
                float.TryParse("0" + wi.Fields["Completed Work"].Value.ToString(), out completedWork);

            remainingWork -= qtdeHoras;
            completedWork += qtdeHoras;

            if (remainingWork < 0)
            {
                throw new Exception("Remaining Work não pode ser negativo!");
            }

            wi.Fields["Remaining Work"].Value = remainingWork;
            wi.Fields["Completed Work"].Value = completedWork;

            this.ChangeStatusTask(wi, state, comments, bugReason);
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
            try
            {
                WorkItem basedOn = this.GetWIById(idBug);

                WorkItem pbiWI = new WorkItem(tfsProject.WorkItemTypes["Task"]);
                WorkItem parent = this.GetParent(idBug);

                pbiWI.Description = string.Format("Verificar o Bug {0}\n\n{1}", basedOn.Title, basedOn.Description);
                pbiWI.Fields["Assigned To"].Value = assignedTo;

                basedOn.Fields["Assigned To"].Value = assignedTo;
                basedOn.State = "Active";
                pbiWI.Title = title;

                pbiWI.Links.Add(new RelatedLink(wiStore.WorkItemLinkTypes.LinkTypeEnds["Tests"], basedOn.Id));

                if (parent != null)
                {
                    pbiWI.Links.Add(new RelatedLink(wiStore.WorkItemLinkTypes.LinkTypeEnds["Parent"], parent.Id));
                    pbiWI.Title = this.GetTitlePrefix(parent, title);
                }

                //for (int i = 0; i < basedOn.WorkItemLinks.Count; i++)
                //{
                //    if (basedOn.WorkItemLinks[i].LinkTypeEnd.Name.Equals("Parent"))
                //    {
                //        parentId = basedOn.WorkItemLinks[i].TargetId;
                //        pbiWI.Links.Add(new RelatedLink(wiStore.WorkItemLinkTypes.LinkTypeEnds["Parent"], basedOn.WorkItemLinks[i].TargetId));
                //        break;
                //    }
                //}

                basedOn.Save();
                pbiWI.Save();

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
