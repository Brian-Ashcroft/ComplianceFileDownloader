using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplianceFileDownloader
{
    public class queryDto
    {
        public int Count { get; }
        public string Sql { get; }

        public queryDto(int count, string sql)
        {
            Count = count;
            Sql = sql;
        }
    }
}
