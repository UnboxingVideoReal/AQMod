﻿using AQMod.Items.TagItems.Starbyte;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Content.Recipes
{
    public class MolitePotionRecipe : ModRecipe
    {
        private readonly StarbytePotionTag _potion;

        public MolitePotionRecipe(Mod mod, ushort potionType) : base(mod)
        {
            _potion = new StarbytePotionTag(potionType);
        }

        public override void OnCraft(Item item)
        {
            ((MoliteTag)item.modItem).potion = _potion;
        }

        internal static void ConstructRecipe(int potionItem, ModItem item)
        {
            var recipe = new MolitePotionRecipe(item.mod, (ushort)potionItem);
            recipe.AddIngredient(ModContent.ItemType<Molite>());
            recipe.AddIngredient(potionItem);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(item);
            var molite = (MoliteTag)recipe.createItem.modItem;
            molite.potion = recipe._potion;
            molite.SetupPotionStats();
            recipe.AddRecipe();
        }
    }
}