using MageAFK.AI;
using MageAFK.Spells;
using MageAFK.UI;



namespace MageAFK.Combat
{
  public class AIStatusHandler : StatusHandler
  {

    private EntityUIController uI;
    private AIShaderController shader;

    public void SetEntityUIController(EntityUIController controller)
    {
      uI = controller;
      uI.SetStatusHandler(this);
    }

    private void Awake()
    {
      entity = GetComponent<Entity>();
      shader = GetComponent<AIShaderController>();
      shader.SetStatusHandler(this);
    }
    private void Update()
    {
      OnUpdate();
    }

    public override void UpdateVisualDisplays()
    {
      shader.UpdateShaderDisplay();
      uI.UpdateStatusDisplay();
    }

    public override StatusType[] ReturnImmunities() => (entity as NPEntity).data.GetImmunities();
  }
}
