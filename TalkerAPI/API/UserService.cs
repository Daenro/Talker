using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.OrmLite.SqlServer;
using ServiceStack.ServiceHost;
using ServiceStack.Common.Web;

namespace TalkerAPI.API
{
    [Authenticate]
    public class UserService : Service
    {
        Repository repository;

        public UserService()
        {
            repository = new Repository();
        }

        public object Post(SendRecord request)
        {
            int id = repository.AddRecord(request.UserName, request.Value, request.Message);
            return new SendRecordResponse { RecordId = id };
        }

        public object Get(UserRecords request)
        {
            if (request.UserName == null)
            {
                var session = this.Request.GetSession();
                request.UserName = session.UserName;
            }
            List<Record> records = repository.GetRecordsForUser(request.UserName);
            return new UserRecordsResponse { UserName = request.UserName, Records = records };
        }
    }

    [Route("/record/{UserName*}", "GET")]
    public class UserRecords : IReturn<UserRecordsResponse>
    {
        public string UserName { get; set; }
    }

    public class UserRecordsResponse
    {
        public string UserName { get; set; }

        public List<Record> Records { get; set; }


    }

    [Route("/record/{UserName}", "POST")]
    public class SendRecord:IReturn<SendRecordResponse>
    {
        public string UserName { get; set; }
        public byte[] Value { get; set; }
        public string Message { get; set; }
    }

    public class SendRecordResponse
    {
        public int RecordId { get; set; }
    }

    public class UserLoginResponse
    {
        public string Login { get; set; }
    }

    public class Error
    {
        public string ErrorMessage { get; set; }
    }

}