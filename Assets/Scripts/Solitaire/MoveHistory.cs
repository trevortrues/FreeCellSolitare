using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MoveRecord
{
    public string cardName;
    public string sourceType;   
    public int sourceIndex;
    public string destType;
    public int destIndex;
    public List<string> additionalCards;  

    public MoveRecord(string card, string srcType, int srcIdx, string dstType, int dstIdx, List<string> extras = null)
    {
        cardName = card;
        sourceType = srcType;
        sourceIndex = srcIdx;
        destType = dstType;
        destIndex = dstIdx;
        additionalCards = extras ?? new List<string>();
    }
}

public class MoveHistory : MonoBehaviour
{
    private Stack<MoveRecord> undoStack = new Stack<MoveRecord>();
    private Stack<MoveRecord> redoStack = new Stack<MoveRecord>();

    public void RecordMove(MoveRecord move)
    {
        undoStack.Push(move);
        redoStack.Clear();  
        Debug.Log($"Move recorded: {move.cardName} from {move.sourceType}[{move.sourceIndex}] to {move.destType}[{move.destIndex}]");
    }

    public MoveRecord? Undo()
    {
        if (undoStack.Count == 0) return null;

        MoveRecord move = undoStack.Pop();
        redoStack.Push(move);
        Debug.Log($"Undo: {move.cardName}");
        return move;
    }

    public MoveRecord? Redo()
    {
        if (redoStack.Count == 0) return null;

        MoveRecord move = redoStack.Pop();
        undoStack.Push(move);
        Debug.Log($"Redo: {move.cardName}");
        return move;
    }

    public bool CanUndo()
    {
        return undoStack.Count > 0;
    }

    public bool CanRedo()
    {
        return redoStack.Count > 0;
    }

    public void Clear()
    {
        undoStack.Clear();
        redoStack.Clear();
    }

    public int GetUndoCount()
    {
        return undoStack.Count;
    }

    public int GetRedoCount()
    {
        return redoStack.Count;
    }
}
