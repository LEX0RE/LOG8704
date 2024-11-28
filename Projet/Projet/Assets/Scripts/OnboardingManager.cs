using System.Collections.Generic;
using UnityEngine;

public class OnboardingManager : MonoBehaviour
{
	[SerializeField]
	public GameObject continueButton;

	[SerializeField]
	public GameObject skipButton;

	[SerializeField]
	public GameObject endButton;

	[SerializeField]
	public List<GameObject> cardList = new();

	private int currentIndex = 0;

	public void NextCard()
	{
		if (currentIndex < cardList.Count)
		{
			this.currentIndex++;
			this.ShowCard();
		}
		else
		{
			this.EndOnboarding();
		}
	}

	public void EndOnboarding()
	{
		currentIndex = 0;
		this.CloseAll();
	}

	void Start()
	{
		this.ShowCard();
	}

	void ShowCard()
	{
		this.CloseAll();

		cardList[currentIndex].SetActive(true);

		this.continueButton.SetActive(currentIndex < cardList.Count - 1);
		this.skipButton.SetActive(currentIndex < cardList.Count - 1);
		this.endButton.SetActive(currentIndex >= cardList.Count - 1);
	}

	void CloseAll()
	{
		foreach (GameObject card in cardList)
		{
			card.SetActive(false);
		}

		this.continueButton.SetActive(false);
		this.skipButton.SetActive(false);
		this.endButton.SetActive(false);
	}
}
