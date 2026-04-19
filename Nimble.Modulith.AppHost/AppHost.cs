var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql");
var usersDb = sql.AddDatabase("UsersDb");
var productsDb = sql.AddDatabase("ProductsDb");

builder.AddProject<Projects.Nimble_Modulith_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(usersDb)
    .WithReference(productsDb)
    .WaitFor(usersDb)
    .WaitFor(productsDb);

builder.Build().Run();