using MageAFK.Core;
using MageAFK.Management;
using MageAFK.Player;
using MageAFK.Spells;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace MageAFK.UI
{
    public class SpellPurchasePopUp : PurchasePopUp
    {

        [SerializeField] private Animator animator;
        [SerializeField] private TMP_Text cost;

        private int modifiedCost = 0;

        protected override void OnEnable()
        {
            base.OnEnable();
            ServiceLocator.Get<IPlayerDeath>().SubscribeToPlayerDeath(() => OnChoicePressed(false), false);
            ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnSilverAltered, CurrencyType.SilverCoins, true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ServiceLocator.Get<IPlayerDeath>().SubscribeToPlayerDeath(() => OnChoicePressed(false), true);
            ServiceLocator.Get<CurrencyHandler>().SubscribeToCurrencyEvent(OnSilverAltered, CurrencyType.SilverCoins, true);
        }

        private void OnSilverAltered(int silver) => SetButtonStates(silver >= modifiedCost);

        public void OpenPanel(Spell spell)
        {
            title.text = spell.spellName;
            animator.runtimeAnimatorController = spell.controller;
            desc.text = spell.desc;
            modifiedCost = (int)ServiceLocator.Get<PlayerStatHandler>().ReturnModifiedValue(Stats.Stat.SpellCost, spell.cost);
            cost.text = $"<sprite name=Silver>{cost:.0f}";
            OpenPanel();
        }





    }
}
