using System.Collections.Generic;
using System.Linq;
using System.Text;
using JosephM.Xrm.Settings.Core;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace JosephM.Xrm.Settings.Aggregate
{
    /// <summary>
    ///     Class for processing and updating aggregate fields in plugins
    ///     For an aggregate from a lookup tthis classes execute method needs to be called from a plugin which is registered
    ///     for the PostOperation, Synchronous event
    ///     for Create, Update, SetStateDynamic and Delete messages
    ///     The Update, SetStateDynamic and Delete messages need to include a preimage with the lookup field, the aggregated
    ///     field, the state and all fields in the filter conditions
    ///     For an aggregate from an association relationahip
    ///     this classes execute method needs to be called
    ///     1. for the entity which is aggregated from a plugin which is registered for the PostOperation, Synchronous event
    ///     for Create, Update, SetStateDynamic and Delete messages
    ///     The Update, SetStateDynamic and Delete messages need to include a preimage with the lookup field, the aggregated
    ///     field, the state and all fields in the filter conditions
    ///     2. for the relationship from a plugin which is registered for the (1) PostOperation, Synchronous, Association event
    ///     (2) PreOperation, Synchronous, Disassociation event
    /// </summary>
    public abstract class XrmAggregate
    {
        protected XrmAggregate(string recordTypeWithAggregate, string aggregateField, string recordTypeAggregated,
            string aggregatedField, AggregateType aggregateType)
        {
            RecordTypeWithAggregate = recordTypeWithAggregate;
            AggregateField = aggregateField;
            RecordTypeAggregated = recordTypeAggregated;
            AggregateType = aggregateType;
            AggregatedField = aggregatedField;
            Filters = new ConditionExpression[] { };
            AddFilter("statecode", XrmPicklists.State.Active);
            LinkEntity = null;
        }

        protected string FetchAlias
        {
            get { return "aggregatevalue"; }
        }

        public string RecordTypeWithAggregate { get; private set; }
        public string AggregateField { get; private set; }

        public string RecordTypeAggregated { get; private set; }
        public AggregateType AggregateType { get; private set; }
        public ConditionExpression[] Filters { get; private set; }

        internal LinkEntity LinkEntity { get; set; }

        public string AggregatedField { get; private set; }


        /// <summary>
        ///     ONLY IMPLEMENTED FOR OPTION SETS
        /// </summary>
        public void ClearFilters()
        {
            Filters = new ConditionExpression[0];
        }

        /// <summary>
        ///     ONLY IMPLEMENTED FOR OPTION SETS
        /// </summary>
        public void AddFilter(string fieldName, int value)
        {
            Filters =
                Filters.Concat(new[] { new ConditionExpression(fieldName, ConditionOperator.Equal, value) }).ToArray();
        }

        /// <summary>
        ///     ONLY IMPLEMENTED FOR OPTION SETS
        /// </summary>
        public void AddFilter(string fieldName, ConditionOperator conditionoperator, object value)
        {
            Filters =
                Filters.Concat(new[] { new ConditionExpression(fieldName, conditionoperator, value) }).ToArray();
        }

        /// <summary>
        ///     !!ONLY IMPLEMENTED FOR STATIC LOOKUP CODES
        /// </summary>
        public void AddLinkFilter(string entityType, string lookup, string fieldName, object value)
        {
            if (LinkEntity == null)
            {
                LinkEntity = new LinkEntity(RecordTypeAggregated, entityType, lookup,
                    XrmEntity.GetPrimaryKeyName(entityType), JoinOperator.Inner);
                LinkEntity.LinkCriteria.AddCondition(new ConditionExpression(fieldName, ConditionOperator.Equal, value));
            }
            else
                throw new InvalidPluginExecutionException("Only one link filter may be added to an aggregate");
        }

        /// <summary>
        ///     If necessary updates the aggregate based on the pluginContext
        /// </summary>
        public void Execute(XrmEntityPlugin plugin)
        {
            ProcessAggregate(plugin);
        }

        protected abstract void ProcessAggregate(XrmEntityPlugin plugin);

        protected static string GetConditionFetchNode(ConditionExpression condition)
        {
            var conditionOperatorString = "";
            switch (condition.Operator)
            {
                case ConditionOperator.Equal:
                    {
                        conditionOperatorString = "eq";
                        break;
                    }
                case ConditionOperator.NotEqual:
                    {
                        conditionOperatorString = "ne";
                        break;
                    }
                case ConditionOperator.In:
                    {
                        conditionOperatorString = "in";

                        break;
                    }
            }
            if (conditionOperatorString.IsNullOrWhiteSpace())
                throw new InvalidPluginExecutionException(
                    string.Format("Error Getting Condition Operator String For Operator Type {0}",
                        condition.Operator));
            if (condition.Operator == ConditionOperator.In)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append(string.Format("<condition attribute='{0}' operator='{1}' >",
                    condition.AttributeName,
                    conditionOperatorString));
                foreach (var value in condition.Values)
                {
                    if (value is IEnumerable<object>)
                    {
                        foreach (var nestValue in (IEnumerable<object>)value)
                            stringBuilder.Append(string.Format("<value>{0}</value>", nestValue));
                    }
                    else
                        stringBuilder.Append(string.Format("<value>{0}</value>", value));
                }
                stringBuilder.Append("</condition>");
                return stringBuilder.ToString();
            }
            return string.Format("<condition attribute='{0}' operator='{1}' value='{2}' />", condition.AttributeName,
                conditionOperatorString, condition.Values[0]);
        }
    }
}