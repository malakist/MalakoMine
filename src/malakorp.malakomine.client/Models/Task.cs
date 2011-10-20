
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.Web.Mvc;

namespace Malakorp.MalakoMine.Client.Models
{
    //[CustomValidation(typeof(ValidateTask), "Validation")]
    public class Task
    {
        public int ID { get; set; }

        [Required]
        public string State { get; set; }
      
        [Range(0, 23.99)]
        public float Hours { get; set; }
        
        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }
        
        [Required]
        public string AssignedTo { get; set; }
        
        [Required]
        public string Title { get; set; }

        public string WorkItemType { get; set; }

        public IEnumerable<string> NextStates { get; set; }

        public AllowedValuesCollection AssignedToList { get; set; }

        public bool HasBugRelated { get; set; }

    }

    public class ValidateTask
    {
        public static ValidationResult Validation(Task task)
        {
            bool isValid = true;
            string message = string.Empty;

            if (task.State == "Closed" && string.IsNullOrEmpty(task.Comment))
            {
                isValid = false;
                message = "Favor inserir um comentario para finalizar a tarefa";
            }

            if (isValid)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(message);
            }
        }
    }

    public static class TaskExtensions
    {
        public static IEnumerable<System.Web.Mvc.SelectListItem> ToSelectList(this IEnumerable<string> items)
        {
            return items.Select(s => new System.Web.Mvc.SelectListItem { Text = s, Value = s });
            
        }

        public static IEnumerable<System.Web.Mvc.SelectListItem> ToSelectList(this AllowedValuesCollection items)
        {
            return Enumerable.OfType<string>(items).Select(s => new System.Web.Mvc.SelectListItem { Text = s, Value = s });
        }
    }


}