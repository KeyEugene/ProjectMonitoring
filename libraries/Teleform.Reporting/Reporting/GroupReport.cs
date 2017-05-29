#define alexj


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using Teleform.Reporting.SeparationOfAccessRightsNew;


using DataTable = System.Data.DataTable;
using DataRow = System.Data.DataRow;

namespace Teleform.Reporting
{
    public class GroupReport : Report
    {
        public IEnumerable<Instance> Instances { get; private set; }

        public GroupReport(Template template, IEnumerable<Instance> instances)
            : base(template)
        {
            this.Instances = instances;
        }


        public static GroupReport Make(Template template, DataTable table, string userID = null)
        {
            if (table.Columns["objID"] != null)
            {
                if (!string.IsNullOrEmpty(userID))
                {
                    table = AuthorizationRulesNew.EntityInstancesResolution(table, template.Entity, userID);
                }

                table.Columns.Remove("objID");
            }

            int rowsCount = table.Rows.Count, colCount = table.Columns.Count;
            var instances = new Instance[rowsCount];
            DataRow row;
            var attributes = new List<Teleform.Reporting.Attribute>(colCount);

            foreach (var item in template.Fields)
            {
                var attribute = template.Entity.Attributes.FirstOrDefault(o => o.ID.ToString() == item.Attribute.ID.ToString());
                attributes.Add(attribute);
            }
            for (int i = 0; i < instances.Length; i++)
            {
                row = table.Rows[i];

                var properties = new List<Instance.Property>();

#if alexj

                for (int j = 0; j < attributes.Count; j++)
                    if (attributes[j] != null)
                    {
                        var attr = attributes[j];
                        var cell = row[attributes[j].ID.ToString()];
                        properties.Add(new Instance.Property(attributes[j], cell));
                    }

#else

                for (int j = 0; j < colCount; j++)
                    if (attributes[j] != null)
                    {
                        var attr = attributes[j];
                        var cell = row[j];
                        properties.Add(new Instance.Property(attributes[j], cell));
                    }

#endif

                instances[i] = new Instance(template.Entity, properties);
            }

            return new Teleform.Reporting.GroupReport(template, instances);
        }
    }


}
