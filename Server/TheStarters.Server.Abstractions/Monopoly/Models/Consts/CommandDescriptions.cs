namespace TheStarters.Server.Abstractions.Monopoly.Models.Consts;

public static class CommandDescriptions
{
	public const string BuyStreet = "Купить улицу";
	public const string BuyStation = "Купить ж/д станцию";
	public const string CancelBuy = "Отказаться от покупки";
	public const string DowngradeStreet = "Понизить кол-во домов на улице на 1";
	public const string UpgradeStreet = "Повысить кол-во домов на улице на 1";
	public const string GoToPrison = "Отправиться с тюрьму";
	public const string Move = "Бросить кубики и сделать ход";
	public const string PayForFree = "Заплатить 50 за выход из тюрьмы";
	public const string PayIncomeTax = "Заплатить 200 подоходного налога";

	public static string PayRent(short value) => $"Заплатить {value} за аренду.";
	public const string SellLand = "Продать недвижимость";
	public const string TakeChance = "Шанс";
	public const string TakeCommunityChest = "Общественная казна";
	public const string ThrowDice = "Бросить кубики";
	public const string ChairmanBoardOfDirectors = "Вас избрали председателем совета директоров. Заплатите каждому игроку по 50.";
	public const string GoToGoLand = "Отправляйтесь на поле \"Вперед\". Получите 200";
	public const string GoToRandomStreet = "Отправляйтесь на случайную улицу.";
	public const string GoToStation = "Отправляйтесь на случайную станцию.";
	public const string MajorRenovation = "Подошло время капитального ремонта вашей собственности. Заплатите за каждый дом по 25. Заплатите за каждый отель по 100";
	public const string MajorRenovationChest = "Подошло время капитального ремонта вашей собственности. Заплатите за каждый дом по 40. Заплатите за каждый отель по 115";
	public const string SpeedingFine = "Оплатите штраф за превышение скорости 15";
	public const string TakeDividends = "Банк платит вам дивиденды в размере 50";
	public const string LifeInsurance = "Наступил срок исполнения платежа по страхованию жизни. Получите 100";
	public const string Inheritance = "Вы получаете наследство 100";
	public const string Fond = "Наступил срок исполнения платежа по отпускному фонду. Получите 100";
	public const string IncomeTaxRefund = "Возмещение подоходного налога. Получите 20";
	public const string BankError = "Банковская ошибку в вашу пользу. Получите 200";
	public const string HospitalizationExpenses = "Оплатите расходы на госпитализацию в размере 100";
	public const string DoctorVisit = "Оплатите визит к врачу 50";
	public const string StudyPayment = "Оплатите обучение в размере 50";
	public const string BirthdayGift = "Сегодня ваш день рождения. Получите по 10 от каждого игрока";
}