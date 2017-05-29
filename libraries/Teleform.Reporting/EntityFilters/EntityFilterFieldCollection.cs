using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Teleform.Reporting.EntityFilters
{
    [Serializable()]
    public class EntityFilterFieldCollection : IEnumerable<EntityFilterField>, ICollection<EntityFilterField> ,IList<EntityFilterField>
    {
        private readonly List<EntityFilterField> fields = new List<EntityFilterField>();

        public EntityFilterFieldCollection(IEnumerable<EntityFilterField> fields)
        {
            if (fields != null)
            {
                this.fields = fields.OrderBy(f => f.Sequence).ToList();

                for (int i = 0; i < Count; i++)
                    this.fields[i].Sequence = i;
            }
        }

        public EntityFilterField this[int index]
        {
            get
            {
                return fields[index]; 
            }
            set
            {
                fields[index] = value;
                value.Sequence = index;
            }
        }

        public IEnumerator<EntityFilterField> GetEnumerator()
        {
            return fields.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return fields.GetEnumerator();
        }
        public int Count { get { return fields.Count; } }
        public bool IsReadOnly { get { return false; } }

        /// <summary>
        /// Меняет элементы с указанными индексами местами.
        /// </summary>
        /// <param name="firstIndex">Индекс первого элемента.</param>
        /// <param name="secondIndex">Индекс второго элемента.</param>
        public void Permute(int firstIndex, int secondIndex)
        {
            var t = fields[firstIndex];

            fields[firstIndex] = fields[secondIndex];
            fields[firstIndex].Sequence = firstIndex;

            fields[secondIndex] = t;
            t.Sequence = secondIndex;
        }
       
        public void Add(EntityFilterField field)
        {
            field.Sequence = Count;
            fields.Add(field);
        }

        public void Clear()
        {
            fields.Clear();
        }

        public bool Contains(EntityFilterField field)
        {
            return fields.Contains(field);
        }

        public void CopyTo(EntityFilterField[] array, int arrayIndex)
        {
            fields.CopyTo(array, arrayIndex);
        }

        public bool Remove(EntityFilterField field)
        {
            var fieldIndex = fields.IndexOf(field);
            if (fieldIndex > -1)
            {
                fields.Remove(field);
                for (int i = fieldIndex; i < Count; i++)
                    fields[i].Sequence = i;
                return true;
            }
            return false;
        }

        public int IndexOf(EntityFilterField field)
        {
           return fields.IndexOf(field);
        }

        public void AddRange(IEnumerable<EntityFilterField> collection)
        {
            var index = Count;

            fields.AddRange(collection);

            while (index < Count)
                fields[index].Sequence = index++;
        }

        public void InsertRange(int index, IEnumerable<EntityFilterField> collection)
        {
            fields.InsertRange(index, collection);

            while (index < Count)
                fields[index].Sequence = index++;
        }

        public void Insert(int index, EntityFilterField field)
        {
            fields.Insert(index, field);

            while (index < Count)
                fields[index].Sequence = index++;
        }

        public void RemoveAt(int index)
        {
            fields.RemoveAt(index);

            while (index < Count)
                fields[index].Sequence = index++;
        }
    }
}
