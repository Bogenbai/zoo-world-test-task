using System;
using System.Collections.Generic;
using Code.Gameplay.Common.Random;
using Code.Gameplay.Features.Movement.Configs;
using Code.Gameplay.Features.Movement.Services;
using Code.Infrastructure;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using UnityEditor;
using Zenject;

namespace Code.Gameplay.Features.Movement.Editor.EditModeTests
{
	[Category(TestCategories.Gameplay)]
	public class MovementStrategyFactoryTests
	{
		private MovementStrategyFactory _factory;

		[SetUp]
		public void SetUp()
		{
			DiContainer container = new();
			container.Bind<IRandomService>().FromInstance(Substitute.For<IRandomService>());

			_factory = new MovementStrategyFactory(container);
		}

		[Test]
		public void EveryMovementConfigType_ShouldHaveStrategy()
		{
			List<string> unsupported = new();

			foreach (Type configType in TypeCache.GetTypesDerivedFrom<MovementConfig>())
			{
				if (configType.IsAbstract)
					continue;

				MovementConfig config = (MovementConfig)Activator.CreateInstance(configType);

				try
				{
					_factory.CreateStrategy(config);
				}
				catch (ArgumentException)
				{
					unsupported.Add(configType.Name);
				}
			}

			unsupported.Should().BeEmpty("every movement config type needs a case in MovementStrategyFactory");
		}

		[Test]
		public void NullConfig_ShouldThrow()
		{
			Action create = () => _factory.CreateStrategy(null);

			create.Should().Throw<ArgumentNullException>("an animal without a movement config cannot move");
		}
	}
}
