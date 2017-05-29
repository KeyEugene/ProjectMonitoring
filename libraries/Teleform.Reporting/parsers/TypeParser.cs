#define NEW_APPROACH

using System;
using System.Collections.Generic;
using System.Linq;
using XElement = System.Xml.Linq.XElement;

namespace Teleform.Reporting.Parsers
{
    public class TypeParser : IParser
    {
        private IParser parser;
        private FormatParser formatParser;
#if NEW_APPROACH
        private OperatorParser operatorParser;
        private AggregateFunctionParser aggregateFunctionParser;
#endif
        public TypeParser()
        {
            parser = this;
            formatParser = new FormatParser();
#if NEW_APPROACH
            operatorParser = new OperatorParser();
            aggregateFunctionParser = new AggregateFunctionParser();
#endif
        }

        public Type Parse(XElement e)
        {
            return (Type) parser.Parse(e);
        }

        object IParser.Parse(XElement e)
        {
            if (e.Name != "type")
                throw new ArgumentException();

            var nameAttribute = e.Attribute("name");

            if (nameAttribute == null)
                throw new ArgumentException();

            var runtimeTypeAttribute = e.Attribute("runtimeType");

#if LATER
            if (runtimeTypeAttribute == null)
                throw new ArgumentException(string.Format("Для типа {0} не заполнено поле runTimeTypeID в таблице [Typing].[BusinessType]", nameAttribute.Value));
#endif

            var formatElements = e.Elements("format");

#if !NEW_APPROACH
            return new Type(nameAttribute.Value, Convert.ToBoolean(Convert.ToInt32(aggregableAttribure.Value)), formatElements.Select(o => formatParser.Parse(o)));
#else
            IEnumerable<Operator> operators = null;

            var operatorsElement = e.Element("operators");

            if (operatorsElement != null)
            {
                var operatorElements = operatorsElement.Elements("operator");

                operators = operatorElements.Select(o => operatorParser.Parse(o));
            }

            IEnumerable<AggregateFunction> aggregateFunctions = null;

            var aggregateFunctionsElement = e.Element("aggregateFunctions");

            if (aggregateFunctionsElement != null)
            {
                var aggregateFunctionsElements = aggregateFunctionsElement.Elements("aggregateFunction");

                aggregateFunctions = aggregateFunctionsElements.Select(o => aggregateFunctionParser.Parse(o));
            }

            TypeDescription typeDescription = TypeDescription.Unknown;
            long minValue = 0, maxValue = ushort.MaxValue;
            string pattern = null, patternDescription = null;
            Format defaultFormat = null;

            var typeDescriptionAttribute = e.Attribute("typeDescription");
            var minValueAttribute = e.Attribute("min");
            var maxValueAttribute = e.Attribute("max");
            var patternAttribute = e.Attribute("pattern");
            var patternDescriptionAttribute = e.Attribute("patternDescription");

            var formats = formatElements.Select(o => formatParser.Parse(o));

            if (typeDescriptionAttribute != null)
                typeDescription = (TypeDescription) int.Parse(typeDescriptionAttribute.Value);

            if (minValueAttribute != null)
                minValue = long.Parse(minValueAttribute.Value);

            if (maxValueAttribute != null)
                maxValue = long.Parse(maxValueAttribute.Value);

            if (patternAttribute != null)
                  pattern = patternAttribute.Value;

            if (patternDescriptionAttribute != null)
                patternDescription = patternDescriptionAttribute.Value;

            var defaultFormatIDAttribute = e.Attribute("defaultFormatID");

            if (defaultFormatIDAttribute != null)
                defaultFormat = formats.Single(o => o.ID.ToString() == defaultFormatIDAttribute.Value);

            var lengthAttribute = e.Attribute("length");
            int? length = null;

            if (lengthAttribute != null)
                length = int.Parse(lengthAttribute.Value);

            var viewLengthAttribute = e.Attribute("viewLength");
            int? viewLength = null;

            if (viewLengthAttribute != null)
                viewLength = int.Parse(viewLengthAttribute.Value);
            
            return new Type(nameAttribute.Value, runtimeTypeAttribute.Value, formats, operators, aggregateFunctions, typeDescription, 
                            minValue, maxValue, pattern, patternDescription, defaultFormat, length, viewLength);
#endif
        }
    }
}
