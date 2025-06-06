using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace HEROsMod.HEROsModServices
{
	internal class TeleportToPlayers : HEROsModService
	{
		public static TeleportToPlayers instance;

		public TeleportToPlayers()
		{
			instance = this;

			// Doesn't work, needs team which is annoying.
			//On_Player.HasUnityPotion += On_Player_HasUnityPotion;
			//On_Player.TakeUnityPotion += On_Player_TakeUnityPotion;
		}
		
		//private bool On_Player_HasUnityPotion(On_Player.orig_HasUnityPotion orig, Player self)
		//{
		//	if (HasPermissionToUse)
		//		return true;
		//	return orig(self);
		//}

		//private void On_Player_TakeUnityPotion(On_Player.orig_TakeUnityPotion orig, Player self)
		//{
		//	if (HasPermissionToUse)
		//		return;
		//	orig(self);
		//}

		public override void MyGroupUpdated()
		{
			this.HasPermissionToUse = HEROsModNetwork.LoginService.MyGroup.HasPermission("TeleportToPlayers");
		}

		public void PostDrawFullScreenMap(ref string mouseText)
		{
			if (!HasPermissionToUse || Main.netMode != NetmodeID.MultiplayerClient)
			{
				return;
			}

			for (int i = 0; i < 255; i++)
			{
				Player otherPlayer = Main.player[i];

				if (!otherPlayer.active || otherPlayer.dead || i == Main.myPlayer)
					continue;

				float UIScale = Main.UIScale;
				Vector2 mapPosition = Main.mapFullscreenPos;
				float offsetX = 0f - mapPosition.X * Main.mapFullscreenScale + (Main.screenWidth / 2);
				float offsetY = 0f - mapPosition.Y * Main.mapFullscreenScale + (Main.screenHeight / 2);
				offsetX += 10 * Main.mapFullscreenScale;
				offsetY += 10 * Main.mapFullscreenScale;
				float playerHeadDrawX = ((otherPlayer.position.X + (otherPlayer.width / 2)) / 16f) * Main.mapFullscreenScale;
				float playerHeadDrawY = ((otherPlayer.position.Y + otherPlayer.gfxOffY + (otherPlayer.height / 2)) / 16f) * Main.mapFullscreenScale;
				playerHeadDrawX += offsetX;
				playerHeadDrawY += offsetY;
				playerHeadDrawX -= 6f;
				playerHeadDrawY -= 2f;
				playerHeadDrawY -= 2f - Main.mapFullscreenScale / 5f * 2f;
				playerHeadDrawX -= 10f * Main.mapFullscreenScale;
				playerHeadDrawY -= 10f * Main.mapFullscreenScale;
				float playerLeft = playerHeadDrawX + 4f - 14f * UIScale;
				float playerTop = playerHeadDrawY + 2f - 14f * UIScale;
				float playerRight = playerLeft + 28f * UIScale;
				float playerBottom = playerTop + 28f * UIScale;

				if (!(Main.mouseX >= playerLeft) || !(Main.mouseX <= playerRight) || !(Main.mouseY >= playerTop) || !(Main.mouseY <= playerBottom))
					continue;

				mouseText = otherPlayer.name;
				if (!Main.cancelWormHole)
				{
					mouseText = Language.GetTextValue("Game.TeleportTo", otherPlayer.name + " (Free Teleport)");
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						Main.mouseLeftRelease = false;
						Main.mapFullscreen = false;
						Main.player[Main.myPlayer].UnityTeleport(otherPlayer.position);
					}
				}
			}

			Main.cancelWormHole = true; // Should prevent real player head code from running unity teleport code.
		}
	}
}