using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace PlainConcepts.LinqToTfs
{
    public class WorkItemQuery : IEnumerable<WorkItem>
    {
        readonly WorkItemStore store;
        readonly Expression condition;
        readonly Dictionary<string, string> translations = new Dictionary<string, string> {
                { "CreatedBy",     "[Created By]" },
                { "CreatedDate",   "[Created Date]" },
                { "Id",            "[Id]" },
                { "Title",         "[Title]" },
                { "Project.Name",  "[System.TeamProject]" }
            };
        
        public WorkItemQuery(WorkItemStore store)
        {
            this.store = store;
        }

        public WorkItemQuery(WorkItemStore store, Expression condition)
        {
            this.store = store;
            this.condition = condition;
        }

        private WorkItemQuery CreateQuery(Expression e)
        {
            var mc = e as MethodCallExpression;
            if (mc == null)
                return null;
            
            switch (mc.Method.Name)
            {
                case "Where":
                    var expr = (mc.Arguments[1] as UnaryExpression).Operand as LambdaExpression;
                    return new WorkItemQuery(store, condition == null ? (Expression)expr : Expression.And(condition, expr));
                case "OrderBy":
                    LambdaExpression expr2 =
                        (mc.Arguments[1] as UnaryExpression).Operand as LambdaExpression;
                    // TODO: sortCriteria.Add(expr2);
                    return this;
                case "Select":
                    /* TODO:
                       LambdaExpression expr3 =
                        (mc.Arguments[1] as UnaryExpression).Operand as LambdaExpression;
                       fieldList.Add(expr3);
                     */
                    return this;
                default:
                    throw new NotImplementedException();
            }
        }


        public string Translate(Expression condition)
        {
            if (condition == null)
                return "";
            
            switch (condition.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return Translate((condition as BinaryExpression).Left) + " AND " + Translate((condition as BinaryExpression).Right);
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return Translate((condition as BinaryExpression).Left) + " OR " + Translate((condition as BinaryExpression).Right);
                case ExpressionType.Equal:
                    return Translate((condition as BinaryExpression).Left) + " = " + Translate((condition as BinaryExpression).Right);
                case ExpressionType.NotEqual:
                    return Translate((condition as BinaryExpression).Left) +  " <> " + Translate((condition as BinaryExpression).Right);
                case ExpressionType.GreaterThan:
                    return Translate((condition as BinaryExpression).Left) + " > " + Translate((condition as BinaryExpression).Right);
                case ExpressionType.GreaterThanOrEqual:
                    return Translate((condition as BinaryExpression).Left) + " >= " + Translate((condition as BinaryExpression).Right);
                case ExpressionType.LessThan:
                    return Translate((condition as BinaryExpression).Left) + " < " + Translate((condition as BinaryExpression).Right);
                case ExpressionType.LessThanOrEqual:
                    return Translate((condition as BinaryExpression).Left) + " <= " + Translate((condition as BinaryExpression).Right);
                case ExpressionType.Constant:
                    var c = condition as ConstantExpression;
                    return c.Type == typeof(string) ? "'" + c.Value.ToString() + "'" : c.Value.ToString();
                case ExpressionType.MemberAccess:
                    var m = condition as MemberExpression;
                    string name = m.ToString();
                    string pName = name.Substring(name.IndexOf('.') + 1);
                    return translations[pName];
                default:
                    throw new NotImplementedException();
            }
        }

        public IEnumerator<WorkItem> GetEnumerator()
        {
            var query = "SELECT Id \nFROM WorkItems" + condition != null ? "\n WHERE " + Translate((condition as LambdaExpression).Body) : "";
            
            foreach (WorkItem wi in store.Query(query))
                yield return wi;
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}