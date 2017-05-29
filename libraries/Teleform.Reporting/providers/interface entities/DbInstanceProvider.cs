using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Teleform.Reporting.Providers
{
    public class DbInstanceProvider : IProvider
    {
        XmlDocumentInstanceProvider provider;

        /// <summary>
        /// Возвращает sql-команду для получения объекта.
        /// </summary>
        public SqlCommand Command { get; private set; }

        public DbInstanceProvider(XmlDocumentInstanceProvider provider, SqlCommand command)
        {
            Command = command;
            this.provider = provider;
        }

        public object GetInstance()
        {
            Command.Connection.Open();

            Command.ExecuteScalar();

            throw new NotImplementedException();
        }
    }
}
