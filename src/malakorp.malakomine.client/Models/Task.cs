
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;

namespace Malakorp.MalakoMine.Client.Models
{
    /// <summary>
    /// DTO para informações de tarefas do TFS.
    /// </summary>
    [HoursValidation]
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
        public bool HasBugRelated { get; set; }
        public float? RemainingWork { get; set; }
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class HoursValidationAttribute : ValidationAttribute
    {
        public HoursValidationAttribute()
            : base("Horas da Tarefa não são iguais ao restante.")
        {
        }

        public override bool IsValid(object value)
        {
            var task = value as Task;

            return task.RemainingWork != null && task.Hours == task.RemainingWork;
        }
        //protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        //{
        //    var task = value as Task;

        //    return task != null && task.State == "Closed" ?
        //        string.IsNullOrEmpty(task.Comment) ?
        //            new ValidationResult("Favor inserir um comentario para finalizar a tarefa") :
        //        task.RemainingWork != null && task.Hours != task.RemainingWork ?
        //            new ValidationResult("Horas da Tarefa não são iguais ao restante.") :
        //            ValidationResult.Success :
        //        ValidationResult.Success;
        //}
    }

    public static class TaskExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectList(this IEnumerable<string> items)
        {
            return items.Select(s => new System.Web.Mvc.SelectListItem { Text = s, Value = s });    
        }
        public static IEnumerable<SelectListItem> ToSelectList(this AllowedValuesCollection items)
        {
            return items.OfType<string>().ToSelectList();
        }
    }
}