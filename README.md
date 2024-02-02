<!-- default badges list -->
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T1172361)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
# BI Dashboard for ASP NET.Core - Row-Level Security 

This example implements connection filtering for dashboards in multi-user environments. The application sets the current user ID within  [SESSION_CONTEXT](https://learn.microsoft.com/en-us/sql/t-sql/functions/session-context-transact-sql?view=sql-server-ver16&viewFallbackFrom=sql-server-ver16). When the database connection opens, security policies filter visible rows for the current user.

## Example Overview

### Configure a Database

1. This example uses a SQL file ([instnwnd.sql](https://github.com/microsoft/sql-server-samples/blob/master/samples/databases/northwind-pubs/instnwnd.sql)). Execute it to recreate the database locally. Do not forget to update [appsettings.json](./WebDashboardInterceptors/appsettings.json) and [Program.cs](./WebDashboardInterceptors/Program.cs): so that the connection string works in your environment.

2. Execute the script below. The script extends the database as follows:

- Creates a new schema and predicate function, which uses user ID stored in SESSION_CONTEXT to filter rows. 
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

Create an `IDBConnectionInterceptor` object ([RLSConnectionInterceptor.cs](./WebDashboardInterceptors/RLSConnectionInterceptor.cs) in this example). When the database connection opens, store the current user ID in SESSION_CONTEXT. Modify queries for the _Orders_ table - filter data by user ID (so as to implement database behavior equivalent to connection filtering).

Register `RLSConnectionInterceptor` within `DashboardConfigurator`.

### Run the Application

When you run the application, a registration form ([Login.cshtml](./WebDashboardInterceptors/Views/Account/Login.cshtml)) will appear on-screen. 

![Registration form](./Images/loginform.png)

Select a user to generate a dashboard with filtered data.

![Dashboard](./Images/dashboard.png)

## Files to Review

- [RLSConnectionInterceptor.cs](./WebDashboardInterceptors/RLSConnectionInterceptor.cs)
- [Program.cs](./WebDashboardInterceptors/Program.cs)
- [AccountController.cs](./WebDashboardInterceptors/Controllers/AccountController.cs)
- [Login.cshtml](./WebDashboardInterceptors/Views/Account/Login.cshtml)











