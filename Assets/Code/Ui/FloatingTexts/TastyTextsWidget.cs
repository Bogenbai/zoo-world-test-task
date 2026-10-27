using System.Collections.Generic;
using Code.Gameplay.Common.Time;
using Code.Gameplay.Features.Animals.Behaviours;
using Code.Gameplay.Features.Animals.Services;
using Code.Gameplay.Features.FoodChain.Services;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.Pooling;
using Code.Ui.FloatingTexts.Data;
using Code.Ui.FloatingTexts.Services;
using UnityEngine;
using Zenject;

namespace Code.Ui.FloatingTexts
{
	public class TastyTextsWidget : MonoBehaviour
	{
		[SerializeField] private AssetReferencePrefab<FloatingText> textPrefab;

		[SerializeField] private string message = "Tasty!";

		[SerializeField, Min(0f)]
		private float lifetime = 1f;

		[SerializeField]
		[Tooltip("Shift on screen in pixels, so the text sits under the predator instead of covering it.")]
		private Vector2 screenOffset = new(0f, -30f);

		private readonly List<FloatingTextEntry> _texts = new();

		private IPredationQuery _predationQuery;
		private IAnimalDeathQuery _animalDeathQuery;
		private IFloatingTextFactory _floatingTextFactory;
		private IPrefabPool _prefabPool;
		private ITimeService _timeService;
		private Camera _camera;

		[Inject]
		private void Construct(
			IPredationQuery predationQuery,
			IAnimalDeathQuery animalDeathQuery,
			IFloatingTextFactory floatingTextFactory,
			IPrefabPool prefabPool,
			ITimeService timeService,
			Camera mainCamera)
		{
			_predationQuery = predationQuery;
			_animalDeathQuery = animalDeathQuery;
			_floatingTextFactory = floatingTextFactory;
			_prefabPool = prefabPool;
			_timeService = timeService;
			_camera = mainCamera;
		}

		private void OnEnable()
		{
			_predationQuery.OnPredatorAte += HandlePredatorAte;
			_animalDeathQuery.OnAnimalDied += HandleAnimalDied;
		}

		private void OnDisable()
		{
			_predationQuery.OnPredatorAte -= HandlePredatorAte;
			_animalDeathQuery.OnAnimalDied -= HandleAnimalDied;
		}

		private void Update()
		{
			for (int i = _texts.Count - 1; i >= 0; i--)
			{
				FloatingTextEntry entry = _texts[i];
				entry.TimeLeft -= _timeService.DeltaTime;

				if (entry.TimeLeft <= 0f)
					Release(i);
				else
					PlaceUnderTarget(entry);
			}
		}

		private void HandlePredatorAte(Animal predator)
		{
			FloatingText text = _floatingTextFactory.CreateText(textPrefab, transform);
			text.SetText(message);

			FloatingTextEntry entry = new(predator, text, lifetime);
			_texts.Add(entry);
			PlaceUnderTarget(entry);
		}

		private void HandleAnimalDied(Animal animal)
		{
			for (int i = _texts.Count - 1; i >= 0; i--)
			{
				if (_texts[i].Target == animal)
					Release(i);
			}
		}

		private void PlaceUnderTarget(FloatingTextEntry entry)
		{
			Vector2 screenPosition = _camera.WorldToScreenPoint(entry.Target.transform.position);
			entry.Text.SetScreenPosition(screenPosition + screenOffset);
		}

		private void Release(int index)
		{
			_prefabPool.Put(textPrefab.AssetGUID, _texts[index].Text);
			_texts.RemoveAt(index);
		}
	}
}
