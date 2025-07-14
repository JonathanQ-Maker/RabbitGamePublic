using System.Collections;
using System.Collections.Generic;

public class FIMFAnimation : IEnumerable<FIMFSequence>
{
    private FIMFSequence[] sequences;
    private float totalDuration;
    private string name;

    public FIMFSequence this[int i] { get { return sequences[i]; } }
    public int SeqCount { get { return sequences.Length; } }
    public float Duration { get { return totalDuration; } }
    public string Name { get { return name; } }

    public FIMFAnimation(string name, float totalDuration, FIMFSequence[] sequences)
    {
        this.name = name;
        this.sequences = sequences;
        this.totalDuration = totalDuration;
    }

    public IEnumerator<FIMFSequence> GetEnumerator()
    {
        return ((IEnumerable<FIMFSequence>)sequences).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return sequences.GetEnumerator();
    }
}
