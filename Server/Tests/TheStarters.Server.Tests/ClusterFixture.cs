using Orleans.TestingHost;

namespace TheStarters.Server.Tests;

public class ClusterFixture : IDisposable
{
	public ClusterFixture()
	{
		var builder = new TestClusterBuilder();
		builder.AddSiloBuilderConfigurator<TestSiloConfigurations>();
		Cluster = builder.Build();
		Cluster.Deploy();
	}

	public void Dispose()
	{
		Cluster.StopAllSilos();
	}

	public TestCluster Cluster { get; }

	public class TestSiloConfigurations : ISiloConfigurator
	{
		public void Configure(ISiloBuilder siloBuilder)
		{
			siloBuilder.ConfigureServices(services =>
				{
				})
				.AddMemoryGrainStorageAsDefault()
				.AddMemoryGrainStorage("TheStartersGrainStorage");
		}
	}
}