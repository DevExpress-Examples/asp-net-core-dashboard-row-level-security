# BI Dashboard for ASP NET.Core - Implement Row-Level Security 

This example shows how you can implement connection filtering in an application, where users share the same application. The application sets the current user ID in [SESSION_CONTEXT](https://learn.microsoft.com/en-us/sql/t-sql/functions/session-context-transact-sql?view=sql-server-ver16&viewFallbackFrom=sql-server-ver16) after connecting to the database, and then security policies filter rows that shouldn't be visible to this ID.

## Example Overview

### Configure a Database

1. This example uses an SQL file ([instnwnd.sql](https://github.com/microsoft/sql-server-samples/blob/master/samples/databases/northwind-pubs/instnwnd.sql)). Execute it to recreate a database on your side. Do not forget to update the connection string in the [appsettings.json](./WebDashboardInterceptors/appsettings.json) and [Program.cs](./WebDashboardInterceptors/Program.cs) files to make it valid in your environment.

2. Execute the script below to configure the execution context to control access to rows in a database table. This script does the following:

- Creates a new schema and predicate function, which uses the user ID stored in SESSION_CONTEXT to filter rows. 
- Creates a security policy that adds this function as a filter predicate and a block predicate on _Orders_.  

```sql
CREATE SCHEMA Security;
GO

CREATE FUNCTION Security.fn_securitypredicate(@EmployeeId int)
    RETURNS TABLE
    WITH SCHEMABINDING
AS
    RETURN SELECT 1 AS fn_securitypredicate_result
    WHERE CAST(SESSION_CONTEXT(N'EmployeeId') AS int) = @EmployeeId;
GO

CREATE SECURITY POLICY Security.OrdersFilter
    ADD FILTER PREDICATE Security.fn_securitypredicate(EmployeeId)
        ON dbo.Orders,
    ADD BLOCK PREDICATE Security.fn_securitypredicate(EmployeeId)
        ON dbo.Orders AFTER INSERT
    WITH (STATE = ON);
GO
```
### Configure the `IDBConnectionInterceptor` Object 

Create a `IDBConnectionInterceptor` object ([RLSConnectionInterceptor.cs](./WebDashboardInterceptors/RLSConnectionInterceptor.cs) in this example) to set the current user ID in SESSION_CONTEXT after opening a connection and simulate the connection filtering by selecting from the _Orders_ table after setting different user IDs in SESSION_CONTEXT.

Register `RLSConnectionInterceptor` in `DashboardConfigurator`.

### Run the Application

When you run the application, the registration form ([Login.cshtml](./WebDashboardInterceptors/Views/Account/Login.cshtml)) appears. 

![Registration form](./Images/loginform.png)

You can select a user to see the dashboard that displays filtered data for the specified user.

![Dashboard](./Images/dashboard.png)

## Files to Review

- [RLSConnectionInterceptor.cs](./WebDashboardInterceptors/RLSConnectionInterceptor.cs)
- [Program.cs](./WebDashboardInterceptors/Program.cs)
- [AccountController.cs](./WebDashboardInterceptors/Controllers/AccountController.cs)
- [Login.cshtml](./WebDashboardInterceptors/Views/Account/Login.cshtml)











