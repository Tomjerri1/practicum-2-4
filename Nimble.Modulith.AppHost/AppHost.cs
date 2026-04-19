var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .AddDatabase("UsersDb");

builder.AddProject<Projects.Nimble_Modulith_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(sql);

builder.Build().Run();