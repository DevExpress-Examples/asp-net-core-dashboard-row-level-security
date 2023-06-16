using DevExpress.DataAccess.Sql;
using System.Data;
using WebDashboard.Code;

namespace WebDashboard {
    public class RLSConnectionInterceptor : IDBConnectionInterceptor {
        IHttpContextAccessor contextAccessor;
        public RLSConnectionInterceptor(IHttpContextAccessor contextAccessor) {
            this.contextAccessor = contextAccessor;
        }
        public void ConnectionOpened(string sqlDataConnectionName, IDbConnection connection) {
            int employeeId = contextAccessor.GetCurrentUserId();
            using(var command = connection.CreateCommand()) {
                command.CommandText = $"EXEC sp_set_session_context @key = N'EmployeeId', @value = {employeeId}";
                command.ExecuteNonQuery();
            }
        }

        public void ConnectionOpening(string sqlDataConnectionName, IDbConnection connection) { }
    }
}
