using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor;

public class CreateGameObjectCommand : IEditorCommand
{
    private readonly Transform? _parent;
    
    public CreateGameObjectCommand(Transform? parent)
    {
        _parent = parent;
    }

    public void Execute()
    {
        GameObject newObj = new GameObject();
        
        SceneManager.ActiveScene.Add(newObj);
        if(_parent != null) newObj.Transform.Parent = _parent;
        
        newObj.EditorStart();
    }
}

public class DeleteGameObjectCommand : IEditorCommand
{
    private readonly GameObject _gameObject;

    public DeleteGameObjectCommand(GameObject gameobject)
    {
        _gameObject = gameobject;
    }
    
    public void Execute()
    {
        SceneManager.ActiveScene.Destroy(_gameObject);
    }
}