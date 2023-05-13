using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using TMPro;

namespace PokemonDisplay
{
	public class PokemonDisplay : MonoBehaviour
	{
		public List<string> pokemons = new List<string>();
		public int pokemonIndex;
		public string nextPokemonDisplaySceneName;

		[SerializeField]
		private RawImage _rawImage;

		[SerializeField]
		private TMP_Text _nameText;

		[SerializeField]
		private TMP_Text _expText;

		[SerializeField]
		private TMP_Text _weightText;

		[SerializeField]
		private TMP_Text _typeText;

		[SerializeField]
		private Button _prevButton;

		[SerializeField]
		private Button _nextButton;

		[SerializeField]
		private Button _nextPokemonDisplaySceneButton;

		private IEnumerator GetPokemonRoutine(string name)
		{
			// Get info
			string pokeApiUrl = "https://pokeapi.co/api/v2/pokemon";
			string fullPokeApiUrl = $"{pokeApiUrl}/{name}";
			
			UnityWebRequest pokeApiUrlGet = UnityWebRequest.Get(fullPokeApiUrl);
			yield return pokeApiUrlGet.SendWebRequest();

			if(pokeApiUrlGet.result == UnityWebRequest.Result.ConnectionError || 
				pokeApiUrlGet.result == UnityWebRequest.Result.DataProcessingError) {
				yield break;
			}

			dynamic pokeInfo = JsonConvert.DeserializeObject<dynamic>(
				pokeApiUrlGet.downloadHandler.text);

			// Get sprite
			string pokeSpriteURL = pokeInfo["sprites"]["front_default"];

			UnityWebRequest pokeApiSpriteGet = UnityWebRequestTexture.GetTexture(pokeSpriteURL);
			yield return pokeApiSpriteGet.SendWebRequest();

			if(pokeApiSpriteGet.result == UnityWebRequest.Result.ConnectionError ||
				pokeApiSpriteGet.result == UnityWebRequest.Result.DataProcessingError) {
				yield break;
			}

			// Set all details
			string pokeName = pokeInfo["name"];
			string pokeEXP = pokeInfo["base_experience"];
			string pokeWeight = pokeInfo["weight"];
			string pokeType = pokeInfo["types"][0]["type"]["name"];

			_nameText.text = $"Name: {pokeName}";
			_expText.text = $"EXP: {pokeEXP}";
			_weightText.text = $"Weight: {pokeWeight}";
			_typeText.text = $"Type: {pokeType}";
			_rawImage.texture = DownloadHandlerTexture.GetContent(pokeApiSpriteGet);
			_rawImage.texture.filterMode = FilterMode.Point;
		}

		private void OnPrevButtonClicked()
		{
			if(--pokemonIndex < 0) { pokemonIndex = pokemons.Count - 1; }
			StartCoroutine(GetPokemonRoutine(pokemons[pokemonIndex]));
		}

		private void OnNextButtonClicked()
		{
			if(++pokemonIndex >= pokemons.Count) { pokemonIndex = 0; }
			StartCoroutine(GetPokemonRoutine(pokemons[pokemonIndex]));
		}
		
		private void OnNextPokemonDisplaySceneButtonClicked()
		{
			SceneManager.LoadScene(nextPokemonDisplaySceneName);
		}

		private void Start()
		{
			StartCoroutine(GetPokemonRoutine(pokemons[pokemonIndex]));
		}

		private void OnEnable()
		{
			_prevButton.onClick.AddListener(OnPrevButtonClicked);
			_nextButton.onClick.AddListener(OnNextButtonClicked);
			_nextPokemonDisplaySceneButton.onClick.AddListener(OnNextPokemonDisplaySceneButtonClicked);
		}

		private void OnDisable()
		{
			_prevButton.onClick.RemoveListener(OnPrevButtonClicked);
			_nextButton.onClick.RemoveListener(OnNextButtonClicked);
			_nextPokemonDisplaySceneButton.onClick.RemoveListener(OnNextPokemonDisplaySceneButtonClicked);
		}
	}
}
