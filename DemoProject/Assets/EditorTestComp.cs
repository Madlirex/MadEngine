using MadEngine;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

public class EditorTestComp : Component
{
	public GameObject? obj;
	public override void Update(float delta)
	{
		if (obj == null) return;

		obj.Transform.Position.Z -= delta;
	}
}