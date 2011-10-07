using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client; 



namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string serverName = "http://itgvs17:8080/tfs/defaultcollection";
            TeamFoundationServer tfs = new TeamFoundationServer(serverName);

            WorkItemStore wis = (WorkItemStore)tfs.GetService(typeof(WorkItemStore));
            Project tfs_project = wis.Projects["Boletos"];

            MessageBox.Show(tfs_project.Name);

            //Perform WIQL Query 
            WorkItemCollection wic = wis.Query(
               " SELECT [System.Id], [System.WorkItemType]," +
               " [System.State], [System.AssignedTo], [System.Title] " +
               " FROM WorkItems " +
               " WHERE [System.TeamProject] = '" + tfs_project.Name +
               "' and [System.Id] in (1703) ORDER BY [System.WorkItemType], [System.Id]");

            foreach (WorkItem wi in wic)
            {
                //wi.Fields["AssignedTo"]
                MessageBox.Show(wi.Title + "[" + wi.Type.Name + "]" + wi.Description);

                //WorkItem changeRequest = DataManager.DevelopmentProject.Store.GetWorkItem(wi.Id);

                string teste = string.Empty;

                for (int i = 0; i < wi.Fields.Count; i++)
                {
                    teste += ";" + wi.Fields[i].Name;
                }

                MessageBox.Show(teste);
                //wi.DataManager.BugType.FieldDefinitions["Severity"].AllowedValues;

                wi.Fields["Completed Work"].Value = 55;
                wi.Save();
                this.GetStatesForState(wi.Fields["State"].Value.ToString());
            }

        }

            public IList<string> GetStatesForState(string currentState) {
                string serverName = "http://itgvs17:8080/tfs/defaultcollection"; 
                TeamFoundationServer tfs = new TeamFoundationServer(serverName);

                WorkItemStore wis = (WorkItemStore)tfs.GetService(typeof(WorkItemStore));
                Project tfs_project = wis.Projects["Boletos"];

                FieldFilterList filterList = new FieldFilterList();
                FieldFilter filter = new  FieldFilter(tfs_project.WorkItemTypes["Task"].FieldDefinitions[CoreField.State], currentState);
                filterList.Add(filter);
                AllowedValuesCollection allowedValues = tfs_project.WorkItemTypes["Task"].FieldDefinitions[CoreField.State].FilteredAllowedValues(filterList);
                IList<string> values = new List<string>(allowedValues.Count);

                foreach (string value in allowedValues)
                {
                values.Add(value);
                }

                return values;

            }



        
    }
}
