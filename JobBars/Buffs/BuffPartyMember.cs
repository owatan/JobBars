﻿using JobBars.Data;
using System.Collections.Generic;

namespace JobBars.Buffs {
    public class BuffPartyMember {
        private JobIds CurrentJob = JobIds.OTHER;
        private readonly List<BuffTracker> Trackers = new();
        private readonly uint ObjectId;
        private readonly bool IsPlayer;

        public BuffPartyMember(uint objectId, bool isPlayer) {
            ObjectId = objectId;
            IsPlayer = isPlayer;
        }

        public bool Tick(HashSet<BuffTracker> trackers, CurrentPartyMember partyMember) {
            if (CurrentJob != partyMember.Job) {
                CurrentJob = partyMember.Job;
                SetupTrackers();
            }

            var highlightMember = false;
            foreach (var tracker in Trackers) {
                tracker.Tick(partyMember.BuffDict);
                if (tracker.Enabled) trackers.Add(tracker);
                if (tracker.Highlighted) highlightMember = true;
            }
            return highlightMember;
        }

        public void ProcessAction(Item action, uint objectId) {
            if (ObjectId != objectId) return;
            foreach (var tracker in Trackers) tracker.ProcessAction(action);
        }

        public void SetupTrackers() {
            Trackers.Clear();

            var trackerProps = JobBars.BuffManager.GetBuffConfigs(CurrentJob, IsPlayer);
            foreach (var prop in trackerProps) {
                if (!prop.Enabled) continue;
                Trackers.Add(new BuffTracker(prop));
            }
        }

        public void Reset() {
            foreach (var item in Trackers) item.Reset();
        }
    }
}
