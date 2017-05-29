#define Alex

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Teleform.Reporting
{
    [Serializable()]
    public class FieldCollection<T> //:  IList<TemplateField>, ICollection<TemplateField>, IEnumerable<TemplateField>, IEnumerable
    {
        private readonly List<T> fields;

        public FieldCollection()
        {
            fields = new List<T>();
        }

        public FieldCollection(IEnumerable<T> fields)
        {
#if Alex
            if (fields != null)
            {
                //this.fields = fields.OrderBy(f => f.Order).ToList();

               // for (int i = 0; i < Count; i++)
                  //  this.fields[i].Order = i;
            }
#else
            this.fields = fields.OrderBy(f => f.Order).ToList();

            for (int i = 0; i < Count; i++)
                this.fields[i].Order = i;
#endif
        }

        public IEnumerator<T> GetEnumerator()
        {
            return fields.GetEnumerator();
        }
        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return fields.GetEnumerator();
        //}

        public int Count { get { return fields.Count; } }

        public bool IsReadOnly { get { return false; } }


        public void Add(T field)
        {
           // field.Order = Count;

            fields.Add(field);
        }

        public void Clear()
        { fields.Clear(); }

        public bool Contains(T field)
        { return fields.Contains(field); }

        public void CopyTo(T[] array, int arrayIndex)
        {
            fields.CopyTo(array, arrayIndex);
        }

        public bool Remove(T field)
        {
            var fieldIndex = fields.IndexOf(field);

            if (fieldIndex > -1)
            {
                fields.Remove(field);

               // for (int i = fieldIndex; i < Count; i++)
                   // fields[i].Order = i;

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
           // fields[firstIndex].Order = firstIndex;

            fields[secondIndex] = t;
           // t.Order = secondIndex;
        }

        public T this[int index]
        {
            get { return fields[index]; }
            set
            {
                fields[index] = value;
                //value.Order = index;
            }
        }

        public int IndexOf(T field)
        {
            return fields.IndexOf(field);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            var index = Count;

            fields.AddRange(collection);

            //while (index < Count)
               // fields[index].Order = index++;
        }

        public void Insert(int index, T field)
        {
            fields.Insert(index, field);

          //  while (index < Count)
               // fields[index].Order = index++;
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            fields.InsertRange(index, collection);

           // while (index < Count)
              //  fields[index].Order = index++;
        }

        public void RemoveAt(int index)
        {
            fields.RemoveAt(index);

           // while (index < Count)
              //  fields[index].Order = index++;
        }
    }
}
