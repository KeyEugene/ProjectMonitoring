#define Alex

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Teleform.Reporting
{
    [Serializable()]
    public class TemplateFieldCollection : IList<TemplateField>, ICollection<TemplateField>, IEnumerable<TemplateField>, IEnumerable
    {
        private readonly List<TemplateField> fields;

        public TemplateFieldCollection()
        {
            fields = new List<TemplateField>();
        }

        public TemplateFieldCollection(IEnumerable<TemplateField> fields)
        {
            if (fields != null)
            {
                this.fields = fields.OrderBy(f => f.Order).ToList();

#if Alex // Work with very HardTemplate
                if (fields.Count() != 0)
                {
                    var maxLevel = fields.Max(x => x.Level);

                    for (int i = 1; i < (maxLevel + 1); i++)
                    {
                        RenumberOrder(i);
                    }
                }
#else
                for (int i = 0; i < Count; i++)
                    this.fields[i].Order = i;
#endif
            }

        }

        public IEnumerator<TemplateField> GetEnumerator()
        {
            return fields.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return fields.GetEnumerator();
        }

        public int Count { get { return fields.Count; } }

        public bool IsReadOnly { get { return false; } }


        public void Add(TemplateField field)
        {
            field.Order = Count;

            fields.Add(field);
        }

        public void Clear()
        { fields.Clear(); }

        public bool Contains(TemplateField field)
        { return fields.Contains(field); }

        public void CopyTo(TemplateField[] array, int arrayIndex)
        {
            fields.CopyTo(array, arrayIndex);
        }

        public bool Remove(TemplateField field)
        {
            var fieldIndex = fields.IndexOf(field);

            if (fieldIndex > -1)
            {
                fields.Remove(field);

                for (int i = fieldIndex; i < Count; i++)
                    fields[i].Order = i;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Меняет элементы с указанными индексами местами.
        /// </summary>
        /// <param name="firstIndex">Индекс первого элемента.</param>
        /// <param name="secondIndex">Индекс второго элемента.</param>
        public void Permute(int firstIndex, int secondIndex)
        {
            var t = fields[firstIndex];

            fields[firstIndex] = fields[secondIndex];
            fields[firstIndex].Order = firstIndex;

            fields[secondIndex] = t;
            t.Order = secondIndex;
        }

        public TemplateField this[int index]
        {
            get { return fields[index]; }
            set
            {
                fields[index] = value;
                value.Order = index;
            }
        }

        public int IndexOf(TemplateField field)
        {
            return fields.IndexOf(field);
        }

        public void AddRange(IEnumerable<TemplateField> collection)
        {
            var index = Count;

            fields.AddRange(collection);

            while (index < Count)
                fields[index].Order = index++;
        }

        public void Insert(int index, TemplateField field)
        {
            fields.Insert(index, field);

            while (index < Count)
                fields[index].Order = index++;
        }

        public void InsertRange(int index, IEnumerable<TemplateField> collection)
        {
            fields.InsertRange(index, collection);

            while (index < Count)
                fields[index].Order = index++;
        }

        public void RemoveAt(int index)
        {
            fields.RemoveAt(index);

            while (index < Count)
                fields[index].Order = index++;
        }



#region Overridden Methods

        private void RenumberOrder(int level)
        {
            int j = 0;

            for (int i = 0; i < fields.Count(); i++)
            {
                if (fields[i].Level == level)
                {
                    fields[i].Order = j++;
                }
            }
        }

        public void Add(TemplateField field, int Level)
        {
            field.Level = Level;
            var t = (List<TemplateField>)fields.Select(x => x.Level == Level);
            field.Order = t.Max(x => x.Order);// x.Level == Level).Value;
            fields.Add(field);
        }

        /// <summary>
        /// Меняет элементы с указанными индексами местами.
        /// </summary>
        /// <param name="firstIndex">Индекс первого элемента.</param>
        /// <param name="secondIndex">Индекс второго элемента.</param>
        public void Permute(int level, int firstIndex, int secondIndex)
        {
            for (int i = 0; i < fields.Count(); i++)
            {
                TemplateField t;
                if (fields[i].Level == level && fields[i].Order == firstIndex)
                {
                    t = fields[i];
                    for (int j = 0; j < fields.Count(); j++)
                    {
                        if (fields[j].Level == level && fields[j].Order == secondIndex)
                        {
                            fields[i] = fields[j];
                            fields[j] = t;

                            RenumberOrder(level);

                            return;
                        }
                    }
                }
            }
        }

        public void InsertRange(int level, int index, IEnumerable<TemplateField> collection)
        {
            var z = fields.Max(x => x.Level);

            for (int i = 1; i < (z + 1); i++)
            {
                RenumberOrder(i);
            }

            var list = collection.ToList();

            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i].Level == level && fields[i].Order == (index - 1))
                {
                    for (int j = 0; j < list.Count(); j++)
                    {
                        list[j].Order = index++;
                        list[j].Level = level;
                    }
                    fields.InsertRange(i, collection);

                    return;
                }
            }
        }

        public void AddRange(IEnumerable<TemplateField> collection, int level)
        {
            fields.AddRange(collection);

            if (collection.Count() == 1)
            {
                var j = fields.Count() - 1;
                fields[j].Level = level;

            } else
            {
                var count = collection.Count();
                var allCount = fields.Count();
                for (int i = allCount - count; i < fields.Count(); i++)
                {
                    fields[i].Level = level;
                }
            }

            RenumberOrder(level);
        }

#warning Особое внимание тутотчик
        public TemplateField this[int level, int index]
        {
            get
            { return fields.FirstOrDefault(x => x.Level == level && x.Order == index); }
            set
            {
                for (int i = 0; i < fields.Count(); i++)
                {
                    if (fields[i].Level == level && fields[i].Order == index)
                    {
                        fields[i] = value;
                        break;
                    }
                }
            }
        }

        public bool Remove(TemplateField field, bool isLevel)
        {
            if (isLevel)
            {
                var fieldIndex = fields.IndexOf(field);

                if (fieldIndex > -1)
                {
                    fields.Remove(field);

                    //Проверяем, на наличие "пустых " левелов
                    var f = fields.Where(x => x.Level == field.Level).ToList();

                    if (f.Count == 0)
                    {
                        for (int i = 0; i < fields.Count; i++)
                            if (fields[i].Level > field.Level)  --fields[i].Level;
                    }

                    RenumberOrder(field.Level);

                    return true;
                }
            }
            return false;
        }

#endregion
    }
}
