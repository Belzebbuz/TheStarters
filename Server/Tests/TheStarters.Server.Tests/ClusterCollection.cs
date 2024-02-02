namespace TheStarters.Server.Tests;

[CollectionDefinition(Name)]
public class ClusterCollection : ICollectionFixture<ClusterFixture>
{
	public const string Name = "TheStartersCluster";
}