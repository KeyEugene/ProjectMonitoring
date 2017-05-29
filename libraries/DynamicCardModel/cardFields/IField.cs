using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Teleform.Reporting.DynamicCard
{
    public interface IField
    {
        string Name { get; }

        string SystemName { get; }

        object Value { get; set; }

        bool IsNullable { get; }

        bool IsForbidden { get; set; }

        string NativeEntityName { get; }


    }

    public interface IRelationField : IField
    {       

    }

    public interface ISelfField : IField
    {
        
    }
}
