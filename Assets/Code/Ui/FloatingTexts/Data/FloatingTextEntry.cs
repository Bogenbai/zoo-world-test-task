using Code.Gameplay.Features.Animals.Behaviours;

namespace Code.Ui.FloatingTexts.Data
{
	public class FloatingTextEntry
	{
		public Animal Target { get; }
		public FloatingText Text { get; }
		public float TimeLeft { get; set; }

		public FloatingTextEntry(Animal target, FloatingText text, float timeLeft)
		{
			Target = target;
			Text = text;
			TimeLeft = timeLeft;
		}
	}
}
