using System;
using Teleform.Reporting.WordExcelTemplateAddIns;

namespace Teleform.Reporting
{
    public class UniqueIDCreator
    {

        public object Aggregate(AddInEntity entity, AddInAttribute attribute, Format format)
        {
            if (entity == null)
                throw new ArgumentNullException("entity", Message.Get("Common.NullArgument", "entity"));

            if (attribute == null)
                throw new ArgumentNullException("attribute", Message.Get("Common.NullArgument", "attribute"));

            if (format == null)
                throw new ArgumentNullException("format", Message.Get("Common.NullArgument", "format"));

            return entity.ID + "|" + attribute.ID + "|" + format.ID;
        }


        public object Aggregate(Entity entity, Attribute attribute, Format format)
        {
            if (entity == null)
                throw new ArgumentNullException("entity", Message.Get("Common.NullArgument", "entity"));

            if (attribute == null)
                throw new ArgumentNullException("attribute", Message.Get("Common.NullArgument", "attribute"));

            if (format == null)
                throw new ArgumentNullException("format", Message.Get("Common.NullArgument", "format"));

            return entity.ID + "|" + attribute.ID + "|" + format.ID;
        }

        public void Split(object uniqueID, out object entityID, out object attributeID, out object formatID)
        {
            if (uniqueID == null)
                throw new ArgumentNullException("uniqueID", Message.Get("Common.NullArgument", "uniqueID"));

            var parts = uniqueID.ToString().Split('|');

#warning Допускается, что структура id допускает не менее 3 элементов.
            if (parts.Length < 3)
                throw new ArgumentException("Передан невалидный уникальный идентификатор.", "uniqueID");

            entityID = parts[0];
            attributeID = parts[1];
            formatID = parts[2];
        }
    }
}
