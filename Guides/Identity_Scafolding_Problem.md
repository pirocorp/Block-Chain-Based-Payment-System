# Identity Scafolding Problem Throbuleshooting

1.Close Visual Studio.

2.Open a command prompt (powershell) and change directories to the project location.

3.Make sure the aspnet-codegenerator tool is installed on your machine by executing this command:

```
dotnet tool install -g dotnet-aspnet-codegenerator
```

4.Add Microsoft.VisualStudio.Web.CodeGeneration.Design package to the project if it does not already exist in your project.

```
Install-Package Microsoft.VisualStudio.Web.CodeGeneration.Design
```

5.Add Microsoft.EntityFrameworkCore.Design package to the project if it does not already exist in your project.

```
Install-Package Microsoft.EntityFrameworkCore.Design
```

6.Run the following command where YourDbContext.Namespace.ApplicationDbContext is the namespace to your DbContext:

```
dotnet aspnet-codegenerator identity -dc YourDbContext.Namespace.ApplicationDbContext
```

dotnet aspnet-codegenerator also has the ability to scaffold only specific files versus all the Identity files if you don’t need the full set by passing in the -files parameter followed by the files you want to create.

```
dotnet aspnet-codegenerator identity -dc ourDbContext.Namespace.ApplicationDbContext –files “Account.Register;Account.Login;Account.Logout”
```