public class UIPlayerStats {
	public int playerNumber;
	public string character;
	public string hitPercentage;
	public string health;

	public UIPlayerStats() {
		this.playerNumber = 0;
		this.character = null;
		this.hitPercentage = null;
		this.health = null;
	}

	public UIPlayerStats(int playerNumber, string character, int hitPercentage, int health) {
		this.playerNumber = playerNumber;
		this.character = character;
		this.hitPercentage = hitPercentage.ToString() + ".00%";
		this.health = health.ToString();
	}

	public static UIPlayerStats CreatePlayerStats(int playerNumber, string character, int hitPercentage, int health) {
		return new UIPlayerStats(playerNumber, character, hitPercentage, health);
	}
}
