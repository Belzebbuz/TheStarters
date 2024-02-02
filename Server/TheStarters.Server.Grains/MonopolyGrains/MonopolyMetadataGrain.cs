using Orleans.Concurrency;
using TheStarters.Server.Abstractions.Monopoly;
using TheStarters.Server.Abstractions.Monopoly.Models;
using TheStarters.Server.Abstractions.Monopoly.Models.Abstractions;
using TheStarters.Server.Abstractions.Monopoly.Models.Lands;

namespace TheStarters.Server.Grains.MonopolyGrains;

[StatelessWorker]
public class MonopolyMetadataGrain : Grain, IMonopolyMetadataGrain
{
	private readonly MonopolyBoardConfig _config = new();

	private readonly byte[] _defaultMap =
	[
		0, 7, 2, 7, 4, 8, 7, 1, 7, 7, 6, 7, 1, 7, 7, 8, 7, 2, 7, 7, 5, 7, 1, 7, 7, 8, 7, 7, 2, 7, 3, 7, 7, 1, 7, 8, 2,
		7, 4, 7
	];
	public MonopolyMetadataGrain()
	{
		foreach (var typeId in _defaultMap)
		{
			_config.Lands.Add(_config.LandTypeMap[typeId]);
		}
	}

	public ValueTask<Dictionary<byte, MonopolyLand>> ConfigureBoardAsync()
	{
		byte index = 1;
		var cityIndex = 1;
		var city = Faker.Address.City();
		var streetCost = 60;
		var houseCost = 50;
		var board = new Dictionary<byte, MonopolyLand>(_config.Lands.Count);
		foreach (var landType in _config.Lands)
		{
			if (index == 1 && landType != typeof(GoLand))
				throw new InvalidOperationException("Неверно сконфигурирована карта, обратитесь к администратору.");
			
			if (landType == typeof(StreetLand))
			{
				if (cityIndex % 6 == 0)
					houseCost += 50;
				if (index % 2 == 0)
					streetCost += 20;
				var land = new StreetLand(index, Faker.Address.StreetName(), (short)(streetCost + cityIndex * 10), (short)houseCost, city);
				cityIndex++;
				if (cityIndex % 3 == 0)
					city = Faker.Address.StreetName();
				board.Add(index,land);
			}
			else
			{
				if(Activator.CreateInstance(landType, index) is not MonopolyLand defaultLand)
					throw new InvalidOperationException("Неверно сконфигурирована карта, обратитесь к админстриатору.");

				board.Add(index,defaultLand);
			}
			
			index++;
		}
		return ValueTask.FromResult(board);
	}

	public ValueTask<MonopolyCommand[]> GetChanceCommandsAsync()
	{
		throw new NotImplementedException();
	}
}