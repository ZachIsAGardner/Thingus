namespace Thingus;

public class HistoryStep
{
    public bool Committed = false;
    public List<HistoryAction> Actions = new List<HistoryAction>() { };

    public void Clear()
    {
        Committed = false;
        Actions.Clear();
    }
}

public class HistoryAction
{
    public string Name;
    public Action Undo;
    public Action Redo;

    public HistoryAction() { }
    public HistoryAction(string name, Action undo, Action redo) 
    {
        Name = name;
        Undo = undo;
        Redo = redo;
    }
}