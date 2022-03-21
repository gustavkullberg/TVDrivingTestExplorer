using Orleans;
using Orleans.Hosting;
using DrivingTestExplorer;

await Host.CreateDefaultBuilder(args)
    .UseOrleans(builder =>
    {
        builder.UseLocalhostClustering();
        builder.AddMemoryGrainStorageAsDefault();
        builder.AddSimpleMessageStreamProvider("SMS");
        builder.AddMemoryGrainStorage("PubSubStore");
        builder.UseDashboard();
    })
    .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>())
    .RunConsoleAsync();