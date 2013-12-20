using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using ServiceStack.ServiceInterface.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace TalkerAPI.API
{
    public class Repository
    {
        OrmLiteConnectionFactory dbFactory;

        public Repository()
        {
            dbFactory = new OrmLiteConnectionFactory(@"Server=dd548439-e3bd-49e2-a091-a26800e5e544.sqlserver.sequelizer.com;Database=dbdd548439e3bd49e2a091a26800e5e544;User ID=fbgkewnryveroklo;Password=cadLym2uMEaWn5YBExNAiwX2iCLUtPTfRCodTbNbj3UL2RBcaRkiS8yNkkC4Hgb7;", SqlServerDialect.Provider);
        }

        public int AddRecord(string username, byte[] value, string message = "")
        {
            using (IDbConnection dbConn = dbFactory.OpenDbConnection())
            using (IDbTransaction dbTrans = dbConn.BeginTransaction())
            {
                Record r = new Record { Message = message, UserName = username, Date = DateTime.Now };
                dbConn.Insert<Record>(r);
                int newid = (int)dbConn.GetLastInsertId();
                string path = @"MediaContent\" + newid.ToString() + ".wav";
                r.Value = path;
                r.Id = newid;
                dbConn.Update<Record>(r);
                dbTrans.Commit();
                File.WriteAllBytes(HttpContext.Current.Server.MapPath(@"\"+path), value);
                return newid;
            }
        }

        public List<Record> GetRecordsForUser(string username)
        {
            using (IDbConnection dbConn = dbFactory.OpenDbConnection())
            {
                List<Record> baserec = dbConn.Select<Record>(rec => rec.UserName == username);
                return baserec;
            }
        }
    }

    public class Record
    {
        [AutoIncrement]
        public int Id { get; set; }
        [StringLength(15)]
        public string UserName { get; set; }
        [StringLength(30)]
        public string Message { get; set; }
        public DateTime Date { get; set; }
        [StringLength(100)]
        public string Value { get; set; }
    }
}