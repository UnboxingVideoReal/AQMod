﻿using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.HookBarbs
{
    public class HookBarbsProjectile : GlobalProjectile
    {
        public bool IsBarb { get; private set; }
        public override bool InstancePerEntity => true;

        public bool CheckBarb(Projectile Projectile)
        {
            return IsBarb = Projectile.aiStyle == 7 && Projectile.friendly && !Projectile.hostile;
        }

        private HookBarbPlayer getPlayer(Projectile projectile)
        {
            return Main.player[projectile.owner].GetModPlayer<HookBarbPlayer>();
        }

        public override bool PreAI(Projectile projectile)
        {
            if (CheckBarb(projectile))
            {
                var plr = getPlayer(projectile);
                if (plr.BarbPreAI != null)
                    return plr.BarbPreAI(projectile, this, plr.player, plr);
            }
            return true;
        }

        public override void AI(Projectile projectile)
        {
            if (IsBarb)
            {
                var plr = getPlayer(projectile);
                plr.BarbAI?.Invoke(projectile, this, plr.player, plr);
            }
        }

        public override void PostAI(Projectile projectile)
        {
            if (IsBarb)
            {
                var plr = getPlayer(projectile);
                plr.BarbPostAI?.Invoke(projectile, this, plr.player, plr);
            }
        }
    }
}