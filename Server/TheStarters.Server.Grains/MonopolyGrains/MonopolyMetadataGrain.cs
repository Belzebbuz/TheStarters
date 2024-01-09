using Orleans.Concurrency;
using TheStarters.Server.Abstractions.Monopoly;
using TheStarters.Server.Abstractions.Monopoly.Models;
using TheStarters.Server.Abstractions.Monopoly.Models.Lands;

namespace TheStarters.Server.Grains.MonopolyGrains;

[StatelessWorker]
public class MonopolyMetadataGrain : IMonopolyMetadataGrain
{
	private readonly MonopolyBoardConfig _config = new();

	public MonopolyMetadataGrain()
	{
		_config.Lands.Add(typeof(StartLand));
		for (var i = 0; i < 10; i++)
		{
			_config.Lands.Add(typeof(CompanyLand));
		}
	}

	public ValueTask<Dictionary<byte, MonopolyLand>> ConfigureBoardAsync()
	{
		byte index = 1;
		var board = new Dictionary<byte, MonopolyLand>(_config.Lands.Count);
		foreach (var landType in _config.Lands)
		{
			var land = default(MonopolyLand);
			if (index == 1 && landType != typeof(StartLand))
				throw new InvalidOperationException("Неверно сконфигурирована карта, обратитесь к администратору.");
			if (landType == typeof(StartLand))
				land = new StartLand(index);
			if (landType == typeof(CompanyLand))
				land = new CompanyLand(index, Faker.Company.Name(), (ushort)(10 * index), Faker.Address.StreetName());
			
			if(land is null)
				throw new InvalidOperationException("Неверно сконфигурирована карта, обратитесь к админстриатору.");

			board.Add(index,land);
			index++;
		}
		return ValueTask.FromResult(board);
	}
}