# ChatRoom
ChatRoom Kata in C#.

## Notes
- This project uses the [Controller-based API template](https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-9.0).
- [Since .NET Core 9.0 Swagger is no longer supported](https://github.com/dotnet/aspnetcore/issues/54599). So instead, we use [Scalar instead](https://dev.to/eminvergil/an-alternative-to-swagger-in-dotnet-9-2jd6). 

```sh
dotnet add package Scalar.AspNetCore
```

You can check the docs in `/scalar/v1`.

- This project uses Postgres as its DB. See:
    - https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app
    - https://www.npgsql.org/efcore
    - Please host a postgres server using docker(note that you should set the host as `localhost`, not the ip address of the docker container)

- DB migrations is required before testing
    - install the following package: `dotnet add package Microsoft.EntityFrameworkCore.Design`
    - refer to: https://learn.microsoft.com/en-us/ef/core/managing-schemas/
