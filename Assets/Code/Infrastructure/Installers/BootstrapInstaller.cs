using Code.Gameplay.Common.Random;
using Code.Gameplay.Common.Time;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.ConfigsManagement;
using Zenject;

namespace Code.Infrastructure.Installers
{
	public class BootstrapInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			BindCommonServices();
			BindInfrastructureServices();
		}

		private void BindCommonServices()
		{
			Container.BindInterfacesTo<UnityTimeService>().AsSingle();
			Container.BindInterfacesTo<UnityRandomService>().AsSingle();
		}

		private void BindInfrastructureServices()
		{
			Container.BindInterfacesTo<AssetsService>().AsSingle();
			Container.BindInterfacesTo<ConfigsService>().AsSingle();
		}
	}
}
