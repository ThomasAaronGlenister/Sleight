using Deck;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardReward : MonoBehaviour
{
    //Sprite renderer of the card reward
    [SerializeField] private SpriteRenderer mcCardRewardSpriteRenderer;

    //DeckController used for getting Card Reward and adding it to deck
    private DeckController DeckControllerAccess;

    //Skill Manager used for Getting Skill Card Reward and adding it to skills
    private SkillManager mcSkillManagerAccess;

    //Data for Deck card to be added to deck
    private Card mcRewardCard;

    //Data for skill card to be applied to player
    private Skill mcRewardSkill;

    //Type of Card Reward   0 = Card / 1 = skill
    private int mnReward = 1;

    // Start is called before the first frame update
    void Start()
    {
        //Determine what the reward is
        DeckControllerAccess = GameObject.Find("DeckController").GetComponent<DeckController>();

        if(DeckControllerAccess && mnReward == 0)
        {
            //TODO: Currently getting random Card, should make pools

            mcRewardCard = DeckControllerAccess.GetCard();

            mcCardRewardSpriteRenderer.sprite = mcRewardCard.GetCardSprite();
        }

        mcSkillManagerAccess = GameObject.Find("SkillManager").GetComponent<SkillManager>();

        if(mcSkillManagerAccess != null && mnReward == 1)
        {
            //TODO: Currently getting random Skill, should make pools with weights

            mcRewardSkill = mcSkillManagerAccess.GetSkill((int)SkillType.eeRandom);

            mcCardRewardSpriteRenderer.sprite = mcRewardSkill.GetSkillCardSprite();
        }
    }

    void Update()
    {
        CardTilt();
    }

    private void CardTilt()
    {
        float sine = Mathf.Sin(Time.time);
        float cosine = Mathf.Cos(Time.time);

        float lerpX = Mathf.LerpAngle(transform.eulerAngles.x, sine * 10, 10 * Time.deltaTime);
        float lerpY = Mathf.LerpAngle(transform.eulerAngles.y, cosine * 10, 10 * Time.deltaTime);
        float lerpZ = Mathf.LerpAngle(transform.eulerAngles.z, transform.eulerAngles.z, 10 * Time.deltaTime);

        transform.eulerAngles = new Vector3(lerpX, lerpY, lerpZ);
    }

    //Catches collider overlaps for the Chest
    private void OnTriggerEnter2D(Collider2D lcCollision)
    {
        // If the card reward is playing card
        if (DeckControllerAccess && mnReward == 0)
        {
            DeckControllerAccess.InsertNewCard(mcRewardCard);
        }
        // Else the card reward is skill card
        else if(mcSkillManagerAccess != null && mnReward == 1)
        {
            //TODO: Check for non-movement skills
            mcSkillManagerAccess.EnableSkill(mcRewardSkill);
        }

        Destroy(gameObject);
    }
}
