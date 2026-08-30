using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Monsters;
using System;

namespace BetterThievingHoppper.Patch
{
    /// <summary>
    /// 扩展原版偷窃草蜢的偷牌优先级表。
    /// </summary>
    public static class ThievingHopperPatch
    {
        public static void Apply()
        {
            ref Func<CardModel, bool>[] stealPriorities = ref AccessTools
                .StaticFieldRefAccess<Func<CardModel, bool>[]>(
                    typeof(ThievingHopper),
                    "_stealPriorities");

            stealPriorities =
            [
                // 不是虚无的诅咒牌
                c => !c.Keywords.Contains(CardKeyword.Ethereal)
                    && c.Rarity == CardRarity.Curse,

                // 虚无的诅咒牌
                c => c.Keywords.Contains(CardKeyword.Ethereal)
                    && c.Rarity == CardRarity.Curse,

                // 没有附魔的初始牌
                c => c.Enchantment is null
                    && c.Rarity == CardRarity.Basic,

                // 有附魔的初始牌
                c => c.Enchantment is not null
                    && c.Rarity == CardRarity.Basic,

                // 没有注能附魔的蓝卡
                c => c.Enchantment is not Imbued
                    && c.Rarity == CardRarity.Uncommon,

                // 没有注能附魔的白卡、金卡与事件卡（只有事件才能生成的卡如异鸟扑击）
                c => c.Enchantment is not Imbued
                    && (c.Rarity == CardRarity.Common
                        || c.Rarity == CardRarity.Rare
                        || c.Rarity == CardRarity.Event),

                // 没有注能附魔的基础牌和任务牌
                c => c.Enchantment is not Imbued
                    && (c.Rarity == CardRarity.Basic
                        || c.Rarity == CardRarity.Quest),

                // 有注能附魔的牌和先古牌
                c => c.Enchantment is Imbued
                    || c.Rarity == CardRarity.Ancient,
            ];
        }

    }
}
