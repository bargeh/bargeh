namespace Users.API.Models;

public class User
{
	public Guid Id { get; set; } = Guid.NewGuid ();
	public required string Username { get; set; }
	public required string DisplayName { get; set; }
	public required string PhoneNumber { get; set; }
	public string? Password { get; set; }
	public string Bio { get; set; } = string.Empty;
	public string ProfileImage { get; set; } = "Default.webp";
	public string CoverImage { get; set; } = "Cover.webp";
	public ushort PremiumDaysLeft { get; set; } = 0;

	public DateTime OnlineDate { get; set; } = DateTime.Now;
	public DateTime RegisterDate { get; set; } = DateTime.Now;
	public required string VerificationCode { get; set; }
	public bool Enabled { get; set; } = true;
	public bool CanCreateForums { get; set; } = true;

	public string? Email { get; set; }
	public bool? IsMale { get; set; }
	public Province? Province { get; set; }
	public DateTime? BirthDate { get; set; }
}

public enum Province
{
	Alborz,
	Ardabil,
	Bushehr,
	ChaharmahalAndBakhtiari,
	EastAzerbaijan,
	Fars,
	Gilan,
	Golestan,
	Hamadan,
	Hormozgan,
	Ilam,
	Isfahan,
	Kerman,
	Kermanshah,
	KhorasanRazavi,
	Khuzestan,
	KohgiluyehAndBoyerAhmad,
	Kurdistan,
	Lorestan,
	Markazi,
	Mazandaran,
	NorthKhorasan,
	Qazvin,
	Qom,
	Semnan,
	SistanAndBaluchestan,
	SouthKhorasan,
	Tehran,
	WestAzerbaijan,
	Yazd,
	Zanjan
}