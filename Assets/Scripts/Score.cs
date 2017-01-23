
using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour
{
	public const int MAXPOINTS = 100;
	public int score = 0;			// The player's score.

	public enum characterClassStatus 	{ 
					scavengerStatus, 
					brawlerStatus, 
					speedrunnerStatus,
					neutralStatus,
					completionistStatus};	// Enum types for the class of player the gamer can take on


	private GGJPlayerControl playerControl;	// Reference to the player control script.
	private int previousScore = 0;			// The score in the previous frame.
	private static float previousScavengerPts,
				previousBrawlerPts,
				previousSpeedrunnerPts = 0.0f; // The experience points in the previous frame.
	private PlayerHealth playerHealth;

	private static float 	scavengerPts, 
						brawlerPts, 
						speedrunnerPts = 0.0f;	// Tracks the weights dictating the characters character class type. 

	protected static characterClassStatus 
		primaryClass, secondaryClass = characterClassStatus.neutralStatus;// Tracks the characters primary and secondary class types.

	//--------------------------------------------------------------------------------------------------------------------------//

	void Awake ()
	{
		// Setting up the reference.
		playerControl = GameObject.FindGameObjectWithTag("Player").GetComponent<GGJPlayerControl>();
		playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
	}


	void Update ()
	{
		// Set the score text.
		//guiText.text = "Score: " + score;

		// If the score has changed...
		if (previousScore != score)
			// ... play a taunt.
			playerControl.StartCoroutine (playerControl.Taunt ());

		// Set the previous score to this frame's score.
		previousScore = score;

		// If there are no experience points on the board...
		if (previousBrawlerPts == 0.0 && previousScavengerPts == 0.0 && previousSpeedrunnerPts == 0.0) {
			// set the primary and secondary class status to neutral.
			primaryClass = characterClassStatus.neutralStatus;
			secondaryClass = characterClassStatus.neutralStatus;
		}
		// If the experience points have changed...
		else if (previousBrawlerPts != brawlerPts || 
			previousScavengerPts != scavengerPts || 
			previousSpeedrunnerPts != speedrunnerPts) {
			// If brawler points have the greatest value of all experience point classes...
			if (brawlerPts > scavengerPts && brawlerPts > speedrunnerPts) {
				// set primary class to Brawler.
				primaryClass = characterClassStatus.brawlerStatus;

				playerHealth.strengthOffset += 30f;
				playerHealth.attackSpeedOffset += 4f;
				playerHealth.speedOffset += - 2f;

				// If scavenger points have the second greatest value of all experience point classes...
				if (scavengerPts > speedrunnerPts){
					// set secondary class to Scavenger,
					secondaryClass = characterClassStatus.scavengerStatus;

					playerHealth.jumpOffset += 500f;
					playerHealth.lightRadiusOffset += 15;
				}
				// otherwise set secondary class to Speedrunner.
				else {
					secondaryClass = characterClassStatus.speedrunnerStatus;

					playerHealth.speedOffset += 6f;
					playerHealth.lightRadiusOffset -= 15;
				}

			}
			// Otherwise, if scavenger points have the greatest value of all experience point classes...
			else if (scavengerPts > brawlerPts && scavengerPts > speedrunnerPts) {
				// set primary class to Scavenger.
				primaryClass = characterClassStatus.scavengerStatus;

				playerHealth.strengthOffset -= 10;
				playerHealth.jumpOffset += 500f;
				playerHealth.lightRadiusOffset += 15;
				// If brawler points have the second greatest value of all experience point classes...
				if (brawlerPts > speedrunnerPts){
					// set secondary class to Brawler,
					secondaryClass = characterClassStatus.brawlerStatus;

					playerHealth.strengthOffset += 25f;
					playerHealth.attackSpeedOffset += 4f;
					playerHealth.speedOffset += - 2f;
				}
				// otherwise set secondary class to Speedrunner.
				else {
					secondaryClass = characterClassStatus.speedrunnerStatus;

					playerHealth.speedOffset += 6f;
					playerHealth.lightRadiusOffset -= 15;
				}

			}
			// Otherwise, if speedrunner points have the greatest value of all experience point classes...
			else if (speedrunnerPts > brawlerPts && speedrunnerPts > scavengerPts) {
				// set primary class to Speedrunner.
				primaryClass = characterClassStatus.speedrunnerStatus;

				playerHealth.speedOffset += 6f;
				playerHealth.lightRadiusOffset -= 15;
				// If brawler points have the second greatest value of all experience point classes...
				if (brawlerPts > scavengerPts){
					// set secondary class to Brawler,
					secondaryClass = characterClassStatus.brawlerStatus;

					playerHealth.strengthOffset += 15f;
					playerHealth.attackSpeedOffset += 4f;
					playerHealth.speedOffset += - 1f;
				}
				// otherwise set secondary class to Scavenger.
				else{

					secondaryClass = characterClassStatus.scavengerStatus;

					playerHealth.strengthOffset -= 10;
					playerHealth.jumpOffset += 500f;
					playerHealth.lightRadiusOffset += 10;
				}

			}
			// For all other cases, of which there should be none, keep primary and secondary classes the same.
			else
				primaryClass = primaryClass;

		} else if (brawlerPts == MAXPOINTS && scavengerPts == MAXPOINTS && speedrunnerPts == MAXPOINTS) {
			primaryClass = characterClassStatus.completionistStatus;

			playerHealth.strengthOffset += 20;
			playerHealth.attackSpeedOffset += 5;
			playerHealth.lightRadiusOffset += 20;
			playerHealth.speedOffset += 5;
			playerHealth.jumpOffset += 500;

		} else
			primaryClass = primaryClass;


		//Debug.Log ("PlayerHealth.strengthOffset"); 
	}

	public void increaseScavengerExperience(){
		previousScavengerPts = scavengerPts;
		scavengerPts += 0.25f;
	}

	public void increaseBrawlerExperience(){
		previousBrawlerPts = brawlerPts;
		brawlerPts += 0.25f;
	}

	public void increaseSpeedrunnerExperience(){
		previousSpeedrunnerPts = speedrunnerPts;
		speedrunnerPts += 0.25f;
	}



}
