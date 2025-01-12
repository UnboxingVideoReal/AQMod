﻿using AQMod.Items.TagItems.ContainersPotion;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Content.Recipes
{
    public class ContainerPotionRecipe : ModRecipe
    {
        private readonly ContainerTag _chestTagCache;

        public ContainerPotionRecipe(Mod mod, int chest) : base(mod)
        {
            _chestTagCache = new ContainerTag(chest);
        }

        public override void OnCraft(Item item)
        {
            ((PotionofContainersTag)item.modItem).chestTag = _chestTagCache;
        }

        internal static void ConstructRecipe(int chestItem, ModItem item)
        {
            var recipe = new ContainerPotionRecipe(item.mod, (ushort)chestItem);
            recipe.AddIngredient(ModContent.ItemType<PotionofContainers>());
            recipe.AddIngredient(chestItem);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(item);
            ((PotionofContainersTag)recipe.createItem.modItem).chestTag = recipe._chestTagCache; // so that it shows in the crafting menu
            recipe.AddRecipe();
        }
    }
}