using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



using System.Web.Script.Serialization;
using System.Net;


namespace Teleform.Reporting.Web
{
    class Instance
    {
        public string ID { get; set; }
        public string Data { get; set; }
    }
    class InstanceCollection
    {
        public IEnumerable<Instance> Instances { get; set; }
    }

    public partial class TableViewControl
    {
        public void SetTemplateFieldsSize(string ColResizableBox)
        {
            if (!string.IsNullOrEmpty(ColResizableBox))
            {
                isColResizable = true;

                IDictionary<string, string> fieldsSizeDict = ColResizableBox.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(part => part.Split('='))
                    .ToDictionary(split => split[0], split => split[1]);

                var sessionKey = string.Concat(Template.ID, "fieldsSizeDict");

                SessionContent.FieldsSize[sessionKey] = fieldsSizeDict;
            }
        }

        public void SetSelfColumnsValue(string SaveObjectsJeysonBox)
        {
            if (!string.IsNullOrEmpty(SaveObjectsJeysonBox))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                InstanceCollection InstanceCollection = serializer.Deserialize<InstanceCollection>(SaveObjectsJeysonBox);

                foreach (var Instance in InstanceCollection.Instances)
                {
                    var InstanceID = Instance.ID;

                    var selfColumnsValueDict = new Dictionary<string, string>();
                    var InstanceData = Instance.Data.TrimEnd(';').Split(';');

                    foreach (string item in InstanceData)
                    {
                        string[] keyValue = item.Split('=');

                        try
                        {
                            selfColumnsValueDict[keyValue[0]] = keyValue[1];
                            //selfColumnsValueDict.Add(keyValue[0], keyValue[1]);
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException();
                        }
                    }

                    var sessionKey = string.Concat("entityInstance", Template.Entity.SystemName, "_", InstanceID);
                    EntityInstance entityInstance;

                    var isFound = SessionContent.EntityInstances.FirstOrDefault(ei => ei.Key == sessionKey).Key;

                    if (!SessionContent.IsInstanceAdded && InstanceID == "-1")
                    {
                        SessionContent.EntityInstances.Remove(sessionKey);
                    }
                    else if (isFound == null)
                    {
                        entityInstance = new EntityInstance(InstanceID, Template.Entity.SystemName, false);
                        entityInstance.SelfColumnsValue = selfColumnsValueDict;
                        SessionContent.EntityInstances[sessionKey] = entityInstance;
                    }
                    else
                        SessionContent.EntityInstances[sessionKey].SelfColumnsValue = selfColumnsValueDict;
                }
            }

        }
    }
}
