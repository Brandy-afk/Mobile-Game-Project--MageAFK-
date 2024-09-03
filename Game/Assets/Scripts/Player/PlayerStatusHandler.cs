using MageAFK.AI;
using MageAFK.Spells;

namespace MageAFK.Combat
{
    public class PlayerStatusHandler : StatusHandler
    {

        private void Awake()
        {
            entity = GetComponent<Entity>();
        }
        private void Update()
        {
            OnUpdate();
        }

        public override void UpdateVisualDisplays()
        {

        }

        public override StatusType[] ReturnImmunities()
        {
            throw new System.NotImplementedException();
        }
    }
}
