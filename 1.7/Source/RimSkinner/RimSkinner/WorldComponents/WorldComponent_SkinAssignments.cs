using RimWorld.Planet;
using Verse;

namespace RimSkinner.WorldComponents
{
    public sealed class WorldComponent_SkinAssignments : WorldComponent
    {
        public Dictionary<Thing, string> Assignments = new();
        private List<Thing>? assignmentThings;
        private List<string>? assignmentSkinIds;

        public static WorldComponent_SkinAssignments Instance => Find.World.GetComponent<WorldComponent_SkinAssignments>();

        public WorldComponent_SkinAssignments(World world) : base(world)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();

            if (Scribe.mode == LoadSaveMode.Saving) RemoveDestroyedAssignments();
            Scribe_Collections.Look(ref Assignments, "RimSkinner.Assignments", LookMode.Reference, LookMode.Value, ref assignmentThings, ref assignmentSkinIds);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                Assignments ??= new();
                RemoveDestroyedAssignments();
            }
        }

        private void RemoveDestroyedAssignments()
        {
            foreach (var thing in Assignments.Keys.Where(thing => thing.Destroyed).ToList())
                Assignments.Remove(thing);
        }

        public void AssignSkin(Thing building, string id)
        {
            if (building is not Building)
                return;
            Assignments.SetOrAdd(building, id);
        }

        public void UnassignSkin(Thing building)
        {
            if (building is not Building)
                return;
            Assignments.Remove(building);
        }
    }
}