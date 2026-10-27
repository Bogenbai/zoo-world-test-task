using Code.Gameplay.Features.Animals.Services;
using Code.Gameplay.Features.Movement.Services;
using Code.Gameplay.Features.WorldBounds.Services;
using Code.Infrastructure.Pooling;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Installers
{
	public class GameplayInstaller : MonoInstaller
	{
		[SerializeField] private Camera mainCamera;

		public override void InstallBindings()
		{
			BindPoolingServices();
			BindWorldServices();
			BindMovementServices();
			BindAnimalServices();
		}

		private void BindPoolingServices()
		{
			// Pooled instances are scene objects, so the pool has to die with the scene
			Container.BindInterfacesTo<PrefabPool>().AsSingle();
		}

		private void BindWorldServices()
		{
			Container.BindInstance(mainCamera);
			Container.BindInterfacesTo<CameraWorldBoundsService>().AsSingle();
		}

		private void BindMovementServices()
		{
			Container.BindInterfacesTo<MovementStrategyFactory>().AsSingle();
		}

		private void BindAnimalServices()
		{
			Container.BindInterfacesTo<AnimalFactory>().AsSingle();
			Container.BindInterfacesTo<AnimalRegistry>().AsSingle();
		}
	}
}
