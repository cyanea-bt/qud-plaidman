﻿using System;
using Plaidman.RecoverableArrows.Events;
using Plaidman.RecoverableArrows.Utils;
using XRL.Rules;

namespace XRL.World.Parts {
	public enum HitType { Wall, Open }

	[Serializable]
	public class RA_RecoverableProjectile : IPart, IModEventHandler<RA_UninstallEvent> {
		[NonSerialized]
		public Cell CurrentCell = null;

		public int BreakChance = 50;
		public string Blueprint = "Wooden Arrow";

		public override void Register(GameObject go, IEventRegistrar registrar) {
			registrar.Register("ProjectileHit");
			registrar.Register(The.Game, RA_UninstallEvent.ID);
			base.Register(go, registrar);
		}

		public override bool FireEvent(Event e) {
			if (e.ID != "ProjectileHit") {
				return base.FireEvent(e);
			}

			GameObject defender = e.GetParameter("Defender") as GameObject;
			if (defender?.CurrentCell != null) {
				CurrentCell = defender.CurrentCell;
			}
			
			if (defender.IsCreature) {
				CheckPin(defender);
			} else {
				CheckSpawn(defender.ConsiderSolid());
			}

			return base.FireEvent(e);
		}

		public bool CheckBreak() {
			int roll = Stat.TinkerRandom(1, 100);
			if (roll <= BreakChance) {
				MessageLogger.VerboseMessage("Your " + Blueprint + " broke. [" + roll + " vs " + BreakChance + "]");
				return true;
			}
			
			MessageLogger.VerboseMessage("Your " + Blueprint + " is intact. [" + roll + " vs " + BreakChance + "]");
			return false;
		}

		public void CheckSpawn(bool isSolid) {
			if (CheckBreak()) {
				return;
			}
			
			if (isSolid) {
				CurrentCell = CurrentCell.GetCellFromDirectionOfCell(The.Player.CurrentCell);
			}

			CurrentCell.AddObject(Blueprint);
		}
		
		public void CheckPin(GameObject defender) {
			if (defender.CurrentCell == null) {
				// BeforeDeathRemovalEvent happens before ProjectileHit.
				// AddPin will have no effect, so we add the final arrow differently
				CheckSpawn(false);
				return;
			}

			if (CheckBreak()) {
				return;
			}

			var part = defender.RequirePart<RA_PinCushion>();
			part.AddPin(Blueprint);
		}
		
		public bool HandleEvent(RA_UninstallEvent e) {
			ParentObject.RemovePart(this);
			return base.HandleEvent(e);
		}
	}
}
